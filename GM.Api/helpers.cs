using JWT;
using JWT.Algorithms;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace GM.Api
{
    static class helpers
    {
        public static string GenerateNonce()
        {
            //17.25 bytes = 130 bits
            //return new BigInteger(130, new SecureRandom()).toString(32);

            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            var byteArray = new byte[17];

            provider.GetBytes(byteArray);
            var nonce = Tokens.Base32.ToBase32String(byteArray);
            return nonce.ToLower().Substring(0, 26);
        }

        /// <summary>
        /// Set an HTTP header to a single value, clearing any existing values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="headerValue"></param>
        /// <param name="value"></param>
        public static void SetValue<T>(this HttpHeaderValueCollection<T> headerValue, string value) where T: class
        {
            headerValue.Clear();
            headerValue.ParseAdd(value);
        }


    }
}
