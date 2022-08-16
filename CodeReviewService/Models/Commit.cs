using System;


namespace CodeReviewService.Models
{
    public class Commit
    {
        public string IdCommit { get; set; }
        public string Nm_mensagem { get; set; }
        public string Nm_autor { get; set; }
        public DateTime Dt_commit { get; set; }

        public Commit() { }

        public Commit(string idCommit, string nm_mensagem, string nm_autor, DateTime dt_commit)
        {
            IdCommit = idCommit;
            Nm_mensagem = nm_mensagem;
            Nm_autor = nm_autor;
            Dt_commit = dt_commit;
        }
    }
}
