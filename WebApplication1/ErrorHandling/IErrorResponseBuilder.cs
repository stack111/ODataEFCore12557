
namespace WebApplication1
{
    public interface IErrorResponseBuilder
    {
        ErrorResponseBuilder AddErrorDetail(string code, string message, string target);
        ErrorResponse Build();
        ErrorResponseBuilder WithErrorCode(string errorCode);
        ErrorResponseBuilder WithMessage(string message);
        ErrorResponseBuilder WithTarget(string target);
    }
}
