using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Vec3Convert : JsonConverter<Vector3> {
    public override bool CanRead => true;
    public override bool CanWrite => true;

    public override Vector3 ReadJson(JsonReader reader, System.Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer) {
        if (reader.TokenType == JsonToken.StartArray) {
            JArray arr = JArray.Load(reader);
            return new Vector3(arr[0].Value<float>(), arr[1].Value<float>(), arr[2].Value<float>());
        }
        return default;
    }

    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer) {
        JArray arr = new JArray(value.x, value.y, value.z);
        arr.WriteTo(writer);
    }
}

