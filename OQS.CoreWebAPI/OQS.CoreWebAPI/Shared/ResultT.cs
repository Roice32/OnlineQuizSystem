namespace OQS.CoreWebAPI.Shared;

public class Result<T> : Result
{
    private readonly T? _value;
    protected internal Result(T? value, bool isSuccess, Error error) : base(isSuccess, error)
        => _value = value;
        

    public T Value => IsSuccess ? _value! : default;
    public static implicit operator Result<T>(T? value) => Create(value);
}