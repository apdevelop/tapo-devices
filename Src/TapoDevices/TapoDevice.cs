using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TapoDevices
{
    /// <summary>
    /// Represents connection to generic Tapo device.
    /// </summary>
    public class TapoDevice
    {
        private readonly string username;

        private readonly string password;

        private readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(5);

        private readonly HttpClient client;

        private readonly CookieContainer cookies;

        private string token;

        private ICryptoTransform encryptor;

        private ICryptoTransform decryptor;

        private KlapCipher klapSession;

        /// <summary>
        /// Creates an instance of connection to Tapo device.
        /// </summary>
        /// <param name="ipAddress">IP address of device in local network.</param>
        /// <param name="username">User name.</param>
        /// <param name="password">Password.</param>
        /// <param name="defaultTimeout">Default request timeout.</param>
        public TapoDevice(
            string ipAddress,
            string username,
            string password,
            TimeSpan defaultTimeout)
        {
            this.username = username;
            this.password = password;
            this.defaultTimeout = defaultTimeout;

            this.cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookies
            };

            this.client = new HttpClient(handler)
            {
                Timeout = this.defaultTimeout,
                BaseAddress = new Uri($"http://{ipAddress}/app/"), // ending slash must be here
            };
        }

        /// <summary>
        /// Creates an instance of connection to Tapo device.
        /// </summary>
        /// <param name="ipAddress">IP address of device in local network.</param>
        /// <param name="username">User name.</param>
        /// <param name="password">Password.</param>
        public TapoDevice(
            string ipAddress,
            string username,
            string password) : this(ipAddress, username, password, TimeSpan.FromSeconds(5))
        {

        }

        /// <summary>
        /// Connect to device using Tapo account credentials.
        /// </summary>
        /// <returns>Task representing connection.</returns>
        public async Task ConnectAsync()
        {
            var localSeed = GenerateLocalSeed();

            var r = await this.client.PostAsync("handshake1", new ByteArrayContent(localSeed));
            r.EnsureSuccessStatusCode(); // TODO: ? fallback to SecurePassthrough

            var responseContent = await r.Content.ReadAsByteArrayAsync();
            var remoteSeed = responseContent.Skip(0).Take(16).ToArray();
            var serverHash = responseContent.Skip(16).ToArray();
            var localAuthHash = CreateAuthHash(this.username, this.password);

            var ls = new List<byte>();
            ls.AddRange(localSeed);
            ls.AddRange(remoteSeed);
            ls.AddRange(localAuthHash);

            var localSeedAuthHash = SHA256.HashData(ls.ToArray());

            if (Utils.ByteArraysEqual(localSeedAuthHash, serverHash))
            {
                var handshake2 = new List<byte>();
                handshake2.AddRange(remoteSeed);
                handshake2.AddRange(localSeed);
                handshake2.AddRange(localAuthHash);

                var payload = SHA256.HashData(handshake2.ToArray());

                var response2 = await this.client.PostAsync("handshake2", new ByteArrayContent(payload));
                response2.EnsureSuccessStatusCode();

                this.klapSession = new KlapCipher(localSeed, remoteSeed, localAuthHash);
            }
            else
            {
                throw new InvalidOperationException("Error while performing handshake.");
            }
        }

        private static byte[] GenerateLocalSeed()
        {
            var bytes = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            return bytes;
        }

        private static byte[] CreateAuthHash(string username, string password)
        {
            var usernameHash = SHA1.HashData(Encoding.UTF8.GetBytes(username));
            var passwordHash = SHA1.HashData(Encoding.UTF8.GetBytes(password));
            var authHashSource = new List<byte>();
            authHashSource.AddRange(usernameHash);
            authHashSource.AddRange(passwordHash);

            return SHA256.HashData(authHashSource.ToArray());
        }

        /// <summary>
        /// Connect to device using Tapo account credentials (Obsolete since 2023 firmwares).
        /// </summary>
        /// <returns>Task representing connection.</returns>
        public async Task ConnectOldAsync()
        {
            var key = RSA.Create(1024);
            var publicKey = key.ExportSubjectPublicKeyInfo(); // TODO: replacement for .NET Standard 2.0
            var publicKeyWrapped = $"-----BEGIN PUBLIC KEY-----\n{Convert.ToBase64String(publicKey)}\n-----END PUBLIC KEY-----\n";

            var handshakeRequest = Handshake.CreateRequest(new Handshake.Params { Key = publicKeyWrapped });
            var handshakeResponse = await this.PostAsync<Handshake.Params, Handshake.Result>(handshakeRequest);
            if (handshakeResponse.ErrorCode != (int)ErrorCode.Success)
            {
                throw new InvalidOperationException($"Handshake error, code {handshakeResponse.ErrorCode} ({((ErrorCode)handshakeResponse.ErrorCode).GetDescription()})."); // TODO: ? proper exception class
            }

            var handshakeResult = handshakeResponse.Result;

            var encryptionParts = key.Decrypt(Convert.FromBase64String(handshakeResult.Key), RSAEncryptionPadding.Pkcs1);
            var aesKey = encryptionParts.Take(16).ToArray();
            var aesIV = encryptionParts.Skip(16).Take(16).ToArray();

            var aes = Aes.Create();
            this.encryptor = aes.CreateEncryptor(aesKey, aesIV);
            this.decryptor = aes.CreateDecryptor(aesKey, aesIV);

            var hashedUsername = SHA1.HashData(Encoding.UTF8.GetBytes(this.username));
            var hashedUsernameHexBytes = Encoding.UTF8.GetBytes(Utils.ToHexString(hashedUsername).ToLower());
            var loginRequest = LoginDevice.CreateRequest(new LoginDevice.Params
            {
                Username = Convert.ToBase64String(hashedUsernameHexBytes, Base64FormattingOptions.InsertLineBreaks),
                Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.password), Base64FormattingOptions.InsertLineBreaks),
            });

            var loginRequestEncoded = Utils.SecureEncode(this.encryptor, loginRequest);
            var loginResponse = await this.PostAsync<SecurePassthrough.Params, SecurePassthrough.Result>(loginRequestEncoded);
            var loginDecoded = Utils.SecureDecode<LoginDevice.Result>(this.decryptor, loginResponse.Result);
            if (loginDecoded.ErrorCode != (int)ErrorCode.Success)
            {
                throw new InvalidOperationException($"Login error, code {loginDecoded.ErrorCode} ({((ErrorCode)loginDecoded.ErrorCode).GetDescription()})."); // TODO: ? proper exception class
            }

            this.token = loginDecoded.Result.Token;
        }

        #region Application-level device control

        public async Task<GetDeviceInfo.Result> GetInfoAsync()
        {
            var request = GetDeviceInfo.CreateRequest();
            return await PostSecuredAsync<TapoRequest<GetDeviceInfo.Params>, GetDeviceInfo.Result>(request);
        }

        public async Task TurnOnAsync()
        {
            var request = SetDeviceInfo.CreateRequest(new SetDeviceInfo.Params { DeviceOn = true });
            await PostSecuredAsync<TapoRequest<SetDeviceInfo.Params>, SetDeviceInfo.Result>(request);
        }

        public async Task TurnOffAsync()
        {
            var request = SetDeviceInfo.CreateRequest(new SetDeviceInfo.Params { DeviceOn = false });
            await PostSecuredAsync<TapoRequest<SetDeviceInfo.Params>, SetDeviceInfo.Result>(request);
        }

        public async Task TurnOnWithDelayAsync(TimeSpan delay)
        {
            var request = AddCountdownRule.CreateRequest(new AddCountdownRule.Params
            {
                Enable = true,
                Delay = (int)delay.TotalSeconds,
                DesiredStates = new AddCountdownRule.ParamsStates { On = true, },
            });

            await PostSecuredAsync<TapoRequest<AddCountdownRule.Params>, AddCountdownRule.Result>(request);
        }

        public async Task TurnOffWithDelayAsync(TimeSpan delay)
        {
            var request = AddCountdownRule.CreateRequest(new AddCountdownRule.Params
            {
                Enable = true,
                Delay = (int)delay.TotalSeconds,
                DesiredStates = new AddCountdownRule.ParamsStates { On = false, },
            });

            await PostSecuredAsync<TapoRequest<AddCountdownRule.Params>, AddCountdownRule.Result>(request);
        }

        public async Task<GetCountdownRules.Result> GetCountdownRulesAsync()
        {
            var request = GetCountdownRules.CreateRequest();
            return await PostSecuredAsync<TapoRequest<GetCountdownRules.Params>, GetCountdownRules.Result>(request);
        }

        public async Task<GetDeviceTime.Result> GetDeviceTimeAsync()
        {
            var request = GetDeviceTime.CreateRequest();
            return await PostSecuredAsync<TapoRequest<GetDeviceTime.Params>, GetDeviceTime.Result>(request);
        }

        // TODO: more control methods

        #endregion

        private bool IsConnected => this.klapSession != null || (this.encryptor != null && this.decryptor != null && this.token != null);

        protected void ValidateConnection()
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException($"No connection to device was established.");
            }
        }

        protected async Task<TResult> PostSecuredAsync<TRequest, TResult>(TRequest request)
        {
            this.ValidateConnection();

            TapoResponse<TResult> decoded;

            if (this.klapSession != null)
            {
                var payload = this.klapSession.Encrypt(Utils.Serialize(request));
                var response = await this.client.PostAsync("request?seq=" + this.klapSession.SeqCounter, new ByteArrayContent(payload));
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsByteArrayAsync();
                var decryptedResponse = this.klapSession.Decrypt(responseData);
                decoded = Utils.Deserialize<TapoResponse<TResult>>(decryptedResponse);
            }
            else
            {
                var encoded = Utils.SecureEncode(this.encryptor, request);
                var response = await this.PostAsync<SecurePassthrough.Params, SecurePassthrough.Result>(encoded, this.token);
                decoded = Utils.SecureDecode<TResult>(this.decryptor, response.Result);
            }

            if (decoded.ErrorCode != (int)ErrorCode.Success)
            {
                throw new InvalidOperationException($"Command execution error, code {decoded.ErrorCode} ({((ErrorCode)decoded.ErrorCode).GetDescription()})."); // TODO: ? proper exception class
            }

            return decoded.Result;
        }

        private async Task<TapoResponse<TResult>> PostAsync<TRequest, TResult>(TapoRequest<TRequest> request, string token = null)
        {
            var serialized = Utils.Serialize(request);
            var content = new ByteArrayContent(serialized);
            content.Headers.ContentType = new MediaTypeHeaderValue(Utils.JsonMediaType);
            var url = String.IsNullOrEmpty(token) ? String.Empty : $"?token={token}";
            var response = await this.client.PostAsync(url, content);
            var responseData = await response.Content.ReadAsByteArrayAsync();
            var deserialized = Utils.Deserialize<TapoResponse<TResult>>(responseData);
            // TODO: ! check var deserialized = await response.Content.ReadFromJsonAsync<TapoResponse<TResult>>(Utils.SerializerOptions);
            return deserialized;
        }
    }
}
