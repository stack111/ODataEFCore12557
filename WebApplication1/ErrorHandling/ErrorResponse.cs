namespace WebApplication1
{
    public class ErrorResponse
    {
        public Error Error { get; set; }

        /// <summary>
        /// Static factory that builds and returns an ErrorResponse object.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="code">Error code.</param>
        public static ErrorResponse Create(string message, string code = null)
        {
            var response = new ErrorResponse()
            {
                Error = new Error
                {
                    Message = message,
                    Code = code
                }
            };

            return response;
        }

        public static ErrorResponseBuilder Builder()
        {
            return new ErrorResponseBuilder();
        }
    }
}
