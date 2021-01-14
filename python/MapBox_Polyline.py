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