using CodeReviewService.Infra.Database.Repositorio;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Service
{
    public class RepositorioService
    {
        private readonly IRepositorioOperations repositorioOperations;
        private readonly ILogger<RepositorioService> logger;

        public RepositorioService(RepositorioOperations repositorioOperations, ILogger<RepositorioService> logger)
        {
            this.logger = logger;
            this.repositorioOperations = repositorioOperations;
        }

        public List<Models.CloneConfig> GetRepositoriesData()
        {
            return repositorioOperations.GetRepositoriesData();
        }
        public List<string> GetRepositoryBranchs(string repoName)
        {
            try
            {
                if (!repositorioOperations.RepositorioExist(repoName))
                    throw new Exception("ESSE REPOSITORIO NÃO EXISTE ->" + repoName);

                return repositorioOperations.GetRepositoryBranchs(repoName);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR :" + e.Message);
                logger.LogWarning("ERROR --> " + e.Message);
                return new();
            }
        }

        public bool RepositorioExist(string repoName) => repositorioOperations.RepositorioExist(repoName);
    }
}
