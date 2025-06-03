using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.Text;

namespace ApiJWT.Utils
{
    public class Common
    {
        /// <summary>
        /// String de conexão com o banco de dados.
        /// </summary>
        public static string? ConnDB = "";

        /// <summary>
        /// Caminho onde os logs serão salvos.
        /// </summary>
        public static string PathLog = "";

        /// <summary>
        /// Chave da API.
        /// </summary>
        public static string ApiKey = "";

        /// <summary>
        /// Chave da API.
        /// </summary>
        public static string Role = "";

        /// <summary>
        /// Contexto HTTP atual.
        /// </summary>
        public static HttpContext? Context = null;

        /// <summary>
        /// Enumeração para representar diferentes bancos de dados.
        /// </summary>
        public enum DataBase
        {
            /// <summary>
            /// Representa o banco de dados principal.
            /// </summary>
            ConnDB = 1
        }

        /// <summary>
        /// Cria uma conexão com o banco de dados especificado.
        /// </summary>
        /// <param name="db">Enumeração que define o banco de dados para a conexão.</param>
        /// <returns>Uma instância aberta de <see cref="SqlConnection"/>.</returns>
        public static SqlConnection GetConnection(DataBase db)
        {
            SqlConnection? retorno = null;
            switch (db)
            {
                case DataBase.ConnDB:
                    retorno = new SqlConnection(Common.ConnDB);
                    break;
                default:
                    retorno = new SqlConnection(Common.ConnDB);
                    break;
            }
            retorno.Open();
            return retorno;
        }

        /// <summary>
        /// Enumeração para identificar a interface responsável.
        /// </summary>
        public enum ResponsibleInterface
        {
            /// <summary>
            /// Representa a interface da API JWT.
            /// </summary>
            ApiJWT = 1
        }

        /// <summary>
        /// Enumeração para identificar o tipo de log.
        /// </summary>
        public enum TypeLog
        {
            /// <summary>
            /// Representa um log de sucesso.
            /// </summary>
            SUCCESS = 1,

            /// <summary>
            /// Representa um log de erro.
            /// </summary>
            ERROR = 2
        }

        /// <summary>
        /// Salva uma mensagem de log em um arquivo.
        /// </summary>
        /// <param name="_ResponsibleInterface">Interface responsável pelo log.</param>
        /// <param name="_TypeLog">Tipo do log (sucesso ou erro).</param>
        /// <param name="dscMessage">Mensagem a ser registrada no log.</param>
        /// <param name="WordWrap">Indica se deve adicionar uma linha em branco antes da mensagem.</param>
        public static void SaveFileLog(ResponsibleInterface _ResponsibleInterface, TypeLog _TypeLog, string dscMessage, bool WordWrap = false)
        {
            try
            {
                string? ficheiro = Common.PathLog + "\\" + (_TypeLog == TypeLog.SUCCESS ? "Success" : "Error") + "\\log_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
                string? path = Path.GetDirectoryName(ficheiro);

                if (path != null && !Directory.Exists(path))
                    Directory.CreateDirectory(path);

                StreamWriter file = new StreamWriter(ficheiro, true, Encoding.Default);

                if (WordWrap)
                { file.WriteLine(" "); }

                file.WriteLine(("[ApiJWT => LOG] - ") + DateTime.Now.ToString() + " : " + dscMessage);
                file.Dispose();
            }
            catch (Exception)
            { /**/ }
        }
    }
}
