using Api_CodeReview.Context;
using Api_CodeReview.Models;
using Api_CodeReview.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_CodeReview.Service
{
    public class FeedbackCommitService
    {
        private readonly FeedbackCommitRepository repository;

        public FeedbackCommitService(AppDbContext context)
        {
            repository = new(context);
        }

        public void PostFeedback(string idCommit, string feedback, string status_resposta)
        {
            if (idCommit is null)
                throw new Exception("id Commit está em branco");

            if (repository.FeedbackExist(idCommit))
                throw new Exception("ja existe um feedback para esse commit");

            if (status_resposta is null)
                throw new Exception("Status da resposta ao commit não encontrado");

            if (feedback is null)
                feedback = string.Empty;

            FeedbackCommit feedbackCommit = repository.GetByIdCommit(idCommit);
            feedbackCommit.Status_resposta = status_resposta;
            feedbackCommit.Mensagem_feedback = feedback;
            feedbackCommit.Dt_feedback = DateTime.Now;

            repository.SaveFeedback(feedbackCommit);
        }
        public bool FeedbackExist(string idCommit)
        {
            return repository.FeedbackExist(idCommit);
        }
    }
}
