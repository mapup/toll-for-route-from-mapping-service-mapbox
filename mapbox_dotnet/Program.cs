using System;    
using System.Collections.Generic;    
using System.Linq;  
using System.Text;    
using System.Threading.Tasks;    
using System.Net.Http;    
using System.Net.Http.Headers;     
namespace HttpClientAPP     
{        
    class Program      
    {
        static void Main(string[] args)    
        {    
            HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("https://docs.mapbox.com/api/overview/");    
            client.BaseAddress = new Uri("https://api.mapbox.com/directions/v5/mapbox/driving/-96.7970,32.7767;-74.0060,40.7128?geometries=polyline&access_token=pk.eyJ1IjoicGF2ZWxjbWFwdXAiLCJhIjoiY2tqbjQwMzRvMTJlajMwbWp3cXgxYzF1byJ9.rp9od-WNfqkI39hdKXy1_Q&overview=full");     
            // Add an Accept header for JSON format.    
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));     
            // List all Names.    
            HttpResponseMessage response = client.GetAsync("api/Values").Result;  // Blocking call!    
            var data = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(data);
        }    
    }    
}  