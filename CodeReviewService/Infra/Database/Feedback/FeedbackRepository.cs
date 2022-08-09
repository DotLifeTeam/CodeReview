﻿using CodeReviewService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Infra.Database.Feedback
{
    public class FeedbackRepository : FeedbackRepositoryOperations
    {
        private readonly string connString;
        private readonly ILogger<FeedbackRepository> logger;

        public FeedbackRepository(ILogger<FeedbackRepository> logger)
        {
            connString = Criptografia.Decrypt(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            this.logger = logger;
        }

        public bool FeedbackExist(string IdCommit)
        {
            string cmd = @"SELECT TOP 1 *FROM tbFeedback where Id_commit = @IdCommit";
            SqlParameter pIdCommit = new("@IdCommit", IdCommit);

            using SqlConnection conn = new(connString);
            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pIdCommit);

            try
            {
                conn.Open();
                object result = command.ExecuteScalar();
                if (result is not null)
                    return true;

                return false;
            }
            catch(Exception e)
            {
                Console.WriteLine("ERRO EM FEEDBACK EXIST :" + e.Message);
                logger.LogWarning("ERRO EM FEEDBACK EXIST :" + e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public IEnumerable<ReviewSla> GetAllFeedbacks()
        {
            List<ReviewSla> result = new();

            string cmd = @"
            SELECT r.Nm_url_clone, r.Nm_email_admin, b.Nm_email_review, f.Dt_registro, s.Nr_dias_sla_review, c.Id_Commit, b.Nm_branch, f.Dt_feedback
                FROM tbRepositorio r (nolock)
	                JOIN tbBranch b (nolock)
                ON r.Id_repositorio = b.Id_repositorio
	                JOIN tbFeedback f (nolock)
                ON b.Id_branch = f.Id_branch
	                JOIN tbCommit c (nolock)
                ON c.Id_branch = b.Id_branch
	                JOIN tbSLA s (nolock)
                ON s.Id_repositorio = r.Id_repositorio
            ";

            using SqlConnection conn = new(connString);
            using SqlCommand command = new(cmd, conn);

            try
            {
                conn.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ReviewSla _result = new();

                    _result.LinkRepo = (string)reader[0];
                    _result.EmailAdmin = (string)reader[1];
                    _result.EmailReview = (string)reader[2];
                    _result.DtRegistro = (DateTime)reader[3];

                    int dtSlaDia = (int)reader[4];
                    _result.DtSla = DateTime.Now.AddDays(-dtSlaDia);

                    _result.IdCommit = (string)reader[5];
                    _result.NmBranch = (string)reader[6];

                    if (reader[7] is not DBNull)
                        _result.DtFeedback = (DateTime)reader[7];


                    result.Add(_result);
                }

                return result;
            }
            catch (Exception e)
            {
                logger.LogWarning("ERROR GET ALL FEEDBACK -> "+ e.Message);
                Console.WriteLine("ERROR GET ALL FEEDBACK -> " + e.Message);
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        public void PostFeedback(string IdCommit, int IdBranch)
        {
            SqlParameter pIdCommit = new("@Id_commit", IdCommit);
            SqlParameter pIdBranch = new("@Id_branch", IdBranch);
            SqlParameter pDtRegistro = new("@Dt_registro", DateTime.Now);

            string cmd = @"insert into tbFeedback(Id_commit, Dt_registro, Id_branch) values (@Id_commit, @Dt_registro, @Id_branch)";

            using SqlConnection conn = new(connString);
            using SqlCommand command = new(cmd, conn);
            command.Parameters.Add(pIdCommit);
            command.Parameters.Add(pIdBranch);
            command.Parameters.Add(pDtRegistro);

            try
            {
                conn.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.LogWarning("ERROR POSTING FEEDBACK -> " + e.Message);
                Console.WriteLine("ERROR POSTING FEEDBACK -> " + e.Message);
            }
            finally
            {
                conn.Close();
            }
        }

    }
}
