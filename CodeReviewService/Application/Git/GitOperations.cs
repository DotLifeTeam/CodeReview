using CodeReviewService.Models;
using CodeReviewService.Service;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace CodeReviewService.Application
{
    public class GitOperations
    {
        private readonly RepositorioService repositorioService;
        private readonly GitAnalisys analisys;
        private readonly ILogger<GitOperations> logger;

        public GitOperations(RepositorioService repositorioService, GitAnalisys analisys, ILogger<GitOperations> logger)
        {
            this.logger = logger;
            this.repositorioService = repositorioService;
            this.analisys = analisys;
        }

        public void ReadAllRepos()
        {
            Console.WriteLine($"ANALISANDO REPOSITORIOS");

            foreach (var folder in ListLocalRepos())
            {

                var repName = folder.Value;
                repName = repName.Remove(0, Util.Tools.GetReposPath().Length + 1);

                Console.WriteLine("GIT PULL IN REP --> " + repName);

                Util.Tools.CmdCommand(@$"/C cd {Util.Tools.GetReposPath()} && cd {repName} && git pull", logger);

                using var repos = new Repository(folder.Value);
                List<string> repoSelectedBranchs = repositorioService.GetRepositoryBranchs(repName);             
                

                foreach (var branch in repos.Branches)
                {

                    string inspectedBranch = branch.FriendlyName;
                    if(inspectedBranch.Contains("origin/"))
                    {
                        inspectedBranch = inspectedBranch.Remove(0, 7);
                        if (!branch.FriendlyName.EndsWith("HEAD") && (repoSelectedBranchs.Contains(inspectedBranch) || repoSelectedBranchs.Contains(inspectedBranch)))
                        {
                            analisys.AnalyzeNewCommits(branch, inspectedBranch, repName, folder.Key);
                            analisys.SlaCommitAnalyzer(branch, repName);
                        }
                    }
                }                
            }

            analisys.AnalyzeNotReviewedCommits();
            analisys.AnalyzeReviewedCommits();
        }

        private Dictionary<string, string> ListLocalRepos()
        {
            Dictionary<string, string> reposLinks = new();
            List<CloneConfig> dbLinks = repositorioService.GetRepositoriesData();
            Console.WriteLine($"BUSCANDO REPOSITORIOS");

            foreach (var item in dbLinks)
            {
                Console.Write(item.RepoName + "\n");
                logger.LogWarning(item.ToString());
                DownloadRepo(item);
            }

            string[] repos = Directory.GetDirectories(Util.Tools.GetReposPath());

            foreach (var folder in repos)
            {
                var repName = folder;
                repName = repName.Remove(0, Util.Tools.GetReposPath().Length + 1);

                foreach (var repLink in dbLinks)
                {
                    if (repLink.Url.Contains(repName))
                    {
                        try { reposLinks.Add(repLink.Url, folder); }
                        catch (Exception) { continue;  }

                    }
                }
            }
            return reposLinks;
        }

        private void DownloadRepo(Models.CloneConfig config)
        {
            if (config.Url.StartsWith("https"))
                config.Url = config.Url[8..];

            config.Password = Service.CriptografiaService.Decrypt(config.Password);
            config.Username = Service.CriptografiaService.Decrypt(config.Username);

            string cloneUrl = $"https://{config.Username}:{config.Password}@{config.Url}";
            
            string cmdCommand = @$"/C cd {Util.Tools.GetReposPath()} && git clone {cloneUrl}";

            Util.Tools.CmdCommand(cmdCommand, logger);
        }
    }
}