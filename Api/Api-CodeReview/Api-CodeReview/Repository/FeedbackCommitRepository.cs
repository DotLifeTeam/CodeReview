using Api_CodeReview.Context;
using Api_CodeReview.Models;
using Api_CodeReview.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_CodeReview.Repository
{
    public class FeedbackCommitRepository : IFeedbackCommitRepository
    {
        private readonly AppDbContext _context;

        public FeedbackCommitRepository(AppDbContext context)
        {
            _context = context;
        }

        public FeedbackCommit GetByIdCommit(string idCommit)
        {
            return _context.FeedbackCommits.AsNoTracking().FirstOrDefault(x => x.Id_Commit == idCommit);
        }

        public bool FeedbackExist(string idCommit)
        {
            try
            {
                if (_context.FeedbackCommits.Any(x => x.Id_Commit == idCommit && x.Status_resposta != null))
                     return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SaveFeedback(FeedbackCommit feedbackCommit)
        {
            _context.Entry(feedbackCommit).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
