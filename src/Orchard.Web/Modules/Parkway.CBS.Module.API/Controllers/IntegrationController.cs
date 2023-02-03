using System;
using System.Net;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web.Http;
using Microsoft.Web.Http;
using System.Configuration;
using System.Security.Cryptography;
using Parkway.CBS.Module.API.Middleware;

namespace Parkway.CBS.Module.API.Controllers
{

    /// <summary>
    /// Version 1
    /// <para>All object responses should be contained in the APIResponse object <see cref="APIResponse"/> except incases we cannot control</para>
    /// </summary>
    //[HasClientKey]
    [ApiVersion("1.0")]
    [RoutePrefix("v1/integration")]
    public class IntegrationController : ApiController
    {

        [Route("encryption/encrypt-this")]
        [HttpPost]
        public IHttpActionResult EncryptThis(dynamic model)
        {
            //self assigned key
            string key = ConfigurationManager.AppSettings["SelfAssignedEncryptionKeyValue"];
            string publicKey = model.RSAKey;
            string jsonObject = JsonConvert.SerializeObject(model.Val);

            string EncrptedKey = EncryptKey(key, publicKey);// pass to header as Key
            string MessageBody = EncryptMessage(jsonObject, key);
            return Content(HttpStatusCode.OK, new { EncrptedKey, MessageBody });
        }


        private static string EncryptMessage(string source, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteHash;
            byte[] byteBuff;

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB

            byteBuff = Encoding.UTF8.GetBytes(source);
            string encoded = Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return encoded;
        }

        private static string EncryptKey(string SelfAssignedKey, string _publicKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_publicKey);

            var dataToEncrypt = Encoding.UTF8.GetBytes(SelfAssignedKey);

            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();

            var EncryptedKey = Convert.ToBase64String(encryptedByteArray);

            return EncryptedKey;
        }
                
    }
}