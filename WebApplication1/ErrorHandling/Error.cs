

namespace WebApplication1
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Type is not intended to be shared with other languages.")]
    public class Error
    {
        public Error()
        {
            Details = new List<ErrorDetail>();
        }

        public string Code { get; set; }

        public string Message { get; set; }

        public string Target { get; set; }

        public List<ErrorDetail> Details { get; private set; }
    }
}
