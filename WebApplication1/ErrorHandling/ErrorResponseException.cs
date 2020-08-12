namespace WebApplication1
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Runtime.Serialization;

    [Serializable]
    public class ErrorResponseException : Exception
    {
        public ErrorResponseException()
        {
        }

        public ErrorResponseException(string message)
            : base(message)
        {
        }

        public ErrorResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ErrorResponseException(HttpStatusCode statusCode, ErrorResponse errorResponse)
        {
            this.StatusCode = statusCode;
            this.ErrorResponse = errorResponse;
        }

        public HttpStatusCode StatusCode { get; set; }

        public ErrorResponse ErrorResponse { get; set; }

        [ExcludeFromCodeCoverage]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new System.ArgumentNullException(nameof(info));
            }

            info.AddValue("statusCode", this.StatusCode);
            info.AddValue("errorResponse", this.ErrorResponse);
            base.GetObjectData(info, context);
        }

        public static ErrorResponseExceptionBuilder Builder()
        {
            return new ErrorResponseExceptionBuilder();
        }
    }
}