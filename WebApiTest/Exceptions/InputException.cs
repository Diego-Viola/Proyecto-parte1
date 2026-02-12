namespace WebApiTest.Exceptions;

public class InputException : Exception
{
    public IDictionary<string, string[]> Errors { get; }
    public InputException(IDictionary<string, string[]> errors)
        : base("Input validation failed")
    {
        Errors = errors;
    }
}
