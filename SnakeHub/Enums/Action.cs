using System.Text.Json.Serialization;

namespace SnakeHub.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Action
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
}
