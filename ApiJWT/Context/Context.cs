using ApiJWT.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ApiJWT
{
	/// <summary>
	/// Context
	/// </summary>
	public class Context
    {
		/// <summary>
		/// SetContext
		/// </summary>
		public static void SetContext(HttpContext httpContext)
        {
            // Alimenta o Context para ser utilizado no módulo Services
            Common.Context = httpContext;

            /* Coleta dados do Header */
            StringValues values = string.Empty;
        }
    }
}
