using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MagicExcel {
    [JsonConverter(typeof(EncryptedTypeJsonConverter))]
    [Serializable]
    public struct EncryptedDouble : IEncryptedType {
        private const long KEY = 82380971L;

        [SerializeField]
        private long encryptedValue;

        public EncryptedDouble(double value) {
            encryptedValue = BitConverter.DoubleToInt64Bits(value) ^ KEY;
        }

        public object Decrypt() => Decrypt(encryptedValue);

        public static double Decrypt(long encryptedValue)
            => BitConverter.Int64BitsToDouble(encryptedValue ^ KEY);

        public static implicit operator double(EncryptedDouble value)
            => Decrypt(value.encryptedValue);

        public override string ToString() => Decrypt().ToString();
    }
}