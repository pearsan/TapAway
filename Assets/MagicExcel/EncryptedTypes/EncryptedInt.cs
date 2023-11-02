using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MagicExcel {
    [JsonConverter(typeof(EncryptedTypeJsonConverter))]
    [Serializable]
    public struct EncryptedInt : IEncryptedType {
        private const int KEY = 82380971;

        [SerializeField]
        private int encryptedValue;

        public EncryptedInt(int value) {
            encryptedValue = value ^ KEY;
        }

        public object Decrypt() => Decrypt(encryptedValue);

        public static int Decrypt(int encryptedValue)
            => encryptedValue ^ KEY;

        public static implicit operator int(EncryptedInt value)
            => Decrypt(value.encryptedValue);

        public override string ToString() => Decrypt().ToString();
    }
}