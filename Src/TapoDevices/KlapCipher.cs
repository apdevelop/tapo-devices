using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TapoDevices
{
    class KlapCipher
    {
        private uint seq;

        private readonly byte[] key;
        private readonly byte[] iv;
        private readonly byte[] sig;

        public KlapCipher(byte[] localSeed, byte[] remoteSeed, byte[] userHash)
        {
            this.key = KeyDerive(localSeed, remoteSeed, userHash);
            this.iv = this.IvDerive(localSeed, remoteSeed, userHash);
            this.sig = SigDerive(localSeed, remoteSeed, userHash);
        }

        public uint SeqCounter => this.seq;

        public byte[] Encrypt(byte[] plainText)
        {
            this.seq++;

            Aes cipher = Aes.Create();
            cipher.Padding = PaddingMode.None;
            cipher.Key = this.key;
            cipher.IV = IvSeq();
            var paddedData = Pkcs7Pad(plainText);
            var ciphertext = cipher.CreateEncryptor().TransformFinalBlock(paddedData, 0, paddedData.Length);
            var seqBytes = BitConverter.GetBytes(this.seq);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(seqBytes);
            }

            var dataToHash = new byte[this.sig.Length + seqBytes.Length + ciphertext.Length];
            Buffer.BlockCopy(this.sig, 0, dataToHash, 0, this.sig.Length);
            Buffer.BlockCopy(seqBytes, 0, dataToHash, this.sig.Length, seqBytes.Length);
            Buffer.BlockCopy(ciphertext, 0, dataToHash, this.sig.Length + seqBytes.Length, ciphertext.Length);

            var signature = SHA256.HashData(dataToHash);
            var result = new byte[signature.Length + ciphertext.Length];
            Buffer.BlockCopy(signature, 0, result, 0, signature.Length);
            Buffer.BlockCopy(ciphertext, 0, result, signature.Length, ciphertext.Length);

            return result;
        }

        public byte[] Decrypt(byte[] cipherText)
        {
            var cipher = Aes.Create();
            cipher.Padding = PaddingMode.None;
            cipher.Key = this.key;
            cipher.IV = IvSeq();

            cipherText = cipherText.Skip(32).ToArray();
            var dp = cipher.CreateDecryptor().TransformFinalBlock(cipherText, 0, cipherText.Length);
            var plainText = Pkcs7Unpad(dp);

            return plainText;
        }

        private static byte[] KeyDerive(byte[] localSeed, byte[] remoteSeed, byte[] userHash)
        {
            var payload = new List<byte>();
            payload.AddRange(Encoding.Default.GetBytes("lsk"));
            payload.AddRange(localSeed);
            payload.AddRange(remoteSeed);
            payload.AddRange(userHash);

            var key = SHA256.HashData(payload.ToArray());

            return key.Take(16).ToArray();
        }

        private byte[] IvDerive(byte[] localSeed, byte[] remoteSeed, byte[] userHash)
        {
            var payload = new List<byte>();
            payload.AddRange(Encoding.Default.GetBytes("iv"));
            payload.AddRange(localSeed);
            payload.AddRange(remoteSeed);
            payload.AddRange(userHash);

            var fulliv = SHA256.HashData(payload.ToArray());

            var seqBytes = fulliv.Skip(fulliv.Length - 4).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(seqBytes);
            }

            this.seq = BitConverter.ToUInt32(seqBytes, 0);

            return fulliv.Take(12).ToArray();
        }

        private static byte[] SigDerive(byte[] localSeed, byte[] remoteSeed, byte[] userHash)
        {
            var payload = new List<byte>();
            payload.AddRange(Encoding.Default.GetBytes("ldk"));
            payload.AddRange(localSeed);
            payload.AddRange(remoteSeed);
            payload.AddRange(userHash);

            var sig = SHA256.HashData(payload.ToArray());

            return sig.Take(28).ToArray();
        }

        private byte[] IvSeq()
        {
            var seqBytes = BitConverter.GetBytes(this.seq);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(seqBytes);
            }

            var iv = new byte[this.iv.Length + seqBytes.Length];
            Buffer.BlockCopy(this.iv, 0, iv, 0, this.iv.Length);
            Buffer.BlockCopy(seqBytes, 0, iv, this.iv.Length, seqBytes.Length);

            return iv;
        }

        private static byte[] Pkcs7Pad(byte[] data)
        {
            const int BlockSize = 16;
            var padding = BlockSize - (data.Length % BlockSize);
            var paddedData = new byte[data.Length + padding];
            Buffer.BlockCopy(data, 0, paddedData, 0, data.Length);
            for (var i = data.Length; i < paddedData.Length; i++)
            {
                paddedData[i] = (byte)padding;
            }

            return paddedData;
        }

        private static byte[] Pkcs7Unpad(byte[] data)
        {
            var padLength = data[data.Length - 1];
            if (padLength < 1 || padLength > 16)
            {
                return data;
            }

            return data.Take(data.Length - padLength).ToArray();
        }
    }
}
