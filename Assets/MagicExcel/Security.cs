using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MagicExcel {
    public static class Security {
        //public static void GenerateKeyAndIV() {
        //    using (var rm = new System.Security.Cryptography.RijndaelManaged()) {
        //        rm.GenerateKey();
        //        rm.GenerateIV();
        //        Debug.Log(string.Join(", ", rm.Key));
        //        Debug.Log(string.Join(", ", rm.IV));
        //    }
        //}

        private static readonly byte[] key = {
            194, 248, 146, 51, 195, 80, 69, 4, 112, 104, 121, 191, 230, 52, 139, 10, 139, 75, 179, 215, 6, 132, 224, 57, 24, 207, 18, 132, 115, 144, 246, 244
        };

        private static readonly byte[] vector = {
            119, 125, 173, 161, 225, 239, 102, 158, 201, 227, 214, 235, 75, 132, 58, 152
        };

        private static readonly ICryptoTransform encryptor;
        private static readonly ICryptoTransform decryptor;
        private static readonly UTF8Encoding encoder = new UTF8Encoding();

        static Security() {
            //Trộn thêm một bước nữa cho chắc ăn
            for (int i = 0; i < key.Length; i++) {
                key[i] = (byte)(key[i] * 5);
            }
            for (int i = 0; i < vector.Length; i++) {
                vector[i] = (byte)(vector[i] * 11);
            }
            using (var rm = new RijndaelManaged { Padding = PaddingMode.Zeros }) {
                encryptor = rm.CreateEncryptor(key, vector);
                decryptor = rm.CreateDecryptor(key, vector);
            }
        }

        public static string Encrypt(this string unencrypted) {
            return Convert.ToBase64String(Encrypt(encoder.GetBytes(unencrypted)));
        }

        public static string Decrypt(this string encrypted) {
            return encoder.GetString(Decrypt(Convert.FromBase64String(encrypted)));
        }

        public static byte[] Encrypt(byte[] buffer) {
            return Transform(buffer, encryptor);
        }

        public static byte[] Decrypt(byte[] buffer) {
            return Transform(buffer, decryptor);
        }

        private static byte[] Transform(byte[] buffer, ICryptoTransform transform) {
            using (var stream = new MemoryStream()) {
                using (var cs = new CryptoStream(stream, transform, CryptoStreamMode.Write)) {
                    cs.Write(buffer, 0, buffer.Length);
                }
                return stream.ToArray();
            }
        }
    }
}
