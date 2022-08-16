using CodeReviewService.Models;
using CodeReviewService.Service;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Application
{
    public class GitAnalisys
    {
        private readonly EmailOperations emailOperations;
        private readonly BranchService branchService;
        private readonly CommitService commitService;
        private readonly SlaService slaService;
        private readonly FeedbackService feedbackService;
        private readonly ILogger<GitAnalisys> logger;

        public GitAnalisys(EmailOperations emailOperations, BranchService branchService,
            CommitService commitService, SlaService slaService, FeedbackService feedbackService, ILogger<GitAnalisys> logger)
        {
            this.logger = logger;
            this.emailOperations = emailOperations;
            this.branchService = branchService;
            this.commitService = commitService;
            this.slaService = slaService;
            this.feedbackService = feedbackService;
        }

        public void AnalyzeNewCommits(Branch branch, string branchName, string repoName, string repoLink)
        {

            LibGit2Sharp.Commit lastCommit = branch.Commits.ElementAtOrDefault(0);
            DateTime lastCommitDate = lastCommit.Author.When.DateTime;

            (DateTime, string) dbLastCommitDateAndId = commitService.GetLastCommitDateAndId(branchName, repoName);
            int idBranch = branchService.GetBranchId(branchName, repoName);

            if (dbLastCommitDateAndId.Item2 == null && idBranch != -1)
                commitService.InsertLastCommit(new Models.Commit(lastCommit.Id.ToString(), lastCommit.Message, lastCommit.Author.Name, lastCommitDate), idBranch);


            if (lastCommitDate.CompareTo(dbLastCommitDateAndId.Item1) > 0)
            {
                SendCommitEmail(branch, repoName, lastCommit, repoLink);
                commitService.UpdateLastCommit(new Models.Commit(lastCommit.Id.ToString(), lastCommit.Message, lastCommit.Author.Name, lastCommitDate), dbLastCommitDateAndId.Item2);

                feedbackService.PostInitialBaseFeedback(lastCommit.Id.ToString(), idBranch);
            }
        }

        public void SlaCommitAnalyzer(Branch branch, string repoName)
        {
            LibGit2Sharp.Commit lastCommit = branch.Commits.ElementAtOrDefault(0);
            DateTime lastCommitDate = lastCommit.Author.When.DateTime;
            DateTime slaCommitDate = slaService.GetSlaCommitDate(repoName);

            if (lastCommitDate.CompareTo(slaCommitDate) < 0 && !slaCommitDate.ToShortDateString().Equals(DateTime.Today.ToShortDateString()))
            {
                SendSlaCommitEmail(branch, repoName);
            }
        }

        public void AnalyzeNotReviewedCommits()
        {
            var result = feedbackService.GetFeedbackNotReviwed();

            if (result is not null)
            {
                foreach (var item in result)
                {
                    if (item.DtRegistro.CompareTo(item.DtSla) < 0)
                        SendAnalyzeNotReviewdCommits(item);
                }
            }
        }

        public void AnalyzeReviewedCommits()
        {
            var result = feedbackService.GetFeedbackReviewed();

            if (result is not null)
            {
                foreach (var item in result)
                {
                    if (item.DtFeedback.CompareTo(DateTime.Today.AddDays(-2)) > 0)
                        SendAnalyzeReviewedCommits(item);
                }
            }
        }

        private void SendCommitEmail(Branch branch, string repoName, LibGit2Sharp.Commit newCommit, string link)
        {
            if (link.EndsWith(".git"))
                link = link.Remove(link.Length - 4, 4);

            string url = @$"{link}" + @$"/commit/{newCommit.Id}?refName=refs%2Fheads%2F{branch.FriendlyName}";
            string aprovarUrl = "http://10.80.10.5:88/API/api/FeedBackCommit/aprovado/" + newCommit.Id + "/commit-aprovado-7876";
            string reprovarUrl = "http://10.80.10.5:88/API/api/FeedBackCommit/" + newCommit.Id;

            var conteudo = ""
            + "<body>"
            + "<div>"
            + "<h2>Um novo commit foi registrado</h2>"
            + "</div>"
            + "<div>"
            + $"autor: {newCommit.Author.Name} | email: {newCommit.Author.Email} | data: {newCommit.Author.When.DateTime} | commit: {newCommit.Message} | branch: {branch.FriendlyName} | <a id = \"link\" href = \"{url}\" target = \"_blank\">link</a>"
            + "</div>"
            + "<div>"
            + $"<a id =\"aprovar\" href=\"{aprovarUrl}\"; target = \"_blank\">APROVAR</a>"
            + "</div>"
            + "<div>"
            + $"<a id =\"reprovar\" href=\"{reprovarUrl}\" target = \"_blank\">REPROVAR</a>"
            + "</div>"
            + "</body>";

            Console.WriteLine("\nNovo commit encontrado, disparando email");
            Console.WriteLine("\n");

            var email = branchService.GetBranchEmailsAdress(branch.FriendlyName, repoName);
            emailOperations.SendNewCommitEmail(conteudo, newCommit.Author.Name, branch.FriendlyName, email.Item2);
        }

        private void SendSlaCommitEmail(Branch branch, string repoName)
        {
            var conteudo =
                $"Atenção dev, dentro repositório {repoName}, a branch {branch.FriendlyName}\nnão recebe um novo commit dentro do prazo limite";

            Console.WriteLine("Nenhum commit novo encontrado, disparando email");
            Console.WriteLine(conteudo);
            Console.WriteLine("\n");

            var email = branchService.GetBranchEmailsAdress(branch.FriendlyName, repoName);
            emailOperations.SendSlaEmail(conteudo, email, branch.FriendlyName);
        }

        private void SendAnalyzeReviewedCommits(ReviewSla content)
        {
            if (content.LinkRepo.EndsWith(".git"))
                content.LinkRepo = content.LinkRepo.Remove(content.LinkRepo.Length - 4, 4);

            string url = @$"{content.LinkRepo}" + @$"/commit/{content.IdCommit}?refName=refs%2Fheads%2F{content.NmBranch}";

            string msg = $"Olá dev, o seu commit : {url}\n";
            msg += $"foi {content.StatusResposta}\n";
            if(content.MsgFeedback is not null)
                msg += $"com o feedback : {content.MsgFeedback}";

            emailOperations.SendNewCommitReviewed(content, msg);
        }

        private void SendAnalyzeNotReviewdCommits(ReviewSla content)
        {
            if (content.LinkRepo.EndsWith(".git"))
                content.LinkRepo = content.LinkRepo.Remove(content.LinkRepo.Length - 4, 4);

            string url = @$"{content.LinkRepo}" + @$"/commit/{content.IdCommit}?refName=refs%2Fheads%2F{content.NmBranch}";
            string aprovarUrl = "http://10.80.10.5:88/API/api/FeedBackCommit/aprovado/" + content.IdCommit + "/commit-aprovado-7876";
            string reprovarUrl = "http://10.80.10.5:88/API/api/FeedBackCommit/" + content.IdCommit;

            var body = ""
            + "<body>"
            +   "<div>"
            +       "<h2>Atenção, um dev está esperando que o <a href=\""+ url +"\"; target= \"_blank\">commit</a> seja revisado </h2>"
            +   "</div>"
            +   "<div>"
            +   "</div>"
            +   "<div>"
            +       $"<a id =\"aprovar\" href=\"{aprovarUrl}\"; target = \"_blank\">APROVAR</a>"
            +   "</div>"
            +   "<div>"
            +       $"<a id =\"reprovar\" href=\"{reprovarUrl}\" target = \"_blank\">REPROVAR</a>"
            +   "</div>"
            + "</body>";

            emailOperations.SendNewCommitNotReviewed(content, body);
        }

    }
}
