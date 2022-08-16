using CodeReviewService.Infra.Database.Feedback;
using CodeReviewService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Service
{
    public class FeedbackService
    {
        private readonly FeedbackRepositoryOperations repository;
        private readonly ILogger<FeedbackService> logger;
        public FeedbackService(ILogger<FeedbackService> logger, FeedbackRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
        }
        public void PostInitialBaseFeedback(string IdCommit, int IdBranch)
        {
            try
            {
                if (repository.FeedbackExist(IdCommit))
                    throw new Exception("Ja existe um feedback para esse commit");

                repository.PostFeedback(IdCommit, IdBranch);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR --> " + e.Message);
                logger.LogWarning("ERROR --> " + e.Message);
            }
        }

        public IEnumerable<ReviewSla> GetFeedbackNotReviwed()
        {
            try
            {
                var content = repository.GetAllFeedbacks();

                if (content is null)
                    throw new Exception("Nenhum feedback encontrado");

                content = content.Where(x => x.StatusResposta != null).ToList();

                return content;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR --> " + e.Message);
                logger.LogWarning("ERROR --> " + e.Message);
                return null;
            }
        }
        
        public IEnumerable<ReviewSla> GetFeedbackReviewed()
        {
            return repository.GetReviewedFeedback();
        }
    }
}
