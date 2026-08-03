namespace ECommerce.APP.Features.Users.Commands.UpdateUser.Common;

// Distinguishes 3 states a JSON field can be in:
//   - key missing entirely from JSON  → IsSet = false, Value = default
//   - key present with value null     → IsSet = true,  Value = null
//   - key present with a real value   → IsSet = true,  Value = "something"
// A plain `string?` can only ever represent 2 of these 3 states.
public readonly struct Optional<T>
{
    public bool IsSet { get; }
    public T? Value { get; }

    private Optional(bool isSet, T? value)
    {
        IsSet = isSet;
        Value = value;
    }

    public static Optional<T> Unset() => new(false, default);
    public static Optional<T> Set(T? value) => new(true, value);
}
