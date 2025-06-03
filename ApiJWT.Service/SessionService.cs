using ApiJWT.Core;
using ApiJWT.DAO;
using ApiJWT.Utils;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiJWT.Service
{
    /// <summary>
    /// Serviço para manipulação de sessões.
    /// </summary>
    public class SessionService
	{
		SqlConnection connDB = Common.GetConnection(Common.DataBase.ConnDB);
		SqlTransaction? trans;

		/// <summary>
		/// Busca sessões com base no filtro fornecido.
		/// </summary>
		/// <param name="filtro">Filtro para busca de sessões.</param>
		/// <returns>Resposta contendo a lista de sessões encontradas.</returns>
		public static async Task<Response> GetSession(Session.Filtro filtro)
		{
			Response _response = new Response();
			try
			{
				using (SqlConnection connDB = Common.GetConnection(Common.DataBase.ConnDB))
				{
					IEnumerable<Session> listaBanco = await SessionDAO.Buscar(connDB, filtro);

					_response.Retorno = listaBanco;
					_response.Success = true;
					_response.Code = 200;
					_response.Msg = "Sucesso";
				}
			}
			catch (Exception e)
			{
				_response.Success = false;
				_response.Code = 500;
				_response.Msg = "Falha na requisição por EXCEPTION! Verifique e tente novamente.";
				_response.Error_description = e.Message;

				return _response;
			}
			return _response;
		}

		/// <summary>
		/// Insere uma nova sessão no banco de dados.
		/// </summary>
		/// <param name="registro">Dados da sessão a ser inserida.</param>
		/// <returns>Resposta indicando o sucesso ou falha da inserção.</returns>
		public async Task<Response> Insert(Session registro)
		{
			Response _response = new Response();
			try
			{
				trans = connDB.BeginTransaction();
				
				string apiKey = Common.ApiKey;
				var criptografiaService = new CriptografiaService(apiKey);

				registro.CreateDate = DateTime.Now;
				registro.User = criptografiaService.Criptografar(registro.User!);
				registro.Roles = criptografiaService.Criptografar(registro.Roles!);

				await SessionDAO.Salvar(connDB, registro, trans);

				trans.Commit();

				_response.Retorno = registro;
				_response.Success = true;
				_response.Code = 200;
				_response.Msg = "Sucesso";

			}
			catch (Exception e)
			{
				trans?.Rollback();

				_response.Success = false;
				_response.Code = 500;
				_response.Msg = "Falha na requisição por EXCEPTION! Verifique e tente novamente.";
				_response.Error_description = e.Message;

				return _response;
			}
			return _response;
		}

		/// <summary>
		/// Valida um refresh token e retorna a sessão correspondente.
		/// </summary>
		/// <param name="refreshToken">Refresh token a ser validado.</param>
		/// <returns>Sessão correspondente ao refresh token, indicando se é válida ou não.</returns>
		public async Task<Session> ValidateRefreshToken(string refreshToken)
		{
			Session? ultimasessao = (await SessionDAO.Buscar(connDB, new Session.Filtro() { RefreshToken = refreshToken })).FirstOrDefault();

			if (ultimasessao != null && ultimasessao.DateRefreshTokenExpiration >= DateTime.UtcNow)
			{
				string apiKey = Common.ApiKey;
				var criptografiaService = new CriptografiaService(apiKey);

				ultimasessao.SessionValid = true;
				ultimasessao.User = criptografiaService.Descriptografar(ultimasessao.User!);
				ultimasessao.Roles = criptografiaService.Descriptografar(ultimasessao.Roles!);
			}
			else
			{
				ultimasessao = new Session() { SessionValid = false };
			}

			return ultimasessao;
		}


		/// <summary>
		/// Valida um token
		/// </summary>
		/// <param name="token">Token a ser validado.</param>
		/// <returns>Sessão correspondente ao refresh token, indicando se é válida ou não.</returns>
		public string ValidateToken(string token)
		{
			if (string.IsNullOrEmpty(token))
				return string.Empty;

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(Common.ApiKey);

			try
			{
				var parameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false,
					ClockSkew = TimeSpan.Zero
				};

				var principal = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);
				var userId = principal.FindFirst(ClaimTypes.Name)?.Value;

				return userId ?? string.Empty;
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}
