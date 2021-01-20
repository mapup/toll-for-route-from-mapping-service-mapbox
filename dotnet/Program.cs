using System;
using System.IO;
using System.Net;
using RestSharp;
namespace mapbox
{
    public class Program
    {
        public static string get_Response(string source_latitude,string source_longitude, string destination_latitude, string destination_longitude){
            string api_key="";
            string url="https://api.mapbox.com/directions/v5/mapbox/driving/"+source_longitude+","+source_latitude+";"+destination_longitude+","+destination_latitude+"?geometries=polyline&access_token="+api_key+"&overview=full";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            String polyline;
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                string[] output = responseFromServer.Split("\"geometry\":\"");
                string[] temp = output[1].Split("\"");
                polyline=temp[0];
            }
            response.Close();
            return(polyline);

        }
        public static string Post_Tollguru(string polyline){
            var client = new RestClient("https://dev.tollguru.com/v1/calc/route");
            var request1 = new RestRequest(Method.POST);
            request1.AddHeader("content-type", "application/json");
            request1.AddHeader("x-api-key", "");
            request1.AddParameter("application/json", "{\"source\":\"mapbox\" , \"polyline\":\""+polyline+"\" }", ParameterType.RequestBody);
            IRestResponse response1 = client.Execute(request1);        
            var content = response1.Content;
            string[] result = content.Split("tag\":");
            string[] temp1 = result[1].Split(",");
            string cost = temp1[0];
            return cost;
        }
        public static void Main()
        {
            string source_longitude="-96.7970";
            string source_latitude="32.7767";
            string destination_longitude="-74.0060";
            string destination_latitude="40.7128";
            string polyline = get_Response(source_latitude,source_longitude,destination_latitude,destination_longitude);
            Console.WriteLine(Post_Tollguru(polyline));        
    }
}
}