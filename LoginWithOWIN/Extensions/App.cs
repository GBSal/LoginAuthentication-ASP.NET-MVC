using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace LoginWithOWIN.Extensions
{
    public static class App
    {

        /// <summary>
        /// Private member to load the application salt
        /// </summary>
        private static string _appSalt = string.Empty;


        /// <summary>
        /// Postfix attached as static salt to the end of a password hash
        /// (in addition to the dynamic hash)
        /// </summary>
        public static string PasswordEncodingPostfix = "|~";

       

        /// <summary>
        /// Returns an hashed and salted password.
        /// 
        /// Encoded Passwords end in || to indicate that they are 
        /// encoded.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="uniqueSalt">Unique per instance salt - use id</param>
        /// <param name="appSalt">Optional salt added to the encrypted value.</param>
        /// <returns></returns>
        public static string EncodePassword(this string password, string uniqueSalt)
        {
            _appSalt = ConfigurationManager.AppSettings["ApplicationSalt"].ToString();
                    
            // don't allow empty password
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            string s2 = string.IsNullOrEmpty(_appSalt) ? "$!23@1f2c9d3432!@" : _appSalt;  // app specific salt
            string s3 = uniqueSalt + password + s2;

            var sha = new SHA1CryptoServiceProvider();
            byte[] Hash = sha.ComputeHash(Encoding.ASCII.GetBytes(s3));

            var sha2 = new SHA256CryptoServiceProvider();
            Hash = sha2.ComputeHash(Hash);

            return Convert.ToBase64String(Hash).Replace("==", "") +
                // add a marker so we know whether a password is encoded 
                App.PasswordEncodingPostfix;
        }
    }
}