using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Wit.Communication.WitAiComm
{
    public static class WitAiComm
    {
        private const string _token = "RVA5TZYWEKPWREXZ6LQUJC2JZD4344ZH";
        private const string _textUri = "https://api.wit.ai/message?q=";
        private const string _voiceUri = "https://api.wit.ai/speech";

        public static Request SendRequest(string request)
        {
            using(WebClient client = new WebClient())
            {
                // Set up the header with the unique server access token.
                client.Headers.Add("Content-Type", "text");
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + _token;

                // Make a request to the wit.ai server.
                string result = client.DownloadString(_textUri + request);

                return new Request { Search = request, Result = result };
            }
        }

        public async static Task<Request> PostVoiceAsync(byte[] data)
        {
            Request response = new Request();

            try
            {

                await Task.Run(() =>
                {
                    using (WebClient client = new WebClient())
                    {
                        // Set up the header with the unique server access token.
                        client.Headers.Add("Content-Type", "audio/wav");
                            client.Headers[HttpRequestHeader.Authorization] = "Bearer " + _token;

                        // Make a request to the wit.ai server.
                        var result = client.UploadData(_voiceUri, data);
                        var str = System.Text.Encoding.Default.GetString(result);
                        WitAiResponse jsonResult = JsonConvert.DeserializeObject<WitAiResponse>(str);
                        response = new Request
                        {
                            Search = jsonResult._text,
                            Result = GetResultStringFromResponse(jsonResult)
                        };
                    }
                });
            }
            catch (Exception ex)
            {

            }

            return response;
        }

        private static string GetResultStringFromResponse(WitAiResponse response)
        {
            string result = "";

            result += "intent: " + response.entities?.intent?.First().value;
            result += Environment.NewLine;
            result += "on_off: " + response.entities?.on_off?.First().value;

            return result;
        }
    }
}