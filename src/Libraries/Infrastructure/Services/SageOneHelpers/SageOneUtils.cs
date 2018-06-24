using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Net.Http;

namespace Infrastructure.Services.SageOneHelpers
{
    public static class SageOneUtils
    {
       
        public static void SetHeaders (HttpClient httpClient, string signature, string nonce )
        {
            // Set the required header values on the web request
            httpClient.DefaultRequestHeaders.Add("X-Signature", signature);
            httpClient.DefaultRequestHeaders.Add("X-Nonce", nonce);
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "damanojornal");
            httpClient.Timeout = new TimeSpan(0,0,100000);            
        }
 
        public static string GenerateNonce()
        {
            RandomNumberGenerator rng = RNGCryptoServiceProvider.Create();
            Byte[] output = new Byte[32];
            rng.GetBytes(output);
            return Convert.ToBase64String(output);       
        }

        public static List<KeyValuePair<string, string>> GetAccessTokenPostData(string code, string clientId, string clientSecret, string callbackUrl)
        {
            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>> {
              new KeyValuePair<string,string>("client_id", clientId),
              new KeyValuePair<string,string>("client_secret",clientSecret),
              new KeyValuePair<string,string>("code", code),
              new KeyValuePair<string,string>("grant_type", "authorization_code"),
              new KeyValuePair<string,string>("redirect_uri",HttpUtility.UrlEncode(callbackUrl))
             };
            return postData;
        }

        public static List<KeyValuePair<string, string>> GetRefreshTokenPostData(string clientId, string clientSecret, string refreshToken)
        {
            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>> {
              new KeyValuePair<string,string>("client_id", clientId),
              new KeyValuePair<string,string>("client_secret",clientSecret),
              new KeyValuePair<string,string>("refresh_token", refreshToken),
              new KeyValuePair<string,string>("grant_type", "refresh_token"),
             };
            return postData;
        }
        public static string ConvertPostParams(List<KeyValuePair<string, string>> requestBody)
        {
            if (requestBody == null)
                return "";
            IEnumerable<KeyValuePair<string, string>> kvpParams = requestBody;
            // Sort the parameters
            IEnumerable<string> sortedParams =
              from p in requestBody
              select p.Key + "=" + p.Value;

            // Add the ampersand delimiter and then URL-encode
            string encodedParams = String.Join("&", sortedParams);
            return encodedParams;

        }
    }
}