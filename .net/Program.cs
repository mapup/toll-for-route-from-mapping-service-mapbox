﻿using System;
using System.IO;
using System.Net;
using RestSharp;
namespace _net
{
    public class Program
    {
        public static void Main()
        {
            string api_key="pk.eyJ1Ijoic2FyYW5zaGphaW4yNDciLCJhIjoiY2tqdG5tOGhvMDcweTJxbzU5Y2JnaTRmdiJ9.Q4GOyx_yxGRsnCVu328EZQ";
            string source_longitude="-96.7970";
            string source_latitude="32.777980";
            string destination_longitude="-74.007233";
            string destination_latitude="40.713051";
            string url="https://api.mapbox.com/directions/v5/mapbox/driving/"+source_longitude+","+source_latitude+";"+destination_longitude+","+destination_latitude+"?geometries=polyline&access_token="+api_key+"&overview=full";
            //Console.WriteLine(url);
            WebRequest request = WebRequest.Create(url);
            //WebRequest request = WebRequest.Create("https://api.mapbox.com/directions/v5/mapbox/driving/-96.7970,32.7767;-74.0060,40.7128?geometries=polyline&access_token=&overview=full");
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            String polyline;
            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.           
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                string[] output = responseFromServer.Split("\"geometry\":\"");
                string[] temp = output[1].Split("\"");
                polyline=temp[0];
            }            //Console.WriteLine(polyline);
            // Close the response.
            response.Close();
/****************************Toll Guru Api*************************************************************/
        var client = new RestClient("https://dev.tollguru.com/v1/calc/here");
        var request1 = new RestRequest(Method.POST);
        request1.AddHeader("content-type", "application/json");
        request1.AddHeader("x-api-key", "R4b73j3TT9nJQhqQDQ7D6bnb6FBQfhmh");
        request1.AddParameter("application/json", "{\"source\":\"mapbox\" , \"polyline\":\""+polyline+"\" }", ParameterType.RequestBody);
        //request1.AddParameter("application/json", "{\"from\":{\"address\":\"Main str, Dallas, TX\"},\"to\":{\"address\":\"Addison, TX\"},\"waypoints\":[{\"address\":\"Plano, TX\"},{\"address\":\"Allen, TX\"}],\"vehicleType\":\"2AxlesAuto\",\"departure_time\":1551541566,\"fuelPrice\":2.79,\"fuelPriceCurrency\":\"USD\",\"fuelEfficiency\":{\"city\":24,\"hwy\":30,\"units\":\"mpg\"},\"truck\":{\"limitedWeight\":44000},\"driver\":{\"wage\":30,\"rounding\":15,\"valueOfTime\":0},\"state_mileage\":true,\"hos\":{\"rule\":60,\"dutyHoursBeforeEndOfWorkDay\":11,\"dutyHoursBeforeRestBreak\":7,\"drivingHoursBeforeEndOfWorkDay\":11,\"timeRemaining\":60}}", ParameterType.RequestBody);
        IRestResponse response1 = client.Execute(request1);        
        var content = response1.Content;
        //string[] result = content.Split("rates"); 
        Console.WriteLine(content);
    }
}
}