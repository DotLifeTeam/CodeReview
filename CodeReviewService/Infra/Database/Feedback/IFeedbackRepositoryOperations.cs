﻿using CodeReviewService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Infra.Database.Feedback
{
    interface IFeedbackRepositoryOperations
    {
        IEnumerable<ReviewSla> GetAllFeedbacks();
        void PostFeedback(string IdCommit, int IdBranch);
        bool FeedbackExist(string IdCommit);
    }
}
