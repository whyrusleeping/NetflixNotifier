﻿using System;
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
        public Action<string> UpdateStatus;
        private HttpClient client;
        private MovieDataItem _movie;
        private const string server = "125.125.125.125";
        private const string query = "/avaliable/";

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
                string link = jsonObject["link"].GetArray()[0].GetObject()["href"].GetString();
                string rating = jsonObject["average_rating"].GetString();
                string imageURL = jsonObject["box_art"].GetObject()["large"].GetString();
                string synopsis = "";

                List<string> actors = new List<string>();
                foreach (var actorPair in jsonObject["cast"].GetArray())
                {
                    actors.Add(actorPair.GetObject()["name"].GetString());
                }


                return new MovieDataItem(ID, link, Title,rating, imageURL, synopsis, actors,false);
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
            }
            catch (Exception)
            {
                updateStatus("Movie Request Failed");
                //throw;
            }
        }


    }
}
