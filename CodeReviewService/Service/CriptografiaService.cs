using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Service
{
    public class CriptografiaService
    {
        public static string Encrypt(string input)
        {
            return Infra.Criptografia.Encrypt(input);
        }

        public static string Decrypt(string input)
        {
            return Infra.Criptografia.Decrypt(input);
        }
    }
}
