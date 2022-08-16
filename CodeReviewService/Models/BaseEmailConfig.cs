using System.Net.Mail;

namespace CodeReviewService.Models
{
    public class BaseEmailConfig
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string[] To { get; set; }
        public string[] Cc { get; set; }
        public string From { get; set; }
        public string FromNome { get; set; }
        public MailPriority Prioridade { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }

        public BaseEmailConfig()
        {

        }
    }
}