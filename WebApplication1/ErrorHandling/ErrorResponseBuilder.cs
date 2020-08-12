
namespace WebApplication1
{
    public class ErrorResponseBuilder : IErrorResponseBuilder
    {
        private readonly ErrorResponse errorResponse;

        public ErrorResponseBuilder()
        {
            errorResponse = new ErrorResponse()
            {
                Error = new Error()
            };
        }

        public ErrorResponseBuilder WithErrorCode(string errorCode)
        {
            errorResponse.Error.Code = errorCode;
            return this;
        }

        public ErrorResponseBuilder WithMessage(string message)
        {
            errorResponse.Error.Message = message;
            return this;
        }

        public ErrorResponseBuilder WithTarget(string target)
        {
            errorResponse.Error.Target = target;
            return this;
        }

        public ErrorResponseBuilder AddErrorDetail(string code, string message, string target)
        {
            errorResponse.Error.Details.Add(new ErrorDetail()
            {
                Code = code,
                Message = message,
                Target = target
            });
            return this;
        }

        public ErrorResponse Build()
        {
            return errorResponse;
        }
    }
}
