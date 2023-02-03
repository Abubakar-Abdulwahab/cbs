using Parkway.DataExporter.Implementations.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.DataExporter.Implementations.Objects
{
    public static class ObjectExtensions
    {

        /*public static IDictionary<string, string> ToDictionary(this object source)
        {
            var dictionary = new Dictionary<string, string>();
            Map(new List<string>(), dictionary, source);
            return dictionary;
        }*/

        public static IDictionary<string, string> ToDictionary(this object source, IEnumerable<string> propertiesToSelect = null)
        {
            var exclusiveList = propertiesToSelect ?? new List<string>();
            var result = new Dictionary<string, string>();
            Map(exclusiveList, result, source);
            return result;
        }

        static void AddValue(this IDictionary<string, string> obj, IEnumerable<string> propertiesToSelect, string key, object value)
        {
            var type = value.GetType();
            if (propertiesToSelect.Contains(key) || !propertiesToSelect.Any())
            {
                var _value = type.IsEnum ? ((Enum)value).ToFriendlyName() :
                    type.IsEquivalentTo(typeof(DateTime)) ? DateTime.Parse(value.ToString()).ToString("MMM dd, yyyy") :
                    (type.IsEquivalentTo(typeof(decimal)) || type.IsEquivalentTo(typeof(decimal))) ? ((Decimal)value).ToString("N") :
                    value.ToString();

                obj.Add(key, _value);
            }

        }

        private static void Map(IEnumerable<string> exclusiveList, IDictionary<string, string> result, object source, string prefix = null)
        {
            var properties = source.GetType().GetProperties();

            foreach (var x in properties)
            {
                var key = string.Format("{0}.{1}", prefix, x.Name).TrimStart('.');
                var value = x.GetValue(source, null);
                if (value != null)
                {
                    var type = value.GetType();
                    if (type.IsValueType || type.IsEquivalentTo(typeof(String)))
                    {
                        if (type.IsGenericType/* && type.GetGenericTypeDefinition().Equals(typeof(KeyValuePair<,>))*/)
                        {
                            Map(exclusiveList, result, value, key);
                        }
                        else/* (type.IsEnum || type.IsPrimitive || type.IsEquivalentTo(typeof(String)))*/
                        {
                            result.AddValue(exclusiveList, key, value);
                        }
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(type))
                    {
                        var elementType = type.GetElementType();
                        var isGeneric = elementType != null && elementType.IsGenericType;
                        var items = ((IEnumerable)value);//.Select((item, index) => new { Key = string.Format("{0}[{1}]", key, index), Value = item, Index = index });

                        var taskList = new List<Task>();

                        var i = 0;
                        foreach (var itm in items)
                        {
                            var iKey = string.Format("{0}[{1}]", key, i);
                            if (isGeneric)
                            {
                                Map(exclusiveList, result, itm, iKey);
                            }
                            else
                            {
                                result.AddValue(exclusiveList, iKey, itm);
                            }
                            i++;
                        }
                    }
                    else
                    {
                        Map(exclusiveList, result, value, key);
                    }
                }

                /*Parallel.ForEach(properties, x => {
                    var key = string.Format("{0}.{1}", prefix, x.Name).TrimStart('.');
                    var value = x.GetValue(source, null);
                    if(value != null)
                    {
                        var type = value.GetType();
                        if (type.IsValueType || type.IsEquivalentTo(typeof(String)))
                        {                        
                            if(type.IsGenericType)
                            {
                                Map(exclusiveList, result, value, key);
                            }
                            else
                            {
                                result.AddValue(exclusiveList, key, value);
                            }
                        } 
                        else if(typeof(IEnumerable).IsAssignableFrom(type))
                        {
                            var isGeneric = type.GetElementType().IsGenericType;
                            var items = ((IEnumerable)value);//.Select((item, index) => new { Key = string.Format("{0}[{1}]", key, index), Value = item, Index = index });

                            var taskList = new List<Task>();

                            var i = 0;
                            foreach(var itm in items)
                            {
                                var iKey = string.Format("{0}[{1}]", key, i);
                                taskList.Add(isGeneric ? Task.Run(() => Map(exclusiveList, result, itm, iKey)) : Task.Run(() => result.AddValue(exclusiveList, iKey, itm)));
                                i++;
                            }
                            Task.WaitAll(taskList.ToArray());
                        }
                        else
                        {
                            Map(exclusiveList, result, value, key);
                        }
                    }
                });*/
            }
        }

    }
}
