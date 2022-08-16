using CodeReviewService.Application;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReviewService.Util
{
    class Start
    {
        public static void StartApp(ILogger logger, GitOperations gitOperations)
        {
            Util.Tools.InitalConfig(logger);
            gitOperations.ReadAllRepos();

        }
    }
}
