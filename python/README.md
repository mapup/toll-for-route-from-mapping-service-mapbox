# [Mapbox](https://www.mapbox.com/)

### Get token to access Mapbox APIs (if you have an API token skip this)
#### Step 1: Login/Signup
* Create an accont to access [Mapbox Account Dashboard](https://account.mapbox.com/)
* Go to signup/login link https://account.mapbox.com/auth/signin/

#### Step 2: Creating a token
* You will be presented with a default token.
* If you want you can create an application specific token.

#### Step 3: Getting Geocodes from Mapbox 
* We will cal Mapbox Geocode API to get geocodes for our source and destination by sending a get request to https://api.mapbox.com/geocoding/v5/mapbox.places/{address}.json?types=address&access_token={token} .
* This can be done by the following code by providing an address string as parameter which will return longitude-latitude pair if there is no error in response.
* For example , get_geocode_from_mapbox("Dallas, TX") will return a float [longitude,latitude] list [-98.220024,26.03734] if there is no error in response.
```python
import json
import requests
import os

#API key for Mapbox
token=os.environ.get("MAPBOX_PUBLIC_API_KEY")

def get_geocode_from_mapbox(address):               
    address_actual=address
    address=address.replace(" ", "%20").replace(",","%2C")
    url=f'https://api.mapbox.com/geocoding/v5/mapbox.places/{address}.json?limit=1&access_token={token}'
    res=requests.get(url).json()
    try:
        return(res['features'][0]['geometry']['coordinates'])
    except:
        return((False,False))
```
#### Step 4: Getting Polyline from Mapbox
* Once you have the geocodes of source and destination , you may use function below to get the polyline from mapbox which return a polyline as "string" if there is no error in Mapbox's response.
* To get the route polyline make a GET request on https://api.mapbox.com/directions/v5/mapbox/driving/{source_longitude},{source_latitude};{destination_longitude},{destination_latitude}?geometries=polyline&access_token={token}&overview=full
* Example of GET request : https://api.mapbox.com/directions/v5/mapbox/driving/-96.7970,32.7767;-74.0060,40.7128?geometries=polyline&access_token=jk.evgggiejdjks2ZWxjbWFwdXAiLCJhIjoiY2tQ&overview=full

### Note:
* We will be sending `geometries` as `polyline` and `overview` as `full`.
* Setting overview as full sends us complete route. Default value for `overview` is `simplified`, which is an approximate (smoothed) path of the resulting directions.
* Mapbox accepts source and destination, as semicolon seperated
  `{longitude},{latitud}`.

```python
import json
import requests
import os

#API key for Mapbox
token=os.environ.get("MAPBOX_PUBLIC_API_KEY")

def get_polyline_from_mapbox(source_longitude,source_latitude,destination_longitude,destination_latitude):
    #Query Mapbox with Key and Source-Destination coordinates
    url='https://api.mapbox.com/directions/v5/mapbox/driving/{a},{b};{c},{d}?geometries=polyline&access_token={e}&overview=full'.format(a=source_longitude,b=source_latitude,c=destination_longitude,d=destination_latitude,e=token)
    #converting the response to json
    response_from_mapbox=requests.get(url,timeout=200).json()
    #checking for errors in response 
    if str(response_from_mapbox).find('message')==-1:
        #Extracting polyline if no error
        polyline_from_mapbox=response_from_mapbox["routes"][0]['geometry']
        return(polyline_from_mapbox)
    else:
        raise Exception('{}'.format(response_from_mapbox['message']))
```

#### Step 5: Calling Tollguru API and getting rates 
* We extracted the polyline for a route from Mapbox API
* We need to send this route polyline to TollGuru API to receive toll information

## [TollGuru API](https://tollguru.com/developers/docs/)

### Get key to access TollGuru polyline API
* Create a dev account to receive a free key from TollGuru https://tollguru.com/developers/get-api-key
* Suggest adding `vehicleType` parameter. Tolls for cars are different than trucks and therefore if `vehicleType` is not specified, may not receive accurate tolls. For example, tolls are generally higher for trucks than cars. If `vehicleType` is not specified, by default tolls are returned for 2-axle cars. 
* Similarly, `departure_time` is important for locations where tolls change based on time-of-the-day.

This snippet can be added at end of the above code to get rates and other details.
```python
#Importing modules
import json
import requests
import os

#API key for Tollguru
Tolls_Key = os.environ.get("TOLLGURU_API_KEY")

def get_rates_from_tollguru(polyline):
    #Tollguru querry url
    Tolls_URL = 'https://dev.tollguru.com/v1/calc/route'
    #Tollguru resquest parameters
    headers = {
                'Content-type': 'application/json',
                'x-api-key': Tolls_Key
                }
    params = {   
                #Explore https://tollguru.com/developers/docs/ to get best off all the parameter that tollguru offers 
                'source': "mapbox",
                'polyline': polyline ,               
                'vehicleType': '2AxlesAuto',                #'''Visit https://tollguru.com/developers/docs/#vehicle-types to know more options'''
                'departure_time' : "2021-01-05T09:46:08Z"   #'''Visit https://en.wikipedia.org/wiki/Unix_time to know the time format'''
                }
    #Requesting Tollguru with parameters
    response_tollguru= requests.post(Tolls_URL, json=params, headers=headers,timeout=200).json()
    #checking for errors or printing rates
    if str(response_tollguru).find('message')==-1:
        #return rates in dictionary is not error
        return(response_tollguru['route']['costs'])
    else:
        raise Exception('{}'.format(response_tollguru['message']))
```

Whole working code can be found in MapBox.py file.

## License
ISC License (ISC). Copyright 2020 &copy;TollGuru. https://tollguru.com/

Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted, provided that the above copyright notice and this permission notice appear in all copies.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.