using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace TapoDevices
{
    static class Utils
    {
        public const string JsonMediaType = "application/json";

        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        private static long MillisecondsNow =>
            (long)Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);

        public static bool ByteArraysEqual(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2) => 
            a1.SequenceEqual(a2);

        // Convert.ToHexString implementation for .NET Standard2.0
        public static string ToHexString(byte[] bytes) => String.Join(String.Empty, bytes.Select(b => b.ToString("X2")));

        public static TapoRequest<TParams> CreateTapoRequest<TParams>(string method, TParams parameters)
        {
            return new TapoRequest<TParams>
            {
                Method = method,
                Parameters = parameters,
                RequestTimeMilliseconds = MillisecondsNow,
                // TODO: ? terminalUUID = Guid in '65fa8d8d1b8cd' form ?
            };
        }

        public static byte[] Serialize<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj, SerializerOptions);
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            var jsonUtfReader = new Utf8JsonReader(new ReadOnlySpan<byte>(bytes));
            return JsonSerializer.Deserialize<T>(ref jsonUtfReader, SerializerOptions);
        }

        public static T Deserialize<T>(string jsonString)
        {
            return JsonSerializer.Deserialize<T>(jsonString, SerializerOptions);
        }

        public static TapoRequest<SecurePassthrough.Params> SecureEncode<TRequest>(ICryptoTransform encryptor, TRequest request)
        {
            var requestSerializedBytes = Utils.Serialize(request);
            var secured = Convert.ToBase64String(encryptor.TransformFinalBlock(requestSerializedBytes, 0, requestSerializedBytes.Length));
            return SecurePassthrough.CreateRequest(new SecurePassthrough.Params { Request = secured });
        }

        public static TapoResponse<TResult> SecureDecode<TResult>(ICryptoTransform decryptor, SecurePassthrough.Result response)
        {
            var responseBytes = Convert.FromBase64String(response.Response);
            var decrypted = decryptor.TransformFinalBlock(responseBytes, 0, responseBytes.Length);
            var deserialized = Utils.Deserialize<TapoResponse<TResult>>(decrypted);
            return deserialized;
        }
    }
}
