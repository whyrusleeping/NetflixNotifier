using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HackFall12.Data;
using Windows.Data.Json;

namespace HackFall12
{
    public class WebRequester
    {
        public Action<string> UpdateStatusAction;
        private HttpClient client;
        private MovieDataItem _movie;
        private string server = "http://125.125.125.125";
        private const string baseMovieUrl = "http://movies.netflix.com/WiPlayer?movieid=";
        private const string query = "/available/";
        public Action RequestFinishedCallback;
        public Action<string, bool> AvailabilityCallback; 

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
        public WebRequester(string serverUrl)
        {
            server = serverUrl;
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
        }

        //Callback to update request status message
        private void updateStatus(string status)
        {
            var del = UpdateStatusAction;
            if (del != null)
            {
                del(status);
            }
        }

        //Creates a request url from a given movie name
        private string makeRequestURL(string movieName)
        {
            var cleanName = new string(movieName.ToCharArray().Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c)).ToArray());
            return server + query + cleanName;
        }

        //Creates a movie object from a JSON object
        private MovieDataItem parseMovieFromJSON(string body)
        {
            try
            {
                JsonObject jsonObject = JsonObject.Parse(body);
                string Title = jsonObject["title"].GetObject()["regular"].GetString();
                string ID = jsonObject["id"].Stringify();
                var spl = ID.Split('/');
                ID = spl[spl.Count() - 1].TrimEnd('\"');
                string link = baseMovieUrl + ID;
                string rating = jsonObject["average_rating"].GetNumber().ToString();
                string imageURL = jsonObject["box_art"].GetObject()["large"].GetString();
                string synopsis = "";
                bool avaliable = jsonObject["available"].GetBoolean();

                var actors = new List<string>();
                foreach (var actorPair in jsonObject["cast"].GetArray())
                {
                    actors.Add(actorPair.GetObject()["name"].GetString());
                }


                return new MovieDataItem(ID, link, Title,rating, imageURL, synopsis, actors,avaliable);
            }
            catch (Exception) 
            {
                updateStatus("JSON Parse failed...");
                return null;
            }
        }



        //Searches for a movie with the given name, places it in the public Movie variable for access after its done.
        public async void GetMovieByName(string name)
        {
            string requestURL = makeRequestURL(name);
            try
            {
                string responseBody;

                updateStatus("Requesting movie...");
                HttpResponseMessage response = await client.GetAsync(requestURL);
                
                response.EnsureSuccessStatusCode();
                updateStatus("Parsing Retrieved Data...");
                responseBody = await response.Content.ReadAsStringAsync();
                responseBody = responseBody.Replace("<br>", Environment.NewLine);
                _movie = parseMovieFromJSON(responseBody);
                updateStatus("Finished");
                if (RequestFinishedCallback != null)
                {
                    RequestFinishedCallback();
                }
            }
            catch (Exception)
            {
                updateStatus("Movie Request Failed");
                throw;
            }

        }

        public async void CheckAvailablilityByName(string name)
        {
            string requestURL = makeRequestURL(name);
            try
            {
                string responseBody;

                HttpResponseMessage response = await client.GetAsync(requestURL);

                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
                responseBody = responseBody.Replace("<br>", Environment.NewLine);
                MovieDataItem mov = parseMovieFromJSON(responseBody);
                if (AvailabilityCallback != null)
                {
                    AvailabilityCallback(name, mov.OnNetflix);
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
