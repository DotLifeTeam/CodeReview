using CodeReviewService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace CodeReviewService.Infra.Database.Repositorio
{
    public class RepositorioOperations : IRepositorioOperations
    {
        private readonly string connString;
        private readonly ILogger<RepositorioOperations> logger;

        public RepositorioOperations(ILogger<RepositorioOperations> logger)
        {
            this.logger = logger;
            connString = Criptografia.Decrypt(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
        }

        public List<CloneConfig> GetRepositoriesData()
        {
            List<CloneConfig> result = new();

            using SqlConnection conn = new(connString);

            string cmd = @"SELECT Nm_repositorio, Nm_url_clone, Nm_usuario, Nm_senha FROM tbRepositorio (nolock)";
            using SqlCommand command = new(cmd, conn);

            try
            {
                conn.Open();
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    CloneConfig config = new();
                    config.RepoName = (string)reader[0];
                    config.Url = (string)reader[1];
                    config.Username = (string)reader[2];
                    config.Password = (string)reader[3];

                    result.Add(config);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("ERRO AO PEGAR REPOSITORIOS\n" + e.Message);
                logger.LogWarning("ERRO AO PEGAR REPOSITORIOS\n" + e.Message);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public List<string> GetRepositoryBranchs(string repoName)
        {
            List<string> branchs = new();
            using SqlConnection conn = new(connString);

            SqlParameter pRepoName = CreateParam("@repoName", repoName);

            string cmd = @"SELECT b.Nm_branch FROM tbBranch b (nolock)
	                            JOIN tbRepositorio r (nolock)
                            ON b.Id_repositorio = r.Id_repositorio
	                            WHERE r.Nm_repositorio = @repoName";

            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pRepoName);

            try
            {
                conn.Open();
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    branchs.Add(reader.GetString(0));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("ERRO AO PEGAR BRANCHS PARA ANALISE DO REPOSITORIO: " + repoName + "\n" + e.Message);
                logger.LogWarning("ERRO AO PEGAR BRANCHS PARA ANALISE DO REPOSITORIO: " + repoName + "\n" + e.Message);
            }
            finally
            {
                conn.Close();
            }

            return branchs;
        }

        public bool RepositorioExist(string repoName)
        {
            using SqlConnection conn = new(connString);
            var pRepoName = CreateParam("@repoName", repoName);

            string cmd = @"select Id_repositorio from tbRepositorio (nolock) where Nm_repositorio = @repoName";
            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pRepoName);

            try
            {

                int data = -1;
                conn.Open();

                using SqlDataReader reader = command.ExecuteReader();
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
                logger.LogWarning("ERRO IN REPOSITORY EXIST :" + e.Message);
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
