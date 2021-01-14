# [Mapbox](https://www.mapbox.com/)

### Get token to access Mapbox APIs (if you have an API token skip this)
#### Step 1: Login/Signup
* Create an accont to access [Mapbox Account Dashboard](https://account.mapbox.com/)
* Go to signup/login link https://account.mapbox.com/auth/signin/

#### Step 2: Creating a token
* You will be presented with a default token.
* If you want you can create an application specific token.


To get the route polyline make a GET request on https://api.mapbox.com/directions/v5/mapbox/driving/{source_longitude},{source_latitude};{destination_longitude},{destination_latitude}?geometries=polyline&access_token={token}&overview=full

Example of GET request : https://api.mapbox.com/directions/v5/mapbox/driving/-96.7970,32.7767;-74.0060,40.7128?geometries=polyline&access_token=jk.evgggiejdjks2ZWxjbWFwdXAiLCJhIjoiY2tQ&overview=full

### Note:
* We will be sending `geometries` as `polyline` and `overview` as `full`.
* Setting overview as full sends us complete route. Default value for `overview` is `simplified`, which is an approximate (smoothed) path of the resulting directions.
* Mapbox accepts source and destination, as semicolon seperated
  `{longitude},{latitud}`.

```python
#Importing modules
import json
import requests
import os

'''Fetching Polyline from Mapbox'''

#API key for Mapbox
token=os.environ.get("MAPBOX_PUBLIC_API_KEY")

#Source and Destination Coordinates
source_longitude='-96.7970'
source_latitude='32.7767'
destination_longitude='-74.0060'
destination_latitude='40.7128'

#Query Mapbox with Key and Source-Destination coordinates
url='https://api.mapbox.com/directions/v5/mapbox/driving/{a},{b};{c},{d}?geometries=polyline&access_token={e}&overview=full'.format(a=source_longitude,b=source_latitude,c=destination_longitude,d=destination_latitude,e=token)

#converting the response to json
response=requests.get(url,timeout=20).json()

#checking for errors in response 
if str(response).find('message')==-1:
    #Extracting polyline
    polyline_from_mapbox=response["routes"][0]['geometry']
else:
    raise Exception(response['message'])

```

Note:

We extracted the polyline for a route from Mapbox API

We need to send this route polyline to TollGuru API to receive toll information

## [TollGuru API](https://tollguru.com/developers/docs/)

### Get key to access TollGuru polyline API
* Create a dev account to receive a free key from TollGuru https://tollguru.com/developers/get-api-key
* Suggest adding `vehicleType` parameter. Tolls for cars are different than trucks and therefore if `vehicleType` is not specified, may not receive accurate tolls. For example, tolls are generally higher for trucks than cars. If `vehicleType` is not specified, by default tolls are returned for 2-axle cars. 
* Similarly, `departure_time` is important for locations where tolls change based on time-of-the-day.

This snippet can be added at end of the above code to get rates and other details.
```python
'''Calling Tollguru API'''

#API key for Tollguru
Tolls_Key = os.environ.get("TOLLGURU_API_KEY")

#Tollguru querry url
Tolls_URL = 'https://dev.tollguru.com/v1/calc/route'

#Tollguru resquest parameters
headers = {
            'Content-type': 'application/json',
            'x-api-key': Tolls_Key
          }
params = {   
            #Explore https://tollguru.com/developers/docs/ to get best of all the parameter that tollguru has to offer 
            'source': "mapbox",
            'polyline': polyline_from_mapbox ,               
            'vehicleType': '2AxlesAuto',                #'''Visit https://tollguru.com/developers/docs/#vehicle-types to know more the options'''
            'departure_time' : "2021-01-05T09:46:08Z"   #'''Visit https://en.wikipedia.org/wiki/Unix_time to know the time format'''
        }

#Requesting Tollguru with parameters
response_tollguru= requests.post(Tolls_URL, json=params, headers=headers,timeout=20).json()

#checking for errors or printing rates
if str(response_tollguru).find('message')==-1:
    print('\n The Rates Are ')
    #extracting rates from Tollguru response is no error
    print(*response_tollguru['route']['costs'].items(),end="\n\n")
else:
    raise Exception(response_tollguru['message'])
    
```

Whole working code can be found in MapBox_Polyline.py file.



## License
ISC License (ISC). Copyright 2020 &copy;TollGuru. https://tollguru.com/

Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted, provided that the above copyright notice and this permission notice appear in all copies.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
