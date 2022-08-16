using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Infra.Database.Commit
{
    public class CommitOperations : ICommitOperations
    {
        private readonly string connString;
        private readonly ILogger<CommitOperations> logger;

        public CommitOperations(ILogger<CommitOperations> logger)
        {
            this.logger = logger;
            connString = Criptografia.Decrypt(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
        }

        public (DateTime, string) GetLastCommitDateAndId(string branchName, string repoName)
        {
            (DateTime, string) result = (DateTime.Now, null);

            using SqlConnection conn = new(connString);

            SqlParameter pBranchName = CreateParam("@branchName", branchName);
            SqlParameter pRepoName = CreateParam("@repoName", repoName);

            string cmd = @"SELECT c.Dt_commit, c.Id_Commit FROM tbCommit c (nolock)
	                        JOIN tbBranch b (nolock)
                        ON b.Id_branch = c.Id_branch
                            join tbRepositorio r (nolock)
                        ON r.Id_repositorio = b.Id_repositorio
	                        WHERE b.Nm_branch = @branchName and r.Nm_repositorio = @repoName";

            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pBranchName);
            command.Parameters.Add(pRepoName);

            try
            {
                conn.Open();
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result.Item1 = reader.GetDateTime(0);
                    result.Item2 = reader.GetString(1);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("ERRO AO PEGAR DATA ULTIMO COMMIT DA BRANCH: " + branchName + "\n" + e.Message);
                logger.LogWarning("ERRO AO PEGAR DATA ULTIMO COMMIT DA BRANCH: " + branchName + "\n" + e.Message);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public void InsertLastCommit(Models.Commit commit, int idBranch)
        {
            string connString = Criptografia.Decrypt(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            using SqlConnection conn = new(connString);

            SqlParameter pIdCommit = CreateParam("@idCommit", commit.IdCommit);
            SqlParameter pIdBranch = CreateParam("@idBranch", idBranch);
            SqlParameter pNm_mensagem = CreateParam("@nmMensagem", commit.Nm_mensagem);
            SqlParameter pNm_autor = CreateParam("@nmAutor", commit.Nm_autor);
            SqlParameter pDt_commit = CreateParam("@dtCommit", commit.Dt_commit);

            string cmd = @"INSERT INTO tbCommit VALUES (@idCommit, @idBranch, @nmMensagem, @nmAutor, @dtCommit)";

            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pIdCommit);
            command.Parameters.Add(pIdBranch);
            command.Parameters.Add(pNm_mensagem);
            command.Parameters.Add(pNm_autor);
            command.Parameters.Add(pDt_commit);

            try
            {
                conn.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.LogWarning("ERRO AO INSERIR UM NOVO ULTIMO COMMIT\n" + e.Message);
                Console.WriteLine("ERRO AO INSERIR UM NOVO ULTIMO COMMIT\n" + e.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public void UpdateLastCommit(Models.Commit commit, string oldIdCommit)
        {
           using SqlConnection conn = new(connString);

            SqlParameter pIdCommit = CreateParam("@idCommit", commit.IdCommit);
            SqlParameter pNm_mensagem = CreateParam("@nmMensagem", commit.Nm_mensagem);
            SqlParameter pNm_autor = CreateParam("@nmAutor",commit.Nm_autor);
            SqlParameter pDt_commit = CreateParam("@dtCommit", commit.Dt_commit);
            SqlParameter pId_Commit_old = CreateParam("@idCommitOld", oldIdCommit);

            string cmd = @"UPDATE t
	                        SET
	                        t.Dt_commit = @dtCommit,
	                        t.Id_Commit = @idCommit,
	                        t.Nm_autor = @nmAutor,
	                        t.Nm_mensagem = @nmMensagem

	                        FROM tbCommit t
	                        WHERE t.Id_Commit = @idCommitOld";

            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pIdCommit);
            command.Parameters.Add(pNm_mensagem);
            command.Parameters.Add(pNm_autor);
            command.Parameters.Add(pDt_commit);
            command.Parameters.Add(pId_Commit_old);

            try
            {
                conn.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.LogWarning("ERRO AO DAR UPDATE NO ULTIMO COMMIT --> " + oldIdCommit + "\n" + e.Message);
                Console.WriteLine("ERRO AO DAR UPDATE NO ULTIMO COMMIT --> " + oldIdCommit + "\n" + e.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public bool CommitExist(string idCommit)
        {
            using SqlConnection conn = new(connString);
            var pIdCommit = CreateParam("@idCommit", idCommit);

            string cmd = @"select Id_Commit from tbCommit where Id_Commit = @idCommit";
            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pIdCommit);

            try
            {
                conn.Open();
                using SqlDataReader reader = command.ExecuteReader();
                string data = null;
                while (reader.Read())
                {
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
                logger.LogWarning("ERRO EM COMMIT EXIST :" + e.Message);
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
