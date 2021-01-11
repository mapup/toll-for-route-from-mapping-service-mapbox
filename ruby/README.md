# [Mapbox](https://www.mapbox.com/)

### Get token to access Mapbox APIs (if you have an API token skip this)
#### Step 1: Login/Signup
* Create an accont to access [Mapbox Account Dashboard](https://account.mapbox.com/)
* go to signup/login link https://account.mapbox.com/auth/signin/

#### Step 2: Creating a token
* You will be presented with a default token.
* If you want you can create an application specific token.


To get the route polyline make a GET request on https://api.mapbox.com/directions/v5/mapbox/driving/#{SOURCE[:longitude]},#{SOURCE[:latitude]};#{DESTINATION[:longitude]},#{DESTINATION[:latitude]}?geometries=polyline&access_token=#{TOKEN}&overview=full

### Note:
* we will be sending `geometries` as `polyline` and `overview` as `full`.
* Setting overview as full sends us complete route. Default value for `overview` is `simplified`, which is an approximate (smoothed) path of the resulting directions.
* Mapbox accepts source and destination, as semicolon seperated
  `[:longitude,:latitude]`.

```ruby
require 'HTTParty'
require 'json'


// Token from mapbox
TOKEN = ENV['MAPBOX_KEY']

MAPBOX_URL = "https://api.mapbox.com/directions/v5/mapbox/driving/#{SOURCE[:longitude]},#{SOURCE[:latitude]};#{DESTINATION[:longitude]},#{DESTINATION[:latitude]}?geometries=polyline&access_token=#{TOKEN}&overview=full"
RESPONSE = HTTParty.get(MAPBOX_URL).body
json_parsed = JSON.parse(RESPONSE)

# Extracting mapbox polyline from JSON
mapbox_polyline = json_parsed['routes'].map { |x| x['geometry'] }.pop
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
```ruby

TOLLGURU_URL = 'https://dev.tollguru.com/v1/calc/route'
TOLLGURU_KEY = ENV['TOLLGURU_KEY']
headers = {'content-type' => 'application/json', 'x-api-key' => TOLLGURU_KEY}
body = {'source' => "mapbox", 'polyline' => mapbox_polyline, 'vehicleType' => "2AxlesAuto", 'departure_time' => "2021-01-05T09:46:08Z"}
tollguru_response = HTTParty.post(TOLLGURU_URL,:body => body.to_json, :headers => headers)
```

Whole working code can be found in main.rb file.
