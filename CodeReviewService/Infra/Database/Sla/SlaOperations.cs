using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Infra.Database.Sla
{
    public class SlaOperations : ISlaOperations
    {
        private readonly string connString;
        private readonly ILogger<SlaOperations> logger;

        public SlaOperations(ILogger<SlaOperations> logger)
        {
            this.logger = logger;
            connString = Criptografia.Decrypt(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
        }

        public DateTime GetSlaCommitDate(string repoName)
        {
            DateTime result = DateTime.Now;// se não achar resultado retorna a data mais recente possivel
            using SqlConnection conn = new(connString);

            SqlParameter pRepoName = CreateParam("@repoName", repoName);

            string cmd = @"SELECT s.Nr_dias_sla_commit FROM tbSLA s (nolock)
	                            JOIN tbRepositorio r (nolock)
                            ON s.id_repositorio = r.Id_repositorio
	                            WHERE r.Nm_repositorio = @repoName";

            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pRepoName);

            try
            {
                conn.Open();
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int dayLimit = reader.GetInt32(0);
                    result = DateTime.Now.AddDays(-dayLimit);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERRO AO PEGAR DATA SLA COMMIT DO REPOSITORIO: " + repoName + "\n" + e.Message);
                logger.LogWarning("ERRO AO PEGAR DATA SLA COMMIT DO REPOSITORIO: " + repoName + "\n" + e.Message);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public DateTime GetSlaReviewDate(string repoName)
        {
            DateTime result = DateTime.Now;// se não achar resultado retorna a data mais recente possivel
            using SqlConnection conn = new(connString);

            SqlParameter pRepoName = CreateParam("@repoName", repoName);

            string cmd = @"SELECT s.Nr_dias_sla_review FROM tbSLA s (nolock)
	                            JOIN tbRepositorio r (nolock)
                            ON s.id_repositorio = r.Id_repositorio
	                            WHERE r.Nm_repositorio = @repoName";

            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pRepoName);

            try
            {
                conn.Open();
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int dayLimit = reader.GetInt32(0);
                    result = DateTime.Now.AddDays(-dayLimit);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERRO AO PEGAR DATA SLA COMMIT DO REPOSITORIO: " + repoName + "\n" + e.Message);
                logger.LogWarning("ERRO AO PEGAR DATA SLA COMMIT DO REPOSITORIO: " + repoName + "\n" + e.Message);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public bool SlaExist(string repoName)
        {
            using SqlConnection conn = new(connString);
            var pRepoName = CreateParam("@repoName", repoName);

            string cmd = @"select s.Id_SLA from tbSLA s
	                        join tbRepositorio r
                        on s.Id_repositorio = r.Id_repositorio
                        where r.Nm_repositorio = @repoName";
            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pRepoName);

            try
            {
                conn.Open();
                using SqlDataReader reader = command.ExecuteReader();
                string data = null;
                while (reader.Read())
                {
                    if(reader[0] is not DBNull)
                        data = (string)reader[0];
                }

                if (!data.Equals(null))
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                logger.LogWarning("ERROR SLA EXIST : " + e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        private SqlParameter CreateParam(string name, object value)
        {
            return new(name, value);
        }
    }
}
