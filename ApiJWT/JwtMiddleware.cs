using ApiJWT.Service;

namespace ApiJWT
{
    /// <summary>
    /// Middleware para validar o JWT e injetar o usuário no contexto HTTP.
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="JwtMiddleware"/>.
        /// </summary>
        /// <param name="next">Delegate para o próximo middleware no pipeline.</param>
        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
            SessionService serviceSessao = new SessionService();
        }

        /// <summary>
        /// Método invocado para processar a solicitação HTTP.
        /// </summary>
        /// <param name="context">O contexto HTTP atual.</param>
        /// <param name="jwtService">Serviço para validação de tokens JWT.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        public async Task Invoke(HttpContext context, SessionService jwtService)
        {
            // Obtém o token JWT do cookie da solicitação.
            var token = context.Request.Cookies["jwt"];

            if (!string.IsNullOrEmpty(token))
            {
                // Valida o token e obtém o ID do usuário associado.
                var userId = jwtService.ValidateToken(token);
                if (userId != null)
                {
                    // Injeta o ID do usuário no contexto HTTP.
                    context.Items["User"] = userId;
                }
            }

            // Chama o próximo middleware no pipeline.
            await _next(context);
        }
    }
}
