using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Infra.Database.Branch
{
    public class BranchOperations : IBranchOperations
    {
        private readonly string connString;
        private readonly ILogger<BranchOperations> logger;

        public BranchOperations(ILogger<BranchOperations> logger)
        {
            this.logger = logger;
            connString = Criptografia.Decrypt(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
        }

        public int GetBranchId(string nmBranch, string repoName)
        {
            int result = -1;//-1 significa que não achou nenhum id
            using SqlConnection conn = new(connString);

            SqlParameter pBranchName = CreateParam("@branchName", nmBranch);
            SqlParameter pRepoName = CreateParam("@repoName", repoName);

            string cmd = @"SELECT b.Id_Branch FROM tbBranch b (nolock)
	                            JOIN tbRepositorio r (nolock)
                            ON b.Id_repositorio = r.Id_repositorio
                            WHERE b.Nm_branch = @branchName and r.Nm_repositorio = @repoName";

            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pRepoName);
            command.Parameters.Add(pBranchName);

            try
            {
                conn.Open();
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result = (int)reader[0];
                }
            }
            catch (Exception e)
            {
                logger.LogWarning("ERRO AO PEGAR ID DA BRANCH" + nmBranch + "\n" + e.Message);
                Console.WriteLine("ERRO AO PEGAR ID DA BRANCH" + nmBranch + "\n" + e.Message);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public (string, string) GetBranchEmailsAdress(string nmBranch, string repoName)
        {
            (string, string) result = (null, null);
            using SqlConnection conn = new(connString);

            SqlParameter pBranchName = CreateParam("@branchName", nmBranch);
            SqlParameter pRepoName = CreateParam("@repoName", repoName);

            string cmd = @"SELECT b.Nm_email_dev, b.Nm_email_review FROM tbBranch b (nolock)
	                            JOIN tbRepositorio r (nolock)
                            ON b.Id_repositorio = r.Id_repositorio
                            WHERE b.Nm_branch = @branchName and r.Nm_repositorio = @repoName";

            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pRepoName);
            command.Parameters.Add(pBranchName);

            try
            {
                conn.Open();
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result.Item1 = (string)reader[0];
                    result.Item2 = (string)reader[1];
                }
            }
            catch (Exception e)
            {
                logger.LogWarning("ERRO AO PEGAR EMAILS DO DEV OU RESPONSAVEL PELA REVISÃO\n" + e.Message);
                Console.WriteLine("ERRO AO PEGAR EMAILS DO DEV OU RESPONSAVEL PELA REVISÃO\n" + e.Message);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public bool BranchExist(string nmBranch)
        {
            using SqlConnection conn = new(connString);
            var pNmBranch = CreateParam("@nmBranch", nmBranch);

            string cmd = @"select Id_branch from tbBranch where Nm_branch = @nmBranch";
            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pNmBranch);

            try
            {
                conn.Open();
                using SqlDataReader reader = command.ExecuteReader();
                int data = -1;
                while (reader.Read())
                {
                    data = (int)reader[0];
                }

                if (data != -1)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                logger.LogWarning("ERRO EM BRANCH EXIST : " + e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public bool BranchExist(int idBranch)
        {
            using SqlConnection conn = new(connString);
            var pIdBranch = CreateParam("@idBranch", idBranch);

            string cmd = @"select Id_branch from tbBranch where Id_branch = @idBranch";
            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pIdBranch);

            try
            {
                conn.Open();
                using SqlDataReader reader = command.ExecuteReader();
                int data = -1;
                while (reader.Read())
                {
                    data = (int)reader[0];
                }

                if (data != -1)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                logger.LogWarning("ERRO EM BRANCH EXIST : " + e.Message);
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
