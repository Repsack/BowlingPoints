﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BowlingPoints
{
    /*
    This class successfully connects to the URL,
    does a REST compatible GET request
    and successfully displays the right data on the screen

    Furthermore it uses the same connection again later.
    After creating some useless data, it does a POST request and grabs the two results,
    namely the HttpStatusCode and the JSON formatted boolean.

    This file contains one public class with all the logic, 
    and 3 data classes that also have some code to show what they contain through the console.
    */

    public class BowlingTest
    {
        private const string URL = "http://13.74.31.101"; //given in the assignment.

        //This could be changed if there were other places accessible at the URL
        private string UrlParameters = "/api/points"; //Must be provided to certain method calls.

        public void BowlingBogus()
        {
            //----- PART 1: THE SETUP -----

            //These two make any http networking possible.
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));




            //----- PART 2: GETTING THE BOWLING POINTS -----

            //Here the call to GetAsync is the GET command used in terms of REST.
            HttpResponseMessage response = client.GetAsync(UrlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            BowlingPointsData bpd; //used to save data in GET, and later again to share the token.
            if (response.IsSuccessStatusCode) //the response contains info about whether the URL received the GET.
            {
                //The response should contain data according to the assignment description of the URLs behavior.
                //-So now we parse the response.
                //Any ReadAsAsync<T>().Result call must be saved to some object.
                //Make sure to add a reference to System.Net.Http.Formatting.dll in order to call ReadAsAsync()
                bpd = response.Content.ReadAsAsync<BowlingPointsData>().Result;

                //Here we just show the data contained in the custom object 'bpd'.
                bpd.WriteToConsole();
            }
            else //The response can possibly fail! The URL will stop working one day.
            {
                Console.WriteLine("GET: {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                bpd = new BowlingPointsData(); //In case of fail, this would go uninitialized, and C# cannot have that if bpd is used later!
            }
            Console.WriteLine();




            //----- PART 3: THE "ALGORITHM" ----- 

            //Creating bogus result, to send back in a POST:
            BowlingScoresData bogus = new BowlingScoresData();
            ScoreCalculator sc = new ScoreCalculator(bpd);
            //bogus.scores = sc.scores;
            bogus.token = bpd.token; //Token copied from actual correct token from the GET-request.
            bogus.scores = new List<int>();
            for (int i = -1; i >= -5; --i) //You can never knock down negative amounts of bowling pins.
            {
                bogus.scores.Add(i); 
            }
            Console.WriteLine("Created new BOGUS POST data with this info:");
            bogus.WriteToConsole();




            //----- PART 4: THE BOWLING SCORE POST -----

            //Sending bogus data back using a REST-POST command
            response = client.PostAsJsonAsync(UrlParameters, bogus).Result;
            if (response.IsSuccessStatusCode) //Just like the GET, the POST can fail
            {
                HttpStatusCode status = response.StatusCode; //is always included. Saved here for later print.
                //Note on ReadAsAsync: 
                //This call will enable you to read any data that the REST request may send back.
                //Even if you expect only a single boolean to come back as a result:
                //-you MUST be ready to grab an object that contain ALL the data in all the correct shapes.
                //So here, an object containing ONLY a single boolean,
                //will be able to catch the expected boolean result.
                BowlingSuccess bs = response.Content.ReadAsAsync<BowlingSuccess>().Result;

                //The HTTP status tells you if the token sent back in the POST actually matches a bowling-game.
                //The boolean tells you if the bowling scores for the token-game are correctly calculated.
                Console.WriteLine($"Posted BOGUS scores.\n" +
                    $"Got answers: (HTTP Status = {(int)status}) (JSON bool = {(Boolean)bs.success})");

                //3+ cases (3 where the server system visibly works):
                //1. HTTP status 400 Bad Request. Means that the token does not correspond to a played game.
                //2. HTTP status 200 OK + JSON { "success":false }. Means the token is recognized but the sums of points were badly calculated.
                //3. HTTP status 200 OK + JSON { "success":true }. Means the token is recognized and the points are correctly calculated.
                //+. HTTP errors of different kinds, and more. 
            }
            else //if the POST fails.
            {
                Console.WriteLine("POST: {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }




            //----- PART 5: THE CLEANUP -----

            //Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();
        }

        public void ScoreCalculatorTest()
        {
            List<List<int>> leest = new List<List<int>>()
            {
                new List<int>() { 1, 2 },
                new List<int>() { 3, 4 },
                new List<int>() { 5, 5 },
                new List<int>() { 6, 3 },
                new List<int>() { 7, 2 },
                new List<int>() { 10, 0 },
                new List<int>() { 9, 0 },
                new List<int>() { 1, 3 },
                new List<int>() { 1, 4 },
                new List<int>() { 0, 0 }
            };
            BowlingPointsData bopoda = new BowlingPointsData();
            bopoda.points = leest;
            bopoda.WriteToConsole();
            ScoreCalculator sc = new ScoreCalculator(bopoda);
            BowlingScoresData bsd = new BowlingScoresData();
            bsd.scores = sc.scores;
            bsd.WriteToConsole();
        }
    }

    public class BowlingPointsData //used when receiving a played game with points, using GET.
    {
        //The 2 data types needed for the GET request
        public string token { get; set; }
        public List<List<int>> points { get; set; }

        //this method will show the received data on screen
        //The DATA shape is still a match for the GET request, no matter what methods this object has too.
        internal void WriteToConsole()
        {
            Console.Write("Points: [");
            foreach (List<int> l in points)
            {
                Console.Write("[");
                foreach (int i in l)
                {
                    Console.Write(i + " ");
                }
                Console.Write("]");
            }
            Console.WriteLine("]");
            Console.WriteLine("Token: {0}", token);
        }
    }

    internal class BowlingScoresData //used when sending back the scores matching a token that matches a game, using POST.
    {
        //The 2 data types needed for the POST request
        public string token { get; set; }
        public List<int> scores { get; set; }

        internal void WriteToConsole() //This method will print the data to the screen
        {
            Console.Write("scores: [");
            foreach (int i in scores)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine("]");
            Console.WriteLine("Token: {0}", token);
        }
    }

    internal class BowlingSuccess //used when grabbing the response from a POST, to see if the POST solved the task.
    {
        public Boolean success { get; set; }
    }
}