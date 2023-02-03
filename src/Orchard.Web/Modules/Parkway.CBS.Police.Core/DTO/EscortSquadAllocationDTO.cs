using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class EscortSquadAllocationGroupDTO
    {
        public Int64 Id { get; set; }

        /// <summary>
        /// this is the level the application is on
        /// </summary>
        public EscortProcessStageDefinitionDTO RequestLevel { get; set; }

        public string Comment { get; set; }

        public bool Fulfilled { get; set; }

    }
}