using CodeReviewService.Infra.Database.Sla;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Service
{
    public class SlaService
    {
        private readonly ISlaOperations slaOperations;
        private readonly RepositorioService repositorioService;
        private readonly ILogger<SlaService> logger;

        public SlaService(RepositorioService repositorio, SlaOperations slaOperations, ILogger<SlaService> logger)
        {
            this.logger = logger;
            this.slaOperations = slaOperations;
            repositorioService = repositorio;
        }

        public DateTime GetSlaCommitDate(string repoName)
        {
            try
            {
                if (!repositorioService.RepositorioExist(repoName))
                    throw new Exception("REPOSITORIO NÃO ENCONTRADO -->" + repoName);

                return slaOperations.GetSlaCommitDate(repoName);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : " + e.Message);
                logger.LogWarning("ERROR --> " + e.Message);
                return DateTime.Now;
            }
        }
    }
}
