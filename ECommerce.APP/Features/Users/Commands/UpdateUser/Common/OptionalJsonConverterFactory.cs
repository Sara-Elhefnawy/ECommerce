using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECommerce.APP.Features.Users.Commands.UpdateUser.Common;

// Optional<T> is generic, so a single JsonConverter<Optional<T>> can't
// exist at compile time for every T. A factory builds the right closed
// converter (e.g. OptionalConverter<string>) at runtime for whatever T
// System.Text.Json encounters.
public sealed class OptionalJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var innerType = typeToConvert.GetGenericArguments()[0]; // e.g. string
        var converterType = typeof(OptionalConverter<>).MakeGenericType(innerType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}
