using CodeReviewService.Infra.Database.Branch;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Service
{
    public class BranchService
    {
        private readonly IBranchOperations branchOperations;
        private readonly RepositorioService repositorioService;
        private readonly ILogger<BranchService> logger;

        public BranchService(RepositorioService repositorio, BranchOperations branchOperations, ILogger<BranchService> logger)
        {
            this.logger = logger;
            this.branchOperations = branchOperations;
            repositorioService = repositorio;
        }

        public int GetBranchId(string nmBranch, string repoName)
        {
            try
            {
                if (nmBranch.Contains("origin/"))
                    nmBranch = nmBranch.Remove(0, 7);

                if (!branchOperations.BranchExist(nmBranch))
                    throw new Exception("BRANCH NÃO ENCONTRADA -->" + nmBranch + " DO REPOSITORIO -->" + repoName);

                if (!repositorioService.RepositorioExist(repoName))
                    throw new Exception("REPOSITORIO NÃO ENCONTRADO -->" + repoName);

                return branchOperations.GetBranchId(nmBranch, repoName);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : " + e.Message);
                logger.LogWarning("ERROR : " + e.Message);
                return -1;
            }
        }
        public (string, string) GetBranchEmailsAdress(string nmBranch, string repoName)
        {
            try
            {
                if (nmBranch.Contains("origin/"))
                    nmBranch = nmBranch.Remove(0, 7);

                if (!branchOperations.BranchExist(nmBranch))
                    throw new Exception("BRANCH NÃO ENCONTRADA -->" + nmBranch + " DO REPOSITORIO -->" + repoName);

                if (!repositorioService.RepositorioExist(repoName))
                    throw new Exception("REPOSITORIO NÃO ENCONTRADO -->" + repoName);

                return branchOperations.GetBranchEmailsAdress(nmBranch, repoName);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : " + e.Message);
                logger.LogWarning("ERROR : " + e.Message);
                return (null, null);
            }

        }

        public bool BranchExist(string nmBranch) => branchOperations.BranchExist(nmBranch);
        public bool BranchExist(int idBranch) => branchOperations.BranchExist(idBranch);
    }
}
