using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AppInsights.EventHub.Sink.NetCore.Telemetry.Security
{
    public class SasKeyGenerator
    {
        private static string _sasToken;

        private const int WeekLifetime = 60 * 60 * 24 * 7;

        public static string CreateSasToken(string resourceUri, string keyName, string key)
        {
            if (_sasToken != null)
            {
                return _sasToken;
            }

            var sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            
            var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + WeekLifetime);
            
            string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
            
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            _sasToken = string.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}", HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);
            
            return _sasToken;
        }
    }
}