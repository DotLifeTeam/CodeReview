
using CodeReviewService.Application;
using CodeReviewService.Infra.Database.Branch;
using CodeReviewService.Infra.Database.Commit;
using CodeReviewService.Infra.Database.Feedback;
using CodeReviewService.Infra.Database.Repositorio;
using CodeReviewService.Infra.Database.Sla;
using CodeReviewService.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CodeReviewService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "Code Review Analise";
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<BranchOperations>();
                    services.AddSingleton<CommitOperations>();
                    services.AddSingleton<FeedbackRepository>();
                    services.AddSingleton<RepositorioOperations>();
                    services.AddSingleton<SlaOperations>();
                    services.AddSingleton<RepositorioService>();
                    services.AddSingleton<BranchService>();
                    services.AddSingleton<CommitService>();
                    services.AddSingleton<SlaService>();
                    services.AddSingleton<FeedbackService>();
                    services.AddSingleton<EmailOperations>();
                    services.AddSingleton<GitAnalisys>();
                    services.AddSingleton<GitOperations>();
                    /*//Repositories
            Infra.Database.Branch.BranchOperations branchOperations = new(logger);
            Infra.Database.Commit.CommitOperations commitOperations = new(logger);
            Infra.Database.Feedback.FeedbackRepository feedbackRepository = new(logger);
            Infra.Database.Repositorio.RepositorioOperations repositorioOperations = new(logger);
            Infra.Database.Sla.SlaOperations slaOperations = new(logger);
            //Services
            Service.RepositorioService repositorioService = new(repositorioOperations, logger);
            Service.BranchService branchService = new(repositorioService, branchOperations, logger);
            Service.CommitService commitService = new(repositorioService, branchService, commitOperations, logger);
            Service.SlaService slaService = new(repositorioService, slaOperations, logger);
            Service.FeedbackService feedbackService = new(logger, feedbackRepository);

            //Aplication
            Application.EmailOperations emailOperations = new(logger);

            Application.GitAnalisys gitAnalysis = new(emailOperations, branchService, commitService, slaService, feedbackService, logger);
            Application.GitOperations gitOperations = new(repositorioService, gitAnalysis, logger);*/
                });
    }
}
