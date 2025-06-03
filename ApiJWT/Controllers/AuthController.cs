using ApiJWT.Core;
using ApiJWT.Service;
using ApiJWT.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static ApiJWT.Core.TokenJWT;

namespace ApiJWT.Controllers
{
    /// <summary>
    /// AuthController
    /// </summary>
    [Route("api/AuthController")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Construtor
        /// </summary>
        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Autenticação da aplicação, aqui você vai autenticar na nossa ApiJWT 
        /// </summary>
        /// <remarks>
        /// Example
        /// 
        /// ```json
        /// 
        ///     POST api/AuthController/Auth
        ///     {
        ///         "User": "Guilherme",
        ///         "Roles":["ROLE000001"],
        ///         "ApiKey": "AF37D202-33FF-4BD1-B26F-6C8F9F15A31F"
        ///     }
        /// ```
        /// </remarks>
        /// <param name="login"></param>
        /// <returns>O sucesso da autenticação</returns>
        /// <response code="200">Returna o Token, Tipo, RefreshToken, ExpirationDate</response>
        /// <response code="400">Se houve alguma falha de paramêtro ou regra de negócio</response> 
        /// <response code="401">Se a autenticação falhar</response> 
        /// <response code="500">Se houve alguma falha sistemica causada por Exception</response> 
        [HttpPost("Auth")]
        public async Task<IActionResult> Auth([FromBody] LoginModel login)
        {
            if (login.ApiKey == Common.ApiKey && login.Roles?.Count() > 0)
            {
                var token = GenerateJwtToken(login);

                //Ative essa parte se quiser salvar a sessão no banco de dados
                //SessionService sessaoService = new SessionService();
                //await sessaoService.Insert(new Session()
                //{
                //    RefreshToken = token.RefreshToken,
                //    Roles = string.Join(",", login.Roles),
                //    User = login.User,
                //    DateRefreshTokenExpiration = token.ExpirationDate.AddMinutes(5)
                //});

                return Ok(new
                {
                    token = token.Token,
                    type = "Bearer",
                    expirationDate = token.ExpirationDate,
                    refreshToken = token.RefreshToken
                });
            }
            return Unauthorized("Autenticação Inválida!");
        }

        /// <summary>
        /// Autenticação do Refresh Token       
        /// Para esse end-point funcionar é necessário criar o banco de dados e as tabelas de sessão
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>O sucesso da autenticação do refresh token</returns>
        /// <response code="200">Returna o Token, Tipo, RefreshToken, ExpirationDate</response>
        /// <response code="400">Se houve alguma falha de paramêtro ou regra de negócio</response> 
        /// <response code="401">Se a autenticação falhar</response> 
        /// <response code="500">Se houve alguma falha sistemica causada por Exception</response> 
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            SessionService sessaoService = new SessionService();
            Session? ultimaSessao = await sessaoService.ValidateRefreshToken(refreshToken);

            if (ultimaSessao.SessionValid)
            {
                var newToken = GenerateJwtToken(new LoginModel() { User = ultimaSessao.User!, Roles = ultimaSessao.Roles!.Split(','), ApiKey = Common.ApiKey });

                await sessaoService.Insert(new Session()
                {
                    RefreshToken = newToken.RefreshToken,
                    Roles = ultimaSessao.Roles,
                    User = ultimaSessao.User,
                    DateRefreshTokenExpiration = newToken.ExpirationDate.AddMinutes(5)
                });

                return Ok(new
                {
                    token = newToken.Token,
                    type = "Bearer",
                    expirationDate = newToken.ExpirationDate,
                    refreshToken = newToken.RefreshToken
                });
            }
            else
            {
                return Unauthorized("Autenticação Inválida!");
            }
        }

        private ReturnToken GenerateJwtToken(LoginModel login)
        {
            var key = Encoding.ASCII.GetBytes(Common.ApiKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var refreshToken = GenerateRefreshToken();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, login.User!),
                }),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            tokenDescriptor.Subject.AddClaims(login.Roles!.Select(role => new Claim(ClaimTypes.Role, role)));
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new ReturnToken
            {
                Token = tokenHandler.WriteToken(token),
                ExpirationDate = token.ValidTo,
                RefreshToken = refreshToken
            };
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
