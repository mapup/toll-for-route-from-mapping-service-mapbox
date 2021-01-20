require 'HTTParty'
require 'json'
require 'cgi'

$token = ENV['MAPBOX_KEY']

def get_toll_rate(source,destination)
   
    def get_coord_array(loc)
        geocoding_url = "https://api.mapbox.com/geocoding/v5/mapbox.places/#{CGI::escape(loc)}.json?limit=1&access_token=#{$token}"
        coord = JSON.parse(HTTParty.get(geocoding_url).body)
        return  coord['features'].pop['geometry']['coordinates']
    end

    # Source Details - Using Geocoding API
    source = get_coord_array(source)
    # Destination Details - Using Geocoding API
    destination = get_coord_array(destination)

    # GET Request to Mapbox for Polyline

    mapbox_url = "https://api.mapbox.com/directions/v5/mapbox/driving/#{source[0]},#{source[1]};#{destination[0]},#{destination[1]}?geometries=polyline&access_token=#{$token}&overview=full"
    response = HTTParty.get(mapbox_url)
    begin
        if response.response.code == '200'
            json_parsed = JSON.parse(response.body)
            mapbox_polyline = json_parsed['routes'].map { |x| x['geometry'] }.pop
        else
            raise "error"
        end
    rescue
        raise "#{response.response.code} #{response.response.message}"
    end

    # Sending POST request to TollGuru
    tollguru_url = 'https://dev.tollguru.com/v1/calc/route'
    tollguru_key = ENV['TOLLGURU_KEY']
    headers = {'content-type' => 'application/json', 'x-api-key' => tollguru_key}
    body = {'source' => "mapbox", 'polyline' => mapbox_polyline, 'vehicleType' => "2AxlesAuto", 'departure_time' => "2021-01-05T09:46:08Z"}
    tollguru_response = HTTParty.post(tollguru_url,:body => body.to_json, :headers => headers)
    begin
        toll_body = JSON.parse(tollguru_response.body)    
        if toll_body["route"]["hasTolls"] == true
            return mapbox_polyline,toll_body["route"]["costs"]["tag"], toll_body["route"]["costs"]["cash"] 
        else
            raise "No tolls encountered in this route"
        end
    rescue Exception => e
        puts e.message 
    end
end