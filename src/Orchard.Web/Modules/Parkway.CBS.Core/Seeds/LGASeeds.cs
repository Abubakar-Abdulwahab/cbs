using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Parkway.CBS.Core.Seeds
{
    public class LGASeeds : ILGASeeds
    {
        private readonly IStateModelManager<StateModel> _repo;
        private readonly ILGAManager<LGA> _lgarepo;

        public LGASeeds(IStateModelManager<StateModel> repo, ILGAManager<LGA> lgarepo)
        {
            _repo = repo;
            _lgarepo = lgarepo;
        }


        public void PopLGAs()
        {
            var states = _repo.GetCollection();//.GetCollection(s => s);

            foreach (var item in states)
            {
                //read the file
                //Dictionary<string, string> lgaAndValue = new Dictionary<string, string>();
                string LGAFilePath = Utilities.Util.GetAppRemotePath();
                try
                {
                    foreach (XElement stateElement in XElement.Load($"{LGAFilePath}\\LGAs.xml").Elements(item.Name))
                    {
                        foreach (XElement lgaElement in stateElement.Elements("lga"))
                        {
                            _lgarepo.Save(new LGA { Name = lgaElement.Attribute("name").Value, CodeName = lgaElement.Attribute("value").Value.ToUpper(), State = item });
                        }
                    }
                }
                catch (Exception) { /*throw new Exception("Could not validate LGA");*/ }
            }
        }

        public void AddLgas(List<Dictionary<string, string>> lgaList)
        {
            foreach (Dictionary<string, string> lga in lgaList)
            {
                try
                {
                    _lgarepo.Save(new LGA { Name = lga["name"], CodeName = lga["name"].ToUpper(), State = new StateModel { Id = Int32.Parse(lga["id"]) } });
                }
                catch (FormatException e)
                {
                    //Invalid String Format
                }
            }
        }


        
    }
}