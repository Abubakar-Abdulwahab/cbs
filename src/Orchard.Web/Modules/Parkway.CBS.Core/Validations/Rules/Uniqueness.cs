using NHibernate.Transform;
using Orchard.Data;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Validations.Rules.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parkway.CBS.Core.Validations.Rules
{
    public class Uniqueness : IUniqueness
    {
        private readonly ITransactionManager _transactionManager;

        public Uniqueness(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        /// <summary>
        /// Checks that the values in the datavalues list are unique, if not 
        /// returns a list of string values which are not unique
        /// </summary>
        /// <typeparam name="T">CBSModel</typeparam>
        /// <param name="dataValues">List{UniqueValidationModel}</param>
        /// <returns>List{string} : Return a list of Identifiers for records that are not unique</returns>
        public List<string> BundleUniqueness<M>(List<UniqueValidationModel> dataValues) where M : CBSBaseModel
        {
            var session = _transactionManager.GetSession();
            Dictionary<string, IEnumerable<ResultCount>> listOfQueryResults = new Dictionary<string, IEnumerable<ResultCount>>();

            foreach (var dataAndValue in dataValues)
            {
                string[] data = dataAndValue.SelectDataValue.Split(new[] { ':' }, 2);
                StringBuilder queryString = new StringBuilder(string.Format("SELECT COUNT(*) as count FROM {0} WHERE ", typeof(M).FullName));
                queryString.Append(String.Format("{0} = :variable ", data[0]));
                List<string> listOfParameters = new List<string>();
                if (dataAndValue.InclusiveClauses.Length > 0)
                {
                    var result = GenerateInclusiveClause(dataAndValue.InclusiveClauses);
                    foreach (var item in result)
                    {
                        queryString.Append(String.Format(" {0} ", item.Key));
                        foreach (var i in item.Value) { listOfParameters.Add(i); }
                        break;
                    }
                }
                var query = session.CreateQuery(queryString.ToString());
                query.SetParameter("variable", data[1].Trim());
                int counter = 1;
                foreach (var item in listOfParameters) { query.SetParameter("variable" + counter++.ToString(), item); }

                listOfQueryResults.Add(dataAndValue.Identifier, query.SetResultTransformer(Transformers.AliasToBean<ResultCount>()).Future<ResultCount>());
            }

            //list to hold the Identifier name of the records that are not unique
            List<string> results = new List<string>();
            foreach (var item in listOfQueryResults)
            {
                foreach (var i in item.Value)
                {
                    var count = i.count;
                    if (count != 0) { results.Add(item.Key); }
                }
            }
            //return the list of records that are not unique. This list contains only the Identifier of the values that are not unique
            return results;
        }

        /// <summary>
        /// This method forms the AND part of the quuery string
        /// The value pass is in the form e.g "TaxEntityId:true:value"
        /// TaxEntityId is the column, true means it's a != operation, and value is
        /// the comparison element
        /// </summary>
        /// <param name="inclusive"></param>
        /// <returns>Dictionary{string, List{string}}</returns>
        private Dictionary<string, List<string>> GenerateInclusiveClause(string[] inclusive)
        {
            Dictionary<string, List<string>> andStringAndVariableParams = new Dictionary<string, List<string>>(1);
            List<string> variables = new List<string>();
            int counter = 1;
            StringBuilder andQuery = new StringBuilder();
            foreach (var item in inclusive)
            {
                string[] inclusiveValues = item.Split(':');
                var andOption = inclusiveValues[1] == "true" ? "!=" : "=";
                andQuery.Append(String.Format(" AND {0} {1} :{2} ", inclusiveValues[0], andOption, "variable" + counter.ToString()));
                variables.Add(inclusiveValues[2]);
                counter++;
            }
            andStringAndVariableParams.Add(andQuery.ToString(), variables);
            return andStringAndVariableParams;
        }
    }
}