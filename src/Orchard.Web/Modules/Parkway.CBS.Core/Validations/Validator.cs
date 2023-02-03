using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Localization;
using System.Text;
using Parkway.CBS.Core.Validations.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Validations.Rules.Contracts;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Validations
{
    public class Validator : IValidator
    {
        private List<ErrorModel> _errors = new List<ErrorModel>();
        private readonly IUniqueness _uniqueData;

        public Validator(IUniqueness uniqueData)
        {
            _uniqueData = uniqueData;
        }

        /// <summary>
        /// Get the list of errors
        /// </summary>
        /// <returns>List{ErrorModel}</returns>
        public List<ErrorModel> HasValidationErrors()
        {
            return _errors;
        }

        /// <summary>
        /// Clear the errors
        /// </summary>
        public void ClearErrors()
        {
            _errors.Clear();
        }

        /// <summary>
        /// Check if the collection has duplicate SelectDataValue
        /// </summary>
        /// <typeparam name="M">Subclass of CBSModel</typeparam>
        /// <param name="dataValues"></param>
        /// <returns>IValidator</returns>
        public IValidator BundleCollectionUnique<M>(ICollection<UniqueValidationModel> dataValues) where M : CBSBaseModel
        {
            List<string> repeatedItems = new List<string>();
            foreach (var dataAndValue in dataValues)
            {
                //if true means the identifier is not in the repeated list therefore making it legible for insertion into the 
                //repeated list if the identifier is a duplicate
                bool isNotAlreadyInTheRepeatedItems = string.IsNullOrEmpty(repeatedItems.Where(k => k == dataAndValue.Identifier)
                                                            .Select(k => new StringBuilder(k).ToString()).FirstOrDefault());
                //if the identifier is already in the repeated list then there is not need for the second check
                if (!isNotAlreadyInTheRepeatedItems) continue;

                IEnumerable<string> items = new List<string>();
                //return all the items that have a duplicate value of dataAndValue.SelectDataValue
                items = dataValues.Where(m => (m.Identifier != dataAndValue.Identifier)
                                        && (m.SelectDataValue == dataAndValue.SelectDataValue)
                                        && (m.Name == dataAndValue.Name))
                                        .Select(m => new StringBuilder(m.Identifier).ToString()).ToList();
                
                if (items.Any() && isNotAlreadyInTheRepeatedItems)
                {
                    foreach (var item in items)
                    {
                        repeatedItems.Add(item);
                        AddToErrorsList(item, dataAndValue.ErrorMessage.Length == 0
                                            ? ErrorLang.notunique_vl_text(dataAndValue.Name)
                                            : ErrorLang.givemelocalizedmessage(dataAndValue.ErrorMessage));
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Prepare and process the result of the uniqueness check.
        /// This method checks that the parameters have been inserted according to the format in which values are to be split for data
        /// </summary>
        /// <typeparam name="M">MDARevenueHeadBaseModel</typeparam>
        /// <param name="dataValues">List{UniqueValidationModel}</param>
        /// <returns>IValidator</returns>
        public IValidator BundleUnique<M>(List<UniqueValidationModel> dataValues) where M : CBSBaseModel
        {
            foreach (var dataAndValue in dataValues)
            {
                string[] data = dataAndValue.SelectDataValue.Split(new[] { ':' }, 2);
                if (data.Length != 2) { throw new MissingFieldException(); }
                //checks to make sure you have formed the string correctly
                if (dataAndValue.InclusiveClauses != null)
                {
                    foreach (var item in dataAndValue.InclusiveClauses)
                    {
                        string[] inclusiveValues = item.Split(new[] { ':' }, 3);
                        if (inclusiveValues.Length != 3) { throw new MissingFieldException(); }
                    }
                }
            }
            //list containing the identifiers of the records that are not unique
            List<string> identifiers = new List<string>();
            //all well and good, perform uniqueness check from the database and return identifier of records that are not unique
            identifiers = _uniqueData.BundleUniqueness<M>(dataValues);

            foreach (var identifier in identifiers)
            {
                var dataAndValue = dataValues.Find(r => r.Identifier == identifier);
                if (dataAndValue == null) { continue; }
                AddToErrorsList(identifier, dataAndValue.ErrorMessage.Length == 0 ? ErrorLang.notunique_vl_text(dataAndValue.Name) : ErrorLang.givemelocalizedmessage(dataAndValue.ErrorMessage));
            }
            return this;
        }

        /// <summary>
        /// Compile the list of errors that belong to the identifier
        /// </summary>
        /// <param name="identifier">Identifier of the record</param>
        /// <param name="errorMessage"></param>
        private void AddToErrorsList(string identifier, LocalizedString errorMessage)
        {
            _errors.Add(new ErrorModel { FieldName = identifier, ErrorMessage = errorMessage.ToString() });
        }
    }
}