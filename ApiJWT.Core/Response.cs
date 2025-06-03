using System;

namespace ApiJWT.Core
{
    public class Response
    {
		public bool Success { get; set; }
		public int Code { get; set; }
		public string? Msg { get; set; }
		public string? Error_description { get; set; }
		public Object? Retorno { get; set; }
	}
}
