using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Api_CodeReview.Models
{
    [Table("tbFeedback")]
    public class FeedbackCommit
    {

        [Key]
        public int Id_feedback { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string Id_Commit { get; set; }

        [StringLength(50, MinimumLength = 5)]
        public string Status_resposta { get; set; }

        [StringLength(700, MinimumLength = 0)]
        public string Mensagem_feedback { get; set; }

        [Required]
        public DateTime Dt_registro { get; set; }
        
        public DateTime Dt_feedback { get; set; }

        [Required]
        public int Id_branch { get; set; }
    }
}
