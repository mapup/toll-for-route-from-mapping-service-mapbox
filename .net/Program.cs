using System;
using System.IO;
using System.Net;
using RestSharp;
namespace _net
{
    public class Program
    {
        public static void Main()
        {
            string api_key="api_key";
            string source_longitude="-96.7970";
            string source_latitude="32.777980";
            string destination_longitude="-74.007233";
            string destination_latitude="40.713051";
            string url="https://api.mapbox.com/directions/v5/mapbox/driving/"+source_longitude+","+source_latitude+";"+destination_longitude+","+destination_latitude+"?geometries=polyline&access_token="+api_key+"&overview=full";
            WebRequest request = WebRequest.Create(url);
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
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
            }
            // Close the response.
            response.Close();

/****************************Toll Guru Api*************************************************************/
        var client = new RestClient("https://dev.tollguru.com/v1/calc/route");
        var request_tollguru = new RestRequest(Method.POST);
        request_tollguru.AddHeader("content-type", "application/json");
        request_tollguru.AddHeader("x-api-key", api_key);
        request_tollguru.AddParameter("application/json", "{\"source\":\"mapbox\" , \"polyline\":\""+polyline+"\" }", ParameterType.RequestBody);
        IRestResponse response_tollguru = client.Execute(request_tollguru);        
        var content = response_tollguru.Content;
        string[] dump = content.Split("tag\":");
        string[] dump1 = dump[1].Split(",");
        //Cost variable contains the price 
        string cost = dump1[0];
        Console.WriteLine(cost);        
    }
}
}