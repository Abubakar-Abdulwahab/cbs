using CBSPay.Core.Models;
using CBSPay.Core.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< dev
using RestSharp;
using Newtonsoft.Json.Converters;
using CBSPay.Core.APIModels;
//========================================================================
using NHibernate;
using Parkway.Tools.NHibernate;
using System.Security.Cryptography;
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> obi_dev

namespace CBSPay.Core.Helpers
{
    public class Utils
    {
        private static ILog Logger { get { return LogManager.GetLogger("CBSPay"); } }
        public static string GetRINFromCustReference(string custReference)
        {
            var custReferenceSplit = custReference.Trim().Split('-');
            var refNo = custReferenceSplit[0];
            return refNo;
        }

        public static bool RefNumberIsNotNull(string referenceNumber)
        {
            if(referenceNumber == null)
            {
                return false;
            }
            return true; 
        }

        public static string GetPhoneNumberFromCustReference(string custReference)
        {
            var custReferenceSplit = custReference.Trim().Split('-');
            var phoneNumber = custReferenceSplit[1];
            return phoneNumber;
        }

        public static string GetPaymentIdentifier(string paymentChannelName, string referenceNumber)
        {
            var randomNumber = GenerateRandom(5).FirstOrDefault();
            var rNo = referenceNumber.Length == 7? referenceNumber.Substring(3, 4) : referenceNumber.Substring(1,4);
            return $"{randomNumber}{rNo}_{ DateTime.Now.Hour}";
        }
        static Random random = new Random();

        /// <summary>
        /// Got this template online
        /// </summary>
        /// <param name="count"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        // Note, max is exclusive here!
        public static List<int> GenerateRandom(int count, int min, int max)
        {

            if (max <= min || count < 0 ||
                    // max - min > 0 required to avoid overflow
                    (count > max - min && max - min > 0))
            {
                // need to use 64-bit to support big ranges (negative min, positive max)
                throw new ArgumentOutOfRangeException("Range " + min + " to " + max +
                        " (" + ((Int64)max - (Int64)min) + " values), or count " + count + " is illegal");
            }

            // generate count random values.
            HashSet<int> candidates = new HashSet<int>();

            // start count values before max, and end at max
            for (int top = max - count; top < max; top++)
            {
                // May strike a duplicate.
                // Need to add +1 to make inclusive generator
                // +1 is safe even for MaxVal max value because top < max
                if (!candidates.Add(random.Next(min, top + 1)))
                {
                    // collision, add inclusive max.
                    // which could not possibly have been added before.
                    candidates.Add(top);
                }
            }

            // load them in to a list, to sort
            List<int> result = candidates.ToList();

            // shuffle the results because HashSet has messed
            // with the order, and the algorithm does not produce
            // random-ordered results (e.g. max-1 will never be the first value)
            for (int i = result.Count - 1; i > 0; i--)
            {
                int k = random.Next(i + 1);
                int tmp = result[k];
                result[k] = result[i];
                result[i] = tmp;
            }
            return result;
        }

        /// <summary>
        /// a list of randomly generated numbers
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<int> GenerateRandom(int count)
        {
            return GenerateRandom(count, 0, Int32.MaxValue);
        }


        public static ISession GetSession()
        {
            return SessionManager.GetInstance("CBSPaySessionFactory", "Default").GetSession();
        }

        public static bool ConfigValueIsNull(string configValue)
        {
            if (!string.IsNullOrWhiteSpace(configValue))
            {
                return false;
            }
            return true;
        }

        public static T DeserializeXML<T>(string XmlString) where T : class
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(T));

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(XmlString)))
                {
                    var _object = deserializer.Deserialize(stream);

                    return (T)_object;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static XDocument SerializeToXML<T>(T entity) where T : class
        //public static XDocument SerializeToXML<T>(T entity) where T : class
        {            
            try
            {
                if (entity == null) return null;

                XmlDocument xmlDoc = new XmlDocument();   //Represents an XML document, 
                // Initializes a new instance of the XmlDocument class.   
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                XmlSerializer xmlSerializer = new XmlSerializer(entity.GetType());
                //StringWriter myWriter = new StringWriter();
                //xmlSerializer.Serialize(myWriter, entity, ns);
                // Creates a stream whose backing store is memory. 
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, entity, ns);
                    xmlStream.Position = 0;
                    //Loads the XML document from the specified string.
                    xmlDoc.Load(xmlStream);
                    var xmlString =  xmlDoc.InnerXml;
                    XDocument doc = XDocument.Parse(xmlString);
                    return doc;

                }
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while deserializing to an XML Document");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }

        public static XmlSerializer GetXMLFromObject<T>(T entity)
        {
            
            XmlSerializer serializer = new XmlSerializer(entity.GetType());
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            StringWriter sw = new StringWriter();
            //XmlTextWriter tw = null;
            serializer.Serialize(sw, entity, ns);
            sw.Close();
           
            return serializer;
            //return sw.ToString();
        }

        public static string ComputeHMAC(string value, string key)
        {
            byte[] byteSecretKey = Encoding.ASCII.GetBytes(key);
            byte[] byteConcatenatedstring = Encoding.ASCII.GetBytes(value);
            HMACSHA256 sha = new HMACSHA256(byteSecretKey);

            byte[] hash = sha.ComputeHash(byteConcatenatedstring);
            return BitConverter.ToString(hash).Replace("-", "");

        }

        /// <summary>
        /// Generate SH512 hash for the input string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns><see cref="string"/></returns>
        public static string GenerateSHA512String(string inputString)
        {
            Logger.Debug($"About to hash {inputString} using SHA512 hashing principle");
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hashBytes = sha512.ComputeHash(bytes);

            string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            return hash;
        }

        public static bool IsValidEIRSPaymentRequest(string MerchantId, string MerchantSecret)
        {
            string configMerchantId = Configuration.MerchantId;
            string configMerchantSecret = Configuration.MerchantSecret;
            if (MerchantId == configMerchantId && MerchantSecret == configMerchantSecret)
            {
                return true;
            }
            else
                return false;

        }

        public static bool IsValidNetPayRequest(string MerchantId, string MerchantSecret)
        {
            string configMerchantId = Configuration.MerchantId;
            string configMerchantSecret = Configuration.MerchantSecret;
            if (MerchantId == configMerchantId && MerchantSecret == configMerchantSecret)
            {
                return true;
            }
            else
                return false;

        }


        public static NetPayPaymentModel GetNetPayModel(string transactionReference, string paymentDescription, string CustomerName, decimal TotalAmount, string returnUrl)
        {
            string MerchantSecretKey = Configuration.MerchantSecret;
            string MerchantUniqueId = Configuration.MerchantId;

            NetPayRequestModel modelToConvert = new NetPayRequestModel()
            {
                Amount = TotalAmount,
                Currency = "NGN",
                MerchantUniqueId = MerchantUniqueId,
                ReturnUrl = returnUrl,
                MerchantSecretKey = MerchantSecretKey,
                TransactionReference = transactionReference

            };
            var requestDictionary = modelToConvert.ToDictionary().OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value);

            string concatenatedValues = string.Join("", requestDictionary.Select(kvp => string.Format("{0}", kvp.Value)));

            string HMAC = ComputeHMAC(concatenatedValues, MerchantSecretKey);

            return new NetPayPaymentModel
            {
                Amount = modelToConvert.Amount,
                MerchantUniqueId = modelToConvert.MerchantUniqueId,
                Currency = modelToConvert.Currency,
                CustomerName = CustomerName,
                HMAC = HMAC,
                Description = paymentDescription,
                ReturnUrl = modelToConvert.ReturnUrl,
                TransactionReference = modelToConvert.TransactionReference
            };

        }

        public static bool IsValidPayDirectCallingIP(string requestUrI)
        {
            //"http://192.168.1.14:2018/api/PayDirectPOARequest"
            //check if the paydirect calling Ip has been configured in the config
            string payDirectIP1 = Configuration.GetPayDirectIP1;
            string payDirectIP2 = Configuration.GetPayDirectIP2;
            var payDirect1 = ConfigValueIsNull(payDirectIP1);
            var payDirect2 = ConfigValueIsNull(payDirectIP2);
            if (payDirect1 && payDirect2 )
            {
                Logger.Error("Could not retrieve configured Client IP from the web config; return false");
                return false;
            }
            //if at least one of the values are in the config, check if the calling ip is the same as the one configured in the db
            if (payDirect1)
            {
                if (!payDirect2)
                {
                    if (payDirectIP2.Contains(requestUrI))
                    {
                        Logger.Debug("IP1 is null, but IP2 is not null and it is the same as the calling IP; return true");
                        return true;
                    }
                }
            }
            if (payDirect2)
            {
                if (!payDirect1)
                {
                    if (payDirectIP1.Contains(requestUrI))
                    {
                        Logger.Debug("IP2 is null, but IP1 is not null and it is the same as the calling IP; return true");
                        return true;
                    }
                }
            }
            if (!payDirect1)
            {
                if (!payDirect2)
                {
                    if (payDirectIP1.Contains(requestUrI) || payDirectIP2.Contains(requestUrI))
                    {
                        Logger.Debug("IP2 is not null, but IP1 is not null and it is the same as the calling IP; return true");
                        return true;
                    }
                }
            }
            Logger.Error("Could not validate calling IP; return false");
            return false;
        }
    }

    public static class ObjectExtensions
    {
        /// <summary>
        /// Used to simplify and beautify casting an object to a type. 
        /// </summary>
        /// <typeparam name="T">Type to be casted</typeparam>
        /// <param name="obj">Object to cast</param>
        /// <returns>Casted object</returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return (T)obj;
        }

        /// <summary>
        /// Converts given object to a value type using <see cref="Convert.ChangeType(object,System.TypeCode)"/> method.
        /// </summary>
        /// <param name="obj">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        public static T To<T>(this object obj)
            where T : struct
        {
            if (typeof(T) == typeof(Guid))
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
            }

            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Check if an item is in a list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="list">List of items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            return obj.ToDictionary<object>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ToFieldDictionary(this object obj)
        {
            return obj.ToFieldDictionary<object>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null) ThrowExceptionWhenSourceArgumentIsNull();

            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                object value = property.GetValue(source);
                if (IsOfType<T>(value))
                {
                    dictionary.Add(property.Name, (T)value);
                }
            }
            return dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IDictionary<string, T> ToFieldDictionary<T>(this object source)
        {
            if (source == null) ThrowExceptionWhenSourceArgumentIsNull();

            var dictionary = new Dictionary<string, T>();
            var fields = source.GetType().GetFields().ToList();

            foreach (var field in fields)
            {
                object value = field.GetValue(source);
                if (IsOfType<T>(value))
                {
                    dictionary.Add(field.Name, (T)value);
                }
            }
            return dictionary;
        }

        public static NameValueCollection ToNameValueCollection<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            var nameValueCollection = new NameValueCollection();

            foreach (var kvp in dict)
            {
                string value = null;
                if (kvp.Value != null)
                    value = kvp.Value.ToString();

                nameValueCollection.Add(kvp.Key.ToString(), value);
            }

            return nameValueCollection;
        }

        

        /// <summary>
        /// 
        /// </summary>
        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new NullReferenceException("Unable to convert anonymous object to a dictionary. The source anonymous object is null.");
        }
    }

    public class JsonNetFormatter : MediaTypeFormatter
    {
        public JsonNetFormatter()
        {
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        }

        public override bool CanWriteType(Type type)
        {
            // don't serialize JsonValue structure use default for that
            if (/*type == typeof(JsonValue) ||*/ type == typeof(JsonObject) || type == typeof(JsonArray))
                return false;

            return true;
        }

        public override bool CanReadType(Type type)
        {
            //if (type == typeof(IKeyValueModel))
            //    return false;

            if (type == typeof(APIResponse))
                return true;

            return false;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var task = Task<object>.Factory.StartNew(() =>
            {
                var settings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };

                var sr = new StreamReader(stream);
                var jreader = new JsonTextReader(sr);

                var ser = new JsonSerializer();
                ser.Converters.Add(new IsoDateTimeConverter());

                object val = ser.Deserialize(jreader, type);
                return val;
            });

            return task;
        }

        public override Task WriteToStreamAsync/*(Type type, object value, Stream stream, System.Net.Http.Headers.HttpContentHeaders contentHeaders, FormatterContext formatterContext, System.Net.TransportContext transportContext)*/
            (Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var settings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };

                string json = JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented,
                                                          new JsonConverter[1] { new IsoDateTimeConverter() });

                byte[] buf = Encoding.Default.GetBytes(json);
                stream.Write(buf, 0, buf.Length);
                stream.Flush();
            });

            return task;
        }
    }

    public class CustomNamespaceXmlFormatter : XmlMediaTypeFormatter
    {
        private readonly string defaultRootNamespace;

        public CustomNamespaceXmlFormatter() : this(string.Empty)
        {
        }

        public CustomNamespaceXmlFormatter(string defaultRootNamespace)
        {
            this.defaultRootNamespace = defaultRootNamespace;
        }

        public override bool CanReadType(Type type)
        {
            //if (type == typeof(IKeyValueModel))
            //    return false;

            if (type == typeof(PaymentNotificationResponse) || type == typeof(CustomerInformationResponse))
                return true;

            return false;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            if (type == typeof(CustomerInformationResponse) || type == typeof(PaymentNotificationResponse))
            {
                var xmlRootAttribute = type.GetCustomAttribute<XmlRootAttribute>(true);
            if (xmlRootAttribute == null)
                xmlRootAttribute = new XmlRootAttribute(type.Name)
                {
                    Namespace = defaultRootNamespace
                };
            else if (xmlRootAttribute.Namespace == null)
                xmlRootAttribute = new XmlRootAttribute(xmlRootAttribute.ElementName)
                {
                    Namespace = defaultRootNamespace
                };

            var xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, xmlRootAttribute.Namespace);

            return Task.Factory.StartNew(() =>
            {
                var serializer = new XmlSerializer(type, xmlRootAttribute);
                serializer.Serialize(writeStream, value, xns);
            });

            }

            return Task.Factory.StartNew(() =>
            {
                var settings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };

                string json = JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented,
                                                          new JsonConverter[1] { new IsoDateTimeConverter() });

                byte[] buf = Encoding.Default.GetBytes(json);
                writeStream.Write(buf, 0, buf.Length);
                writeStream.Flush();

                //var formatter = new JsonMediaTypeFormatter();
                //var settings = formatter.SerializerSettings;
                //settings.Formatting = Newtonsoft.Json.Formatting.Indented;
                //settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                //settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            });
        }
    }

    

}
