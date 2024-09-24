
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace BlobStorageSample
{
    internal class AccountOperations
    {
        public static async Task<HttpResponseHeaders> GetAccountInformationAsyncREST(string storageAccountName, string storageAccountKey, CancellationToken cancellationToken)
        {

            // Construct the URI. This will look like this:
            //   https://myaccount.blob.core.windows.net/resource
            String uri = string.Format("https://{0}.blob.core.windows.net/?restype=account&comp=properties", storageAccountName);

            // Set this to whatever payload you desire. Ours is null because 
            //   we're not passing anything in.
            Byte[] requestPayload = null;

            //Instantiate the request message with a null payload.
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
            { Content = (requestPayload == null) ? null : new ByteArrayContent(requestPayload) })
            {

                // Add the request headers for x-ms-date and x-ms-version.
                DateTime now = DateTime.UtcNow;
                httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                httpRequestMessage.Headers.Add("x-ms-version", "2018-03-28");
                // If you need any additional headers, add them here before creating
                //   the authorization header. 

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                   storageAccountName, storageAccountKey, now, httpRequestMessage);

                // Send the request.
                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage, cancellationToken))
                {
                    // If successful (status code = 200), 
                    //   parse the XML response for the container names.
                    Console.WriteLine("StatusCode: {0}", httpResponseMessage.StatusCode);
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return httpResponseMessage.Headers;
                    }
                }
            }

            return null;
        }
        public static async Task<string> GetServicePropertiesAsyncREST(string storageAccountName, string storageAccountKey, CancellationToken cancellationToken)
        {

            // Construct the URI. This will look like this:
            //   https://myaccount.blob.core.windows.net/resource
            String uri = string.Format("https://{0}.blob.core.windows.net?restype=service&comp=properties", storageAccountName);

            // Set this to whatever payload you desire. Ours is null because 
            //   we're not passing anything in.
            Byte[] requestPayload = null;

            //Instantiate the request message with a null payload.
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
            { Content = (requestPayload == null) ? null : new ByteArrayContent(requestPayload) })
            {

                // Add the request headers for x-ms-date and x-ms-version.
                DateTime now = DateTime.UtcNow;
                httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                httpRequestMessage.Headers.Add("x-ms-version", "2017-04-17");
                // If you need any additional headers, add them here before creating
                //   the authorization header. 

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                   storageAccountName, storageAccountKey, now, httpRequestMessage);

                // Send the request.
                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage, cancellationToken))
                {
                    // If successful (status code = 200), 
                    //   parse the XML response for the container names.
                    Console.WriteLine("StatusCode: {0}", httpResponseMessage.StatusCode);
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return await httpResponseMessage.Content.ReadAsStringAsync();                        
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// This is the method to call the REST API to retrieve a list of
        /// containers in the specific storage account.
        /// This will call CreateRESTRequest to create the request, 
        /// then check the returned status code. If it's OK (200), it will 
        /// parse the response and show the list of containers found.
        /// </summary>
        public static async Task<string> ListContainersAsyncREST(string storageAccountName, string storageAccountKey, CancellationToken cancellationToken)
        {

            // Construct the URI. This will look like this:
            //   https://myaccount.blob.core.windows.net/resource
            String uri = string.Format("https://{0}.blob.core.windows.net?comp=list", storageAccountName);

            // Set this to whatever payload you desire. Ours is null because 
            //   we're not passing anything in.
            Byte[] requestPayload = null;

            //Instantiate the request message with a null payload.
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
            { Content = (requestPayload == null) ? null : new ByteArrayContent(requestPayload) })
            {

                // Add the request headers for x-ms-date and x-ms-version.
                DateTime now = DateTime.UtcNow;
                httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                httpRequestMessage.Headers.Add("x-ms-version", "2017-04-17");
                // If you need any additional headers, add them here before creating
                //   the authorization header. 

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                   storageAccountName, storageAccountKey, now, httpRequestMessage);

                // Send the request.
                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage, cancellationToken))
                {
                    // If successful (status code = 200), 
                    //   parse the XML response for the container names.
                    Console.WriteLine("StatusCode: {0}", httpResponseMessage.StatusCode);
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return await httpResponseMessage.Content.ReadAsStringAsync();                        
                    }
                }
            }

            return null;
        }

        public static async Task<bool> CreateContainerAsyncREST(string storageAccountName, string storageAccountKey, CancellationToken cancellationToken, string newContainerName)
        {

            // Construct the URI. This will look like this:
            //   https://myaccount.blob.core.windows.net/resource
            String uri = string.Format("https://{0}.blob.core.windows.net/{1}?restype=container", storageAccountName, newContainerName);

            // Set this to whatever payload you desire. Ours is null because 
            //   we're not passing anything in.
            Byte[] requestPayload = null;

            //Instantiate the request message with a null payload.
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, uri)
            { Content = (requestPayload == null) ? null : new ByteArrayContent(requestPayload) })
            {

                // Add the request headers for x-ms-date and x-ms-version.
                DateTime now = DateTime.UtcNow;
                httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                httpRequestMessage.Headers.Add("x-ms-version", "2018-03-28");
                httpRequestMessage.Headers.Add("x-ms-meta-Name", newContainerName);
                // If you need any additional headers, add them here before creating
                //   the authorization header. 

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                   storageAccountName, storageAccountKey, now, httpRequestMessage);

                // Send the request.
                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage, cancellationToken))
                {
                    // If successful (status code = 200), 
                    //   parse the XML response for the container names.
                    Console.WriteLine("StatusCode: {0}", httpResponseMessage.StatusCode);
                    return httpResponseMessage.StatusCode == HttpStatusCode.Created;
                }
            }
        }
    }
}
