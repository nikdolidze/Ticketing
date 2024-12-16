using System.Text.Json.Serialization;

[JsonSerializable(typeof(string))]
public partial class MyJsonContext : JsonSerializerContext
{
}