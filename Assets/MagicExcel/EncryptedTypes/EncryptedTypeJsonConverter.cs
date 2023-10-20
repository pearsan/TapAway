using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace MagicExcel {
    public class EncryptedTypeJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType.GetInterfaces().Contains(typeof(IEncryptedType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var token = JToken.Load(reader);
            if (objectType == typeof(EncryptedDouble)) {
                return new EncryptedDouble(token.ToObject<double>());
            } else if (objectType == typeof(EncryptedFloat)) {
                return new EncryptedFloat(token.ToObject<float>());
            } else if (objectType == typeof(EncryptedInt)) {
                return new EncryptedInt(token.ToObject<int>());
            } else if (objectType == typeof(EncryptedLong)) {
                return new EncryptedLong(token.ToObject<long>());
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            IEncryptedType obj = (IEncryptedType)value;
            JToken t = JToken.FromObject(obj.Decrypt());
            t.WriteTo(writer);
        }
    }
}
