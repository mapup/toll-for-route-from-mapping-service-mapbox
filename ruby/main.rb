require 'HTTParty'
require 'json'
require 'cgi'
TOKEN = ENV['MAPBOX_KEY']

def get_coord_array(loc)
    geocoding_url = "https://api.mapbox.com/geocoding/v5/mapbox.places/#{CGI::escape(loc)}.json?limit=1&access_token=#{TOKEN}"
    coord = JSON.parse(HTTParty.get(geocoding_url).body)
    return  coord['features'].pop['geometry']['coordinates']
end

# Source Details - Using Geocoding API
SOURCE = get_coord_array("Dallas, TX")
# Destination Details - Using Geocoding API
DESTINATION = get_coord_array("New York, NY")

# GET Request to Mapbox for Polyline

MAPBOX_URL = "https://api.mapbox.com/directions/v5/mapbox/driving/#{SOURCE[0]},#{SOURCE[1]};#{DESTINATION[0]},#{DESTINATION[1]}?geometries=polyline&access_token=#{TOKEN}&overview=full"
RESPONSE = HTTParty.get(MAPBOX_URL).body
json_parsed = JSON.parse(RESPONSE)

# Extracting mapbox polyline from JSON
mapbox_polyline = json_parsed['routes'].map { |x| x['geometry'] }.pop

# Sending POST request to TollGuru
TOLLGURU_URL = 'https://dev.tollguru.com/v1/calc/route'
TOLLGURU_KEY = ENV['TOLLGURU_KEY']
headers = {'content-type' => 'application/json', 'x-api-key' => TOLLGURU_KEY}
body = {'source' => "mapbox", 'polyline' => mapbox_polyline, 'vehicleType' => "2AxlesAuto", 'departure_time' => "2021-01-05T09:46:08Z"}
tollguru_response = HTTParty.post(TOLLGURU_URL,:body => body.to_json, :headers => headers)