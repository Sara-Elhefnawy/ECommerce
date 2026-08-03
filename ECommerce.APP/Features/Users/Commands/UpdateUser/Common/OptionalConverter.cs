using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECommerce.APP.Features.Users.Commands.UpdateUser.Common;

public sealed class OptionalConverter<T> : JsonConverter<Optional<T>>
{
    // This Read method is ONLY invoked when the JSON key IS present —
    // System.Text.Json never calls Read for a missing key. That's why
    // "key missing" doesn't need handling here at all: it's covered by
    // Optional<T>'s own struct default (IsSet = false), used automatically
    // when the constructor parameter has no corresponding JSON property.
    public override Optional<T> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        // key present, value is JSON null → client wants to CLEAR the field
        if (reader.TokenType == JsonTokenType.Null)
            return Optional<T>.Set(default);

        // key present, real value → deserialize it as T and wrap it
        var value = JsonSerializer.Deserialize<T>(ref reader, options);
        return Optional<T>.Set(value);
    }

    // Only relevant if you ever serialize Optional<T> back OUT (e.g. in a
    // response DTO). For an incoming command like UpdateUserCommand this
    // never actually runs, but JsonConverter<T> requires implementing it.
    public override void Write(
        Utf8JsonWriter writer,
        Optional<T> value,
        JsonSerializerOptions options)
    {
        if (!value.IsSet || value.Value is null)
            writer.WriteNullValue();
        else
            JsonSerializer.Serialize(writer, value.Value, options);
    }
}
