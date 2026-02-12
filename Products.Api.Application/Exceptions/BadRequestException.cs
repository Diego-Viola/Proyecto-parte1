namespace Products.Api.Application.Exceptions;
public class BadRequestException : Exception
{
    public string Code { get; }

    public BadRequestException(string message, string code) : base(message)
    {
        Code = code;
    }
}
