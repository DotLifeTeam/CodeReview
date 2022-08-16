using CodeReviewService.Infra.Database.Commit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Service
{
    public class CommitService
    {
        private readonly ICommitOperations commitOperations;
        private readonly RepositorioService repositorioService;
        private readonly BranchService branchService;
        private readonly ILogger<CommitService> logger;

        public CommitService(RepositorioService repositorio, BranchService branch, CommitOperations commitOperations, ILogger<CommitService> logger)
        {
            this.logger = logger;
            this.commitOperations = commitOperations;
            repositorioService = repositorio;
            branchService = branch;
        }

        public (DateTime, string) GetLastCommitDateAndId(string branchName, string repoName)
        {
            try
            {
                if (!branchService.BranchExist(branchName))
                    throw new Exception("BRANCH NÃO ENCONTRADA -->" + branchName + " DO REPOSITORIO ->" + repoName);

                if (!repositorioService.RepositorioExist(repoName))
                    throw new Exception("REPOSITORIO NÃO ENCONTRADO -->" + repoName);

                return commitOperations.GetLastCommitDateAndId(branchName, repoName);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : " + e.Message);
                logger.LogWarning("ERROR : " + e.Message);
                return (DateTime.Now, null);
            }
        }

        public void InsertLastCommit(Models.Commit commit, int idBranch)
        {

            try
            {
                if (commit.IdCommit == null)
                    throw new Exception("ID DO NOVO COMMIT EM BRANCO");

                if (commit.Nm_autor == null)
                    throw new Exception("AUTOR NOVO COMMIT EM BRANCO");

                if(commit.Nm_mensagem == null)
                    throw new Exception("MENSAGEM DO NOVO COMMIT EM BRANCO");

                if (!branchService.BranchExist(idBranch))
                    throw new Exception("ESSA BRANCH NÃO EXISTE ->" + idBranch);

                commitOperations.InsertLastCommit(commit, idBranch);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : " + e.Message);
                logger.LogWarning("ERROR : " + e.Message);
                return;
            }
        }

        public void UpdateLastCommit(Models.Commit commit, string oldIdCommit)
        {
            try
            {
                if (commit.IdCommit == null)
                    throw new Exception("ID DO NOVO COMMIT EM BRANCO");

                if (commit.Nm_autor == null)
                    throw new Exception("AUTOR NOVO COMMIT EM BRANCO");

                if (commit.Nm_mensagem == null)
                    throw new Exception("MENSAGEM DO NOVO COMMIT EM BRANCO");

                if (!commitOperations.CommitExist(oldIdCommit))
                    throw new Exception("ESSE ID COMMIT NÃO EXISTE ->" + oldIdCommit);

                commitOperations.UpdateLastCommit(commit, oldIdCommit);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : " + e.Message);
                logger.LogWarning("ERROR : " + e.Message);
                return;
            }
        }
    }
}
