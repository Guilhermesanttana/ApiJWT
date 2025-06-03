using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJWT.Core
{
	public class TokenJWT
	{
		public class LoginModel
		{
			public string? User { get; set; }
			public string[]? Roles { get; set; }
			public string? ApiKey { get; set; }
			public string? RefreshToken { get; set; }
		}

		public class ReturnToken
		{
			public string? Token { get; set; }
			public string? Type { get; set; }
			public DateTime ExpirationDate { get; set; }
			public string? RefreshToken { get; set; }
		}
	}
}
