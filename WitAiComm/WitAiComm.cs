using System.Net;

namespace Wit.Communication.WitAiComm
{
    public static class WitAiComm
    {
        private const string _token = "RVA5TZYWEKPWREXZ6LQUJC2JZD4344ZH";
        private const string _uri = "https://api.wit.ai/message?q=";

        public static Request SendRequest(string request)
        {
            using(WebClient client = new WebClient())
            {
                // Set up the header with the unique server access token.
                client.Headers.Add("Content-Type", "text");
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + _token;

                // Make a request to the wit.ai server.
                string result = client.DownloadString(_uri + request);

                return new Request { Search = request, Result = result };
            }
        }
    }
}