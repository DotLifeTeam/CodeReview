using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Models
{
    public class ReviewSla
    {
        public string LinkRepo { get; set; }
        public string EmailAdmin { get; set; }
        public string EmailReview { get; set; }
        public DateTime DtRegistro { get; set; }
        public DateTime DtSla { get; set; }
        public DateTime DtFeedback { get; set; }
        public string IdCommit { get; set; }
        public string NmBranch { get; set; }
        public string StatusResposta { get; set; }
        public string MsgFeedback { get; set; }
        public string EmailDev { get; set; }

        public ReviewSla()
        {

        }

        public override string ToString()
        {
            return $"[{LinkRepo}, {EmailAdmin}, {EmailReview}, {DtRegistro}, {DtSla}, {DtFeedback}, {IdCommit}, {NmBranch}]";
        }
    }
}
