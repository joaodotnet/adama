using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ApplicationCore.DTOs
{
    public class SageResponseDTO
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string ResponseBody { get; set; }
        

        public long? InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
