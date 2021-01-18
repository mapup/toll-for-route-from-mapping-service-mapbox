#Importing modules
import json
import requests
import os

#API key for Mapbox
token=os.environ.get("MAPBOX_PUBLIC_API_KEY")
#API key for Tollguru
Tolls_Key = os.environ.get("TOLLGURU_API_KEY")

'''Fetching geocode from Mapbox'''  
def get_geocode_from_mapbox(address):               
    address_actual=address
    address=address.replace(" ", "%20").replace(",","%2C")
    url=f'https://api.mapbox.com/geocoding/v5/mapbox.places/{address}.json?types=address&access_token={token}'
    res=requests.get(url).json()
    try:
        return(res['features'][0]['geometry']['coordinates'])
    except:
        print(f'error in name {address_actual}')
        return((False,False))

'''Fetching Polyline from Mapbox'''
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
 
'''Calling Tollguru API'''
def get_rates_from_tollguru(polyline,count=0):
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
                # explore https://tollguru.com/developers/docs/ to get best off all the parameter that tollguru offers 
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
        raise Exception('{} in row {}'.format(response_tollguru['message'],count))

'''Program Starts'''
#Step 1 :provide source and destination location and get geocodes from mapbox for these locations
source_longitude,source_latitude= get_geocode_from_mapbox('Caraun More Ireland')
destination_longitude,destination_latitude=get_geocode_from_mapbox('Johnstown Way Enfield A83Ireland')

#Step 2 : extract polyline from mapbox 
polyline_from_mapbox=get_polyline_from_mapbox(source_longitude,source_latitude,destination_longitude,destination_latitude)

#Step 3: get rates from tollguru for that route
rates_from_tollguru=get_rates_from_tollguru(polyline_from_mapbox)

#Print the rates of all the available modes of payment
if rates_from_tollguru=={}:
    print("The route doesn't have tolls")
else:
    print(f"The rates are \n {rates_from_tollguru}")
'''Program Ends'''