using Api_CodeReview.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_CodeReview.Repository.Interfaces
{
    interface IFeedbackCommitRepository
    {
        FeedbackCommit GetByIdCommit(string idCommit);
        void SaveFeedback(FeedbackCommit feedbackCommit);
        bool FeedbackExist(string idCommit);
    }
}
