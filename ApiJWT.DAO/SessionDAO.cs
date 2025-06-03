using Dapper;
using ApiJWT.Core;
using System.Data.SqlClient;

namespace ApiJWT.DAO
{
	public class SessionDAO
	{
		#region CRUD
		#region Buscar
		public static async Task<List<Session>> Buscar(SqlConnection conn, Session.Filtro filtro)
		{
			List<Session> lista = (await conn.QueryAsync<Session>(@"
                    SELECT
						IdSessao,
						User,
						CreateDate,
						Roles,
						RefreshToken,
						DateExpirationRefreshToken
					FROM 
						Session
                    WHERE
                        (@IdSessao IS NULL OR IdSessao = @IdSessao) AND                        
                        (@RefreshToken IS NULL OR RefreshToken = @RefreshToken) 
                    ORDER BY 
						IdSessao DESC
                ", filtro)).ToList();

			return lista;
		}
		#endregion

		#region Inserir e Atualizar
		public static async Task<Session> Salvar(SqlConnection conn, Session registro, SqlTransaction trans)
		{
			if (registro.IdSessao == 0)
			{
				registro.IdSessao = (await conn.QueryAsync<int>(@"
                    INSERT INTO Session
                    (
						User,
						CreateDate,
						Roles,
						RefreshToken,
						DateExpirationRefreshToken
                    )
                    VALUES
                    (
						@User,
						@CreateDate,
						@Roles,
						@RefreshToken,
						@DateExpirationRefreshToken
                    );SELECT SCOPE_IDENTITY();
                ", registro, trans)).FirstOrDefault();
			}
			else
			{
				await conn.ExecuteAsync(@"
                        UPDATE 
                            Session
                        SET
							User = @User,
							CreateDate = @CreateDate,
							Roles = @Roles,
							RefreshToken = @RefreshToken
                        WHERE
							IdSessao = @IdSessao", registro, trans);
			}
			return registro;
		}

		public static async Task InserirLista(SqlConnection conn, List<Session> registros, SqlTransaction trans)
		{
			await conn.ExecuteAsync(@"
                    INSERT INTO Session
                    (
						User,
						CreateDate,
						Roles,
						RefreshToken,
						DateExpirationRefreshToken
                    )
                    VALUES
                    (
						@User,
						@CreateDate,
						@Roles,
						@RefreshToken,
						@DateExpirationRefreshToken
                    );
                ", registros, trans);
		}

		public static async Task AtualizarLista(SqlConnection conn, List<Session> registros, SqlTransaction trans)
		{

			await conn.ExecuteAsync(@"
                    UPDATE 
                        Session
                    SET
						User = @User,
						CreateDate = @CreateDate,
						Roles = @Roles,
						RefreshToken = @RefreshToken,
						DateExpirationRefreshToken = @DateExpirationRefreshToken
                    WHERE
						IdSessao = @IdSessao", registros, trans);

		}
		#endregion

		#region Excluir
		public static async Task<bool> Excluir(SqlConnection conn, int id, SqlTransaction trans)
		{
			return (await conn.ExecuteAsync(@"
                    DELETE 
                        Session 
                    WHERE 
                        (@IdSessao IS NULL OR IdSessao = @IdSessao)", new { IdSessao = id }, trans)) > 0;
		}
		#endregion

		#endregion
	}
}
