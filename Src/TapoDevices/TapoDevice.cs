using System;
using System.Linq;
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
    public class TapoDevice // TODO: TapoPlug
    {
        private readonly string username;

        private readonly string password;

        private readonly TimeSpan defaultRequestTimeout = TimeSpan.FromSeconds(3);

        private readonly HttpClient client;

        private string token;

        private ICryptoTransform encryptor;

        private ICryptoTransform decryptor;

        /// <summary>
        /// Creates an instance of connection to Tapo device.
        /// </summary>
        /// <param name="ipAddress">IP address of device in local network.</param>
        /// <param name="username">User name.</param>
        /// <param name="password">Password.</param>
        /// <param name="defaultRequestTimeout">Default request timeout.</param>
        public TapoDevice(
            string ipAddress,
            string username,
            string password,
            TimeSpan defaultRequestTimeout)
        {
            this.username = username;
            this.password = password;
            this.defaultRequestTimeout = defaultRequestTimeout;

            this.client = new HttpClient
            {
                Timeout = this.defaultRequestTimeout,
                BaseAddress = new Uri($"http://{ipAddress}/app")
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
            string password) : this(ipAddress, username, password, TimeSpan.FromSeconds(3))
        {

        }

        /// <summary>
        /// Connect to device using Tapo account credentials.
        /// </summary>
        /// <returns>Task representing connection.</returns>
        public async Task ConnectAsync()
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
            var hashedUsernameHexBytes = Encoding.UTF8.GetBytes(Convert.ToHexString(hashedUsername).ToLower());
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

        // TODO: more control methods
        // TODO: Turn on/off with delay ("add_countdown_rule")

        #endregion

        private bool IsConnected => this.encryptor != null && this.decryptor != null && this.token != null;

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

            var encoded = Utils.SecureEncode(this.encryptor, request);
            var response = await this.PostAsync<SecurePassthrough.Params, SecurePassthrough.Result>(encoded, this.token);
            var decoded = Utils.SecureDecode<TResult>(this.decryptor, response.Result);

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
            var deserialized = await response.Content.ReadFromJsonAsync<TapoResponse<TResult>>(Utils.SerializerOptions);
            return deserialized;
        }
    }
}
