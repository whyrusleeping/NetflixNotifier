using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HttpTestApp
{
    public class ImdbRequester
    {
        private HttpClient client;
        private const string baseurl = "http://www.imdb.com/xml/find?xml=1&nr=1&tt=on&q=";
        public string returnString;
        public Action<string> reqCallback;

        public ImdbRequester()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
        }

        public async void getImdbUrlForTitle(string title)
        {
            string responseBody;
            try
            {
                HttpResponseMessage response = await client.GetAsync(makeRequestURL(title));

                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
                responseBody = responseBody.Replace("<br>", Environment.NewLine).Trim();
            }
            catch (Exception)
            {

                throw;
            }

            XDocument doc = XDocument.Parse(responseBody);
            returnString = doc.Descendants("ImdbEntity").ElementAt(0).Attribute("id").Value;

            if (reqCallback != null)
                reqCallback("http://www.imdb.com/title/" + returnString + "/");
        }

        public string makeRequestURL(string title)
        {
            return baseurl + title.Replace(' ', '+');
        }
    }
}