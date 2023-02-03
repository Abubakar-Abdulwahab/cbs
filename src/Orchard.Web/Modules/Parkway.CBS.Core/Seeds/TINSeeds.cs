using Orchard.Localization;
using Orchard.Logging;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Seeds
{
    public class TINSeeds : ITINSeeds
    {
        ITINManager<TIN> _repository;
        ITaxEntityCategoryManager<TaxEntityCategory> _sectorRepository;
        ITaxEntityCategoryManager<TaxEntityCategory> _catRepository;
        List<TaxEntityCategory> catties;
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public TINSeeds(ITINManager<TIN> repository, ITaxEntityCategoryManager<TaxEntityCategory> sectorRepository, ITaxEntityCategoryManager<TaxEntityCategory> catRepository)
        {
            _repository = repository;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _sectorRepository = sectorRepository;
            _catRepository = catRepository;
            catties = GetCats();
        }

        public bool Seed1()
        {
            if(catties == null || catties.Count <= 0) { return false; }
            List<TIN> tins = new List<TIN>();

            for (int i = 0; i < 1; i++)
            {
                tins.Add(new TIN()
                {
                    Address = "295 Herbert Macaulay Way, Yaba, Lagos, Nigeria",
                    CompanyName = "Parkway Projects Ltd",
                    DOB = DateTime.Now.ToString(),
                    FirstName = "Uzo",
                    LastName = "Eziukwu",
                    Nationality = "Nigerian",
                    State = "Edo",
                    Occupation = Faker.Lorem.Sentence(4),
                    TINNumber = 1234543.ToString(),
                    Email = "ueziukwu@parkwayprojects.com",
                    TaxEntityCategory = catties.ElementAt(Faker.RandomNumber.Next(0, catties.Count-1)),
                    PhoneNumber = Faker.Phone.Number(),
                });
            }
            if (_repository.SaveBundle(tins)) { Logger.Error("Could not seed tin table"); return false; }
            return true;
        }


        /// <summary>
        /// Return a list of tax categories
        /// </summary>
        /// <returns>List{TaxEntityCategory}</returns>
        private List<TaxEntityCategory> GetCats()
        {
            return _catRepository.GetCollection(catty => catty.Status).ToList();
        }

        public bool Seed2()
        {
            if (catties == null || catties.Count <= 0) { return false; }

            List<TIN> tins = new List<TIN>();

            for (int i = 0; i < 1; i++)
            {
                tins.Add(new TIN()
                {
                    Address = "295 Herbert Macaulay Way, Yaba, Lagos, Nigeria",
                    CompanyName = "Parkway Projects Ltd",
                    DOB = DateTime.Now.ToString(),
                    FirstName = "OluwaRita",
                    LastName = "Fadipe",
                    Nationality = "Nigerian",
                    State = "Edo",
                    Occupation = Faker.Lorem.Sentence(4),
                    TINNumber = 12345432.ToString(),
                    Email = "oluwalita@parkwayprojects.com",
                    PhoneNumber = Faker.Phone.Number(),
                    RCNumber = Faker.RandomNumber.Next(5, 10).ToString(),
                    TaxEntityCategory = catties.ElementAt(Faker.RandomNumber.Next(0, catties.Count - 1)),
                });
            }
            if (_repository.SaveBundle(tins)) { Logger.Error("Could not seed tin table"); return false; }
            return true;
        }

        public bool Seed3()
        {
            if (catties == null || catties.Count <= 0) { return false; }

            List<TIN> tins = new List<TIN>();

            for (int i = 0; i < 1; i++)
            {
                tins.Add(new TIN()
                {
                    Address = "295 Herbert Macaulay Way, Yaba, Lagos, Nigeria",
                    CompanyName = "Parkway Projects Ltd",
                    DOB = DateTime.Now.ToString(),
                    FirstName = "Obinna",
                    LastName = "Agim",
                    Nationality = "Nigerian",
                    State = "Edo",
                    Occupation = Faker.Lorem.Sentence(4),
                    TINNumber = 7777777.ToString(),
                    Email = "oagim@parkwayprojects.com",
                    PhoneNumber = Faker.Phone.Number(),
                    RCNumber = Faker.RandomNumber.Next(5, 10).ToString(),
                    TaxEntityCategory = catties.ElementAt(Faker.RandomNumber.Next(0, catties.Count - 1)),
                });
            }
            if (_repository.SaveBundle(tins)) { Logger.Error("Could not seed tin table"); return false; }
            return true;
        }

        public string Seed4()
        {
            if (catties == null || catties.Count <= 0) { return "No tax category found. Add some tax category"; }

            List<TIN> tins = new List<TIN>();
            var sectors = _sectorRepository.GetCollection(s => s.Status == true).ToList();
            if (sectors == null || sectors.Count <= 0) { return "No sectors found. Add some sectors"; } ;
            int secMax = sectors.Count - 1;

            for (int i = 0; i < 50; i++)
            {
                tins.Add(new TIN()
                {
                    Address = Faker.Address.StreetAddress(true),
                    CompanyName = Faker.Company.Name(),
                    DOB = DateTime.Now.ToString(),
                    FirstName = Faker.Name.First(),
                    LastName = Faker.Name.Last(),
                    Nationality = Faker.Address.Country(),
                    State = Faker.Address.UsState(),
                    Occupation = Faker.Lorem.Sentence(4),
                    TINNumber = Faker.RandomNumber.Next(1234543, 7777777).ToString() + i + DateTime.Now.Second,
                    Email = Faker.Internet.Email(Faker.Name.First()),
                    PhoneNumber = Faker.Phone.Number(),
                    RCNumber = Faker.RandomNumber.Next(1234567, 1000000000).ToString(),
                    TaxEntityCategory = catties.ElementAt(Faker.RandomNumber.Next(0, catties.Count - 1)),
                });
            }
            if (!_repository.SaveBundle(tins)) { return "Could not seed tin table"; }
            return "Seed 4 completed, check database to confirm entries";
        }
    }
}