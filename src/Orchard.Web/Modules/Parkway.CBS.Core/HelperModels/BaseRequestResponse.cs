using System.Text;

namespace Parkway.CBS.Core.HelperModels
{
    public class BaseRequestResponse
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            var properties = this.GetType().GetProperties();

            var members = this.GetType().GetMembers();

            foreach (var property in properties)
            {
                if (property.MemberType == System.Reflection.MemberTypes.NestedType)
                {
                    var mems = property.GetType().GetProperties();
                    foreach (var _mem in mems)
                    {
                        sb.AppendLine(string.Format("Name: {0}  Value: {1}", _mem.Name, _mem.GetValue(_mem)));

                    }
                }
                //else if () { }
                else
                {
                    sb.AppendLine(string.Format("Name: {0}  Value: {1}", property.Name, property.GetValue(this)));
                }
            }

            return sb.ToString();
        }
    }
}