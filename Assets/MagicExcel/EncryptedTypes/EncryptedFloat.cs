using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MagicExcel {
    [JsonConverter(typeof(EncryptedTypeJsonConverter))]
    [Serializable]
    public struct EncryptedFloat : IEncryptedType {
        private const int KEY = 82380971;

        [SerializeField]
        private int encryptedValue;

        public EncryptedFloat(float value) {
            encryptedValue = BitConverter.ToInt32(BitConverter.GetBytes(value), 0) ^ KEY;
        }

        public object Decrypt() => Decrypt(encryptedValue);

        public static float Decrypt(int encryptedValue)
            => BitConverter.ToSingle(BitConverter.GetBytes(encryptedValue ^ KEY), 0);

        public static implicit operator float(EncryptedFloat value)
            => Decrypt(value.encryptedValue);

        public override string ToString() => Decrypt().ToString();
    }
}