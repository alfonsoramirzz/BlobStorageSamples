
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BlobStorageSample
{
    internal class AzureStorageAuthenticationHelper
    {
        private static string GetCanonicalizedHeaders(HttpRequestMessage httpRequestMessage)
        {
            var headers = from kvp in httpRequestMessage.Headers
                          where kvp.Key.StartsWith("x-ms-", StringComparison.OrdinalIgnoreCase)
                          orderby kvp.Key
                          select new { Key = kvp.Key.ToLowerInvariant(), kvp.Value };

            StringBuilder headersBuilder = new StringBuilder();

            foreach (var kvp in headers)
            {
                headersBuilder.Append(kvp.Key);
                char separator = ':';

                // Get the value for each header, strip out \r\n if found, then append it with the key.
                foreach (string headerValue in kvp.Value)
                {
                    string trimmedValue = headerValue.TrimStart().Replace("\r\n", string.Empty);
                    headersBuilder.Append(separator).Append(trimmedValue);

                    // Set this to a comma; this will only be used
                    // if there are multiple values for one of the headers.
                    separator = ',';
                }

                headersBuilder.Append("\n");
            }

            return headersBuilder.ToString();
        }

        private static string GetCanonicalizedResource(Uri address, string storageAccountName)
        {
            // The absolute path will be "/" because for we're getting a list of containers.
            StringBuilder sb = new StringBuilder("/").Append(storageAccountName).Append(address.AbsolutePath);

            // Address.Query is the resource, such as "?comp=list".
            // This ends up with a NameValueCollection with 1 entry having key=comp, value=list.
            // It will have more entries if you have more query parameters.
            NameValueCollection values = HttpUtility.ParseQueryString(address.Query);

            foreach (var item in values.AllKeys.OrderBy(k => k))
            {
                sb.Append('\n').Append(item.ToLower()).Append(':').Append(values[item]);
            }

            return sb.ToString();
        }

        internal static AuthenticationHeaderValue GetAuthorizationHeader(
        string storageAccountName, string storageAccountKey, DateTime now,
        HttpRequestMessage httpRequestMessage, string ifMatch = "", string md5 = "")
        {
            // This is the raw representation of the message signature.
            HttpMethod method = httpRequestMessage.Method;
            String MessageSignature = String.Format("{0}\n\n\n{1}\n{5}\n\n\n\n{2}\n\n\n\n{3}{4}",
                        method.ToString(),
                        (method == HttpMethod.Get || method == HttpMethod.Head || method == HttpMethod.Put) ? String.Empty
                          : httpRequestMessage.Content.Headers.ContentLength.ToString(),
                        ifMatch,
                        GetCanonicalizedHeaders(httpRequestMessage),
                        GetCanonicalizedResource(httpRequestMessage.RequestUri, storageAccountName),
                        md5);

            // Now turn it into a byte array.
            byte[] SignatureBytes = Encoding.UTF8.GetBytes(MessageSignature);

            // Create the HMACSHA256 version of the storage key.
            HMACSHA256 SHA256 = new HMACSHA256(Convert.FromBase64String(storageAccountKey));

            // Compute the hash of the SignatureBytes and convert it to a base64 string.
            string signature = Convert.ToBase64String(SHA256.ComputeHash(SignatureBytes));

            // This is the actual header that will be added to the list of request headers.
            AuthenticationHeaderValue authHV = new AuthenticationHeaderValue("SharedKey",
                storageAccountName + ":" + signature);
            return authHV;
        }
    }
}
