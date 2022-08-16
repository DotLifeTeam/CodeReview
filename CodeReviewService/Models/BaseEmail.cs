using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Models
{
    public class BaseEmail
    {
        public bool IsHtml { get; set; }
        public string Conteudo { get; set; }
    }
}
