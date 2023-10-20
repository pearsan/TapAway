using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MagicExcel {
    [JsonConverter(typeof(EncryptedTypeJsonConverter))]
    [Serializable]
    public struct EncryptedLong : IEncryptedType {
        private const long KEY = 82380971L;

        [SerializeField]
        private long encryptedValue;

        public EncryptedLong(long value) {
            encryptedValue = value ^ KEY;
        }

        public object Decrypt() => Decrypt(encryptedValue);

        public static long Decrypt(long encryptedValue) 
            => encryptedValue ^ KEY;

        public static implicit operator long(EncryptedLong value) 
            => Decrypt(value.encryptedValue);

        public override string ToString() => Decrypt().ToString();
    }
}