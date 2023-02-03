using Newtonsoft.Json;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxEntityCategoryVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Identifier { get; set; }

        public string StringIdentifier { get; set; }

        public bool Status { get; set; }

        public bool RequiresLogin { get; set; }


        /// <summary>
        /// JSON setting for category
        /// </summary>
        public string JSONSettings { get; set; }


        private TaxEntityCategorySettings _settings;


        public TaxEntityCategorySettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = GetSettings();
                }
                return _settings;
            }
        }

        public TaxEntityCategorySettings GetSettings()
        {
            if (string.IsNullOrEmpty(this.JSONSettings)) { return new TaxEntityCategorySettings { }; }
            return JsonConvert.DeserializeObject<TaxEntityCategorySettings>(this.JSONSettings);
        }
    }
}