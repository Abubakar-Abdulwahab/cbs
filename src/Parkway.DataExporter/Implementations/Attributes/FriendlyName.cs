using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.DataExporter.Implementations.Attributes
{
    [global::System.AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    public sealed class SpecializedEnum : Attribute
    { }

    [global::System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class EnumFriendlyName : Attribute
    {
        private string friendlyName = string.Empty;
        public EnumFriendlyName()
        {

        }
        public EnumFriendlyName(string friendlyName)
        {
            this.friendlyName = friendlyName;
        }
        public string FriendlyName
        {
            get
            {
                return this.friendlyName;
            }
        }
    }

    /// <summary>
    /// Summary description for FriendlyName.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FriendlyName : Attribute
    {
        private string _Name;
        public FriendlyName(string name)
        {
            _Name = name;
        }




        public string Name

        {
            get

            {
                return _Name;
            }
        }
    }
}
