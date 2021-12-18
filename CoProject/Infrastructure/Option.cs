using System.Diagnostics.CodeAnalysis;

namespace CoProject.Infrastructure;

[ExcludeFromCodeCoverage]
public struct Option<T> where T : class
{
    private readonly T? _value;

    public T Value => _value ?? throw new InvalidOperationException();

    public bool IsNone => _value == null;

    public bool IsSome => _value != null;

    public Option(T? value)
    {
        _value = value;
    }

    public static implicit operator T(Option<T> option)
    {
        return option.Value;
    }

    public static implicit operator Option<T>(T? value)
    {
        return new(value);
    }
}