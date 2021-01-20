# [Mapbox](https://www.mapbox.com/)

### Get token to access Mapbox APIs (if you have an API token skip this)
#### Step 1: Login/Signup
* Create an accont to access [Mapbox Account Dashboard](https://account.mapbox.com/)
* go to signup/login link https://account.mapbox.com/auth/signin/

#### Step 2: Creating a token
* You will be presented with a default token.
* If you want you can create an application specific token.


To get the route polyline make a GET request on "https://api.mapbox.com/directions/v5/mapbox/driving/"+source_longitude+","+source_latitude+";"+destination_longitude+","+destination_latitude+"?geometries=polyline&access_token="+api_key+"&overview=full"

### Note:
* we will be sending `geometries` as `polyline` and `overview` as `full`.
* Setting overview as full sends us complete route. Default value for `overview` is `simplified`, which is an approximate (smoothed) path of the resulting directions.
* Mapbox accepts source and destination, as semicolon seperated
  `[:longitude,:latitude]`. We will use geocoding API to convert location to latitude-longitude pair

```.net
using System;
using System.IO;
using System.Net;
using RestSharp;

# Token from mapbox
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
```

Note:

We extracted the polyline for a route from Mapbox API

We need to send this route polyline to TollGuru API to receive toll information

## [TollGuru API](https://tollguru.com/developers/docs/)

### Get key to access TollGuru polyline API
* create a dev account to receive a free key from TollGuru https://tollguru.com/developers/get-api-key
* suggest adding `vehicleType` parameter. Tolls for cars are different than trucks and therefore if `vehicleType` is not specified, may not receive accurate tolls. For example, tolls are generally higher for trucks than cars. If `vehicleType` is not specified, by default tolls are returned for 2-axle cars. 
* Similarly, `departure_time` is important for locations where tolls change based on time-of-the-day.

the last line can be changed to following
```.net

            var client = new RestClient("https://dev.tollguru.com/v1/calc/route");
            var request1 = new RestRequest(Method.POST);
            request1.AddHeader("content-type", "application/json");
            request1.AddHeader("x-api-key", "Toll Guru Api");
            request1.AddParameter("application/json", "{\"source\":\"mapmyindia\" , \"polyline\":\""+polyline+"\" }", ParameterType.RequestBody);
            IRestResponse response1 = client.Execute(request1);        
            var content = response1.Content;
            string[] result = content.Split("tag\":");
            string[] temp1 = result[1].Split(",");
            string cost = temp1[0];
            return cost;
```


Whole working code can be found in main.rb file.
