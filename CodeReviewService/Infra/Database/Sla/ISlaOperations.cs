using System;

namespace CodeReviewService.Infra.Database.Sla
{
    interface ISlaOperations
    {
        DateTime GetSlaCommitDate(string repoName);
        DateTime GetSlaReviewDate(string repoName);
        bool SlaExist(string repoName);
    }
}
