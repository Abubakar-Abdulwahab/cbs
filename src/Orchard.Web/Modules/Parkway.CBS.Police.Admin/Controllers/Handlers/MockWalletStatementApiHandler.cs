using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class MockWalletStatementApiHandler : IMockWalletStatementApiHandler
    {

        public MockWalletStatementApiHandler()
        {

        }

        public List<dynamic> GetStatements(int skip)
        {
            List<dynamic> statements = new List<dynamic>
                {
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894958"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894959"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894960"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894961"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894962"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894963"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894964"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894965"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894966"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894967"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894968"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894969"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894970"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894971"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894972"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894973"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894974"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894975"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894976"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894977"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894978"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894979"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894980"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894981"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894982"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894983"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894984"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894985"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894986"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894987"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894988"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894989"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894990"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894991"
                    },
                    new
                    {
                        date = DateTime.Now.ToString(),
                        longDescription = "something something 23rd - 30th",
                        tranType = 1,
                        amount = 40000.00m,
                        ourReference = $"{DateTime.Now.Ticks}-REFE-00-894992"
                    },
                };

            return statements.Skip(skip).Take(20).ToList();
        }
    }
}