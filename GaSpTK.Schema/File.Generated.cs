// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using GaSpTK.Schema;
//
//    var file = File.FromJson(jsonString);

namespace GaSpTK.Schema
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// A file containing a collection of sprite animations authored with GaSpTK
    /// </summary>
    public partial class File
    {
        /// <summary>
        /// Collection of sprite animations defined in this file
        /// </summary>
        [JsonProperty("animation", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<SpriteAnim> Animation { get; set; }

        /// <summary>
        /// Collection of sprite atlas definitions referenced by animations in this file
        /// </summary>
        [JsonProperty("atlas", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<SpriteAtlas> Atlas { get; set; }

        /// <summary>
        /// Collection of information about events which may be triggered by animations in this file
        /// </summary>
        [JsonProperty("event", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<EventInfo> Event { get; set; }

        /// <summary>
        /// Information about properties which can be defined in an animation's metadata
        /// </summary>
        [JsonProperty("metadata", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<MetaPropInfo> Metadata { get; set; }
    }

    public partial class SpriteAnim
    {
        [JsonProperty("duration", Required = Required.Always)]
        public long Duration { get; set; }

        [JsonProperty("events", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<Event> Events { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("metadata", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<Metadatum> Metadata { get; set; }

        [JsonProperty("rectLayers", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<RectLayer> RectLayers { get; set; }

        [JsonProperty("spriteLayers", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<SpriteLayer> SpriteLayers { get; set; }
    }

    public partial class Event
    {
        [JsonProperty("eventId", Required = Required.Always)]
        public string EventId { get; set; }

        [JsonProperty("location", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Location Location { get; set; }

        [JsonProperty("params", Required = Required.Always)]
        public List<object> Params { get; set; }

        [JsonProperty("time", Required = Required.Always)]
        public long Time { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("x", Required = Required.Always)]
        public long X { get; set; }

        [JsonProperty("y", Required = Required.Always)]
        public long Y { get; set; }
    }

    public partial class Metadatum
    {
        [JsonProperty("data", Required = Required.Always)]
        public Dictionary<string, object> Data { get; set; }

        [JsonProperty("time", Required = Required.Always)]
        public long Time { get; set; }
    }

    public partial class RectLayer
    {
        [JsonProperty("frames", Required = Required.Always)]
        public List<RectLayerFrame> Frames { get; set; }

        [JsonProperty("groupId", Required = Required.Always)]
        public string GroupId { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public long Id { get; set; }
    }

    public partial class RectLayerFrame
    {
        [JsonProperty("active", Required = Required.Always)]
        public bool Active { get; set; }

        [JsonProperty("height", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? Height { get; set; }

        [JsonProperty("time", Required = Required.Always)]
        public long Time { get; set; }

        [JsonProperty("width", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? Width { get; set; }

        [JsonProperty("x", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? X { get; set; }

        [JsonProperty("y", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? Y { get; set; }
    }

    public partial class SpriteLayer
    {
        [JsonProperty("atlasId", Required = Required.Always)]
        public string AtlasId { get; set; }

        [JsonProperty("frames", Required = Required.Always)]
        public List<SpriteLayerFrame> Frames { get; set; }
    }

    public partial class SpriteLayerFrame
    {
        [JsonProperty("posX", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? PosX { get; set; }

        [JsonProperty("posY", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? PosY { get; set; }

        [JsonProperty("scaleX", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double? ScaleX { get; set; }

        [JsonProperty("scaleY", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double? ScaleY { get; set; }

        [JsonProperty("spriteId", Required = Required.Always)]
        public string SpriteId { get; set; }

        [JsonProperty("time", Required = Required.Always)]
        public long Time { get; set; }
    }

    public partial class SpriteAtlas
    {
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("path", Required = Required.Always)]
        public string Path { get; set; }

        [JsonProperty("sprites", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<Sprite> Sprites { get; set; }
    }

    public partial class Sprite
    {
        [JsonProperty("height", Required = Required.Always)]
        public long Height { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("width", Required = Required.Always)]
        public long Width { get; set; }

        [JsonProperty("x", Required = Required.Always)]
        public long X { get; set; }

        [JsonProperty("y", Required = Required.Always)]
        public long Y { get; set; }
    }

    public partial class EventInfo
    {
        [JsonProperty("hasLocation", Required = Required.Always)]
        public bool HasLocation { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("params", Required = Required.Always)]
        public List<Param> Params { get; set; }
    }

    public partial class Param
    {
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("type", Required = Required.Always)]
        public TypeEnum Type { get; set; }
    }

    public partial class MetaPropInfo
    {
        [JsonProperty("default")]
        public object Default { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("type", Required = Required.Always)]
        public TypeEnum Type { get; set; }
    }

    public enum TypeEnum { Bool, BoolArray, BoolMap, Number, NumberArray, NumberMap, String, StringArray, StringMap };

    public partial class File
    {
        public static File FromJson(string json) => JsonConvert.DeserializeObject<File>(json, GaSpTK.Schema.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this File self) => JsonConvert.SerializeObject(self, GaSpTK.Schema.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                TypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "bool":
                    return TypeEnum.Bool;
                case "bool-array":
                    return TypeEnum.BoolArray;
                case "bool-map":
                    return TypeEnum.BoolMap;
                case "number":
                    return TypeEnum.Number;
                case "number-array":
                    return TypeEnum.NumberArray;
                case "number-map":
                    return TypeEnum.NumberMap;
                case "string":
                    return TypeEnum.String;
                case "string-array":
                    return TypeEnum.StringArray;
                case "string-map":
                    return TypeEnum.StringMap;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            switch (value)
            {
                case TypeEnum.Bool:
                    serializer.Serialize(writer, "bool");
                    return;
                case TypeEnum.BoolArray:
                    serializer.Serialize(writer, "bool-array");
                    return;
                case TypeEnum.BoolMap:
                    serializer.Serialize(writer, "bool-map");
                    return;
                case TypeEnum.Number:
                    serializer.Serialize(writer, "number");
                    return;
                case TypeEnum.NumberArray:
                    serializer.Serialize(writer, "number-array");
                    return;
                case TypeEnum.NumberMap:
                    serializer.Serialize(writer, "number-map");
                    return;
                case TypeEnum.String:
                    serializer.Serialize(writer, "string");
                    return;
                case TypeEnum.StringArray:
                    serializer.Serialize(writer, "string-array");
                    return;
                case TypeEnum.StringMap:
                    serializer.Serialize(writer, "string-map");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }
}
