using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class VerTokenResult
    {
        public Int64 VerObjId { get; set; }

        public DateTime CreatedAt { get; set; }
    }


    public class VerTokenEncryptionObject
    {
        public Int64 VerId { get; set; }

        public int VerificationType { get; set; }

        public string PaddingText { get; set; }

        public DateTime CreatedAt { get; set; }

        public RedirectReturnObject RedirectObj { get; set; }
    }


    public class RedirectReturnObject
    {
        public string RouteName { get; set; }

        public dynamic RedirectObject { get; set; }

    }

    public class ValidateVerReturnValue
    {
        public long CBSUserId { get; set; }

        public bool IsAdministrator { get; set; }

        public RedirectReturnObject RedirectObj { get; set; }

    }
}