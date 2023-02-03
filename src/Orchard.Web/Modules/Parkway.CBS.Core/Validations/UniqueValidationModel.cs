namespace Parkway.CBS.Core.Validations
{
    public class UniqueValidationModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// If you want to add error to a callback, this is the best place to put the element' name 
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SelectDataValue { get; set; }
        /// <summary>
        /// inclusive clauses is a string containing value you would want to be added to the where DB query
        /// <para>
        /// 
        /// </para>
        /// </summary>
        /// <example></example>
        public string[] InclusiveClauses { get; set; }
        /// <summary>
        /// Error message you want the object model to have
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}