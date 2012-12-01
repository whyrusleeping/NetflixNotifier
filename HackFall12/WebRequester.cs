using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HackFall12.Data;

namespace HackFall12
{
    class WebRequester
    {
        public Action<string> UpdateStatus;
        private HttpClient client;
        private MovieDataItem _movie;

        public MovieDataItem Movie
        {
            get 
            {
                MovieDataItem m = _movie;
                _movie = null;
                return m;
            }
        }

        public WebRequester()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
        }

        private void updateStatus(string status)
        {
            var del = UpdateStatus;
            if (del != null)
            {
                del(status);
            }
        }

        private string makeRequestURL(string movieName)
        {
            return "";
        }

        private MovieDataItem parseMovieFromJSON(string body)
        {
            return new MovieDataItem("","","","","","",null);
        }

        public async void GetMovieByName(string name)
        {
            string requestURL = makeRequestURL(name);
            try
            {
                string responseBody;

                HttpResponseMessage response = await client.GetAsync(requestURL);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
                responseBody = responseBody.Replace("<br>", Environment.NewLine);
                _movie = parseMovieFromJSON(responseBody);
            }
            catch (Exception)
            {
                
                throw;
            }
        }


    }
}
