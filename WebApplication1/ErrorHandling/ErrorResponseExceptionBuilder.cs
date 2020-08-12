
namespace WebApplication1
{
    using System.Net;

    public class ErrorResponseExceptionBuilder
    {
        private readonly ErrorResponseBuilder builder;
        private HttpStatusCode statusCode;

        public ErrorResponseExceptionBuilder()
        {
            builder = ErrorResponse.Builder();
        }

        public ErrorResponseExceptionBuilder WithStatusCode(HttpStatusCode statusCode)
        {
            this.statusCode = statusCode;
            return this;
        }

        public ErrorResponseExceptionBuilder WithBadRequest()
        {
            return WithStatusCode(HttpStatusCode.BadRequest);
        }

        public ErrorResponseExceptionBuilder WithTarget(string target)
        {
            builder.WithTarget(target);
            return this;
        }

        public ErrorResponseExceptionBuilder WithMessage(string message)
        {
            builder.WithMessage(message);
            return this;
        }

        public ErrorResponseExceptionBuilder WithErrorCode(string errorCode)
        {
            builder.WithErrorCode(errorCode);
            return this;
        }

        public ErrorResponseExceptionBuilder WithErrorDetail(string errorCode, string errorMessage, string target)
        {
            builder.AddErrorDetail(errorCode, errorMessage, target);
            return this;
        }

        public ErrorResponseException Result()
        {
            return new ErrorResponseException(statusCode, builder.Build());
        }
    }
}