using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJWT.Core
{
	public class Session
	{
		public int IdSessao { get; set; }
		public string? User { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime DateRefreshTokenExpiration { get; set; }
		public string? Roles { get; set; }
		public string? RefreshToken { get; set; }
		public bool SessionValid { get; set; }

		public class Filtro
		{
			public int? IdSessao { get; set; }
			public string? RefreshToken { get; set; }
			public string? User { get; set; }
		}

	}
}
