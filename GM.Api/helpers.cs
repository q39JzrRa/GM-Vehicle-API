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
            var nonce = Base32.ToBase32String(byteArray);
            return nonce.ToLower().Substring(0, 26);
        }


        public static IJwtEncoder GetJwtEncoder()
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new CustomJsonSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            return new JwtEncoder(algorithm, serializer, urlEncoder);
        }

        public static IJwtDecoder GetJwtDecoder()
        {
            IJsonSerializer serializer = new CustomJsonSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IDateTimeProvider dateTimeProvider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, dateTimeProvider);
            return new JwtDecoder(serializer, validator, urlEncoder);
        }



        public static void SetValue<T>(this HttpHeaderValueCollection<T> headerValue, string value) where T: class
        {
            headerValue.Clear();
            headerValue.ParseAdd(value);
        }


    }
}
