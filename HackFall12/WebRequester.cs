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

        //One time only getter for the retreived movie
        public MovieDataItem Movie
        {
            get 
            {
                MovieDataItem m = _movie;
                _movie = null;
                return m;
            }
        }

        //Public constructor, initializes httpclient
        public WebRequester()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
        }

        //Callback to update request status message
        private void updateStatus(string status)
        {
            var del = UpdateStatus;
            if (del != null)
            {
                del(status);
            }
        }

        //Creates a request url from a given movie name
        private string makeRequestURL(string movieName)
        {
            return "";
        }

        //Creates a movie object from a JSON object
        private MovieDataItem parseMovieFromJSON(string body)
        {
            return new MovieDataItem("","","","","","");
        }

        //Searches for a movie with the given name, places it in the public Movie variable for access after its done.
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
