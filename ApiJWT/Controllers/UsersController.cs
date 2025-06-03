using ApiJWT.Core;
using ApiJWT.Core.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiJWT.Controllers
{
    /// <summary>
	/// UsersController
	/// </summary>
	[Route("api/Users")]
    public class UsersController : Controller
    {
        /// <summary>
		/// Busca em uma lista de nomes prédifinidos os nomes corresponderem ao filtro
		/// </summary>
		/// <param name="filter"></param>
		/// <returns>A lista de Nomes que tiveram correspondência com o filtro</returns>
		/// <response code="200">A lista de Nomes que tiveram correspondência com o filtro</response>
		/// <response code="400">Se houve alguma falha de paramêtro ou regra de negócio</response> 
		/// <response code="500">Se houve alguma falha sistemica causada por Exception</response> 
		[HttpGet("FindUsers")]
        [Authorize(Roles = "ROLE000001")]
        public async Task<IActionResult> FindUsers(FilterUsers filter)
        {
            Context.SetContext(HttpContext);

            // Arrange  
            var names = new List<string>
            {
                   "Alice", "Bob", "Charlie", "David", "Eve",
                   "Frank", "Grace", "Hannah", "Ivy", "Jack"
            };            

            // Act  
            var result = await Task.Run(async () =>
            {
                await Task.Delay(1000); // Simulate 1-second delay  

                if (string.IsNullOrEmpty(filter.UserName))
                    return names;
                else
                    return names.Where(name => name.Contains(filter.UserName, System.StringComparison.OrdinalIgnoreCase)).ToList();
            });

            // Assert
            Response _resp = new Response()
            {
                Retorno = result,
                Success = true,
                Code = 200,
                Msg = "Sucesso"
            };   
            
            return StatusCode(_resp.Code, _resp);
        }
    }
}
