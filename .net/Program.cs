using System;
using System.IO;
using System.Net;

namespace mapbox
{
    public class Program
    {
        public static void Main()
        {
            // Create a request for the URL.
            string api_key="";
            string source_longitude="80.131123";
            string source_latitude="28.552413";
            string destination_longitude="77.113091";
            string destination_latitude="28.544649";
            string url="https://api.mapbox.com/directions/v5/mapbox/driving/"+source_longitude+","+source_latitude+";"+destination_longitude+","+destination_latitude+"?geometries=polyline&access_token="+api_key+"&overview=full";
            Console.WriteLine(url);
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
            }
            Console.WriteLine(polyline);
            

            // Close the response.
            response.Close();
        }
    }
}
