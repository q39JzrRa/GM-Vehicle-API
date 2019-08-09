using JWT;
using JWT.Algorithms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Api.Tokens
{
    class JwtTool
    {
        public IJwtEncoder Encoder { get; private set; }

        public IJwtDecoder Decoder { get; private set; }

        byte[] _key;

        public JwtTool(string key)
        {
            _key = Encoding.ASCII.GetBytes(key);

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new SortedJsonSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            Encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            IDateTimeProvider dateTimeProvider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, dateTimeProvider);
            Decoder = new JwtDecoder(serializer, validator, urlEncoder);
        }


        public string EncodeToken(object claim)
        {
            return Encoder.Encode(claim, _key);
        }


        public string DecodeToken(string token)
        {
            return Decoder.Decode(token);
        }

        public T DecodeTokenToObject<T>(string token)
        {
            return Decoder.DecodeToObject<T>(token);
        }
    }





}
