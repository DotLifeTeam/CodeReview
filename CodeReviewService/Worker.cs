using CodeReviewService.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeReviewService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly GitOperations gitOperations;

        public Worker(ILogger<Worker> logger, GitOperations gitOperations)
        {
            this.gitOperations = gitOperations;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("Worker running at: {time}", DateTimeOffset.Now);
                Util.Start.StartApp(_logger, gitOperations);

                _logger.LogWarning("Worker end run at: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
