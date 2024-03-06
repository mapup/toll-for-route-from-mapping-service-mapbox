const request = require("request");

const MAPBOX_API_KEY = process.env.MAPBOX_API_KEY
const MAPBOX_API_URL = "https://api.mapbox.com/directions/v5/mapbox/driving";

const TOLLGURU_API_KEY = process.env.TOLLGURU_API_KEY
const TOLLGURU_API_URL = "https://apis.tollguru.com/toll/v2";
const POLYLINE_ENDPOINT = "complete-polyline-from-mapping-service";

const source = { longitude: '-96.7970', latitude: '32.7767' } // Dallas, TX
const destination = { longitude: '-74.0060', latitude: '40.7128' }; // New York, NY

const url = `${MAPBOX_API_URL}/${source.longitude},${source.latitude};${destination.longitude},${destination.latitude}?geometries=polyline&access_token=${MAPBOX_API_KEY}&overview=full`

const head = arr => arr[0];
// JSON path "$..geometry"
const getGeometry = body => body.routes.map(x => x.geometry);
const getPolyline = body => head(getGeometry(JSON.parse(body)));

const getRoute = (cb) => request.get(url, cb);

//const handleRoute = (e, r, body) => console.log(getPolyline(body));
//getRoute(handleRoute);

const tollguruUrl = `${TOLLGURU_API_URL}/${POLYLINE_ENDPOINT}`;

const handleRoute = (e, r, body) => {
  console.log(body)
  const _polyline = getPolyline(body);
  request.post(
    {
      url: tollguruUrl,
      headers: {
        'content-type': 'application/json',
        'x-api-key': TOLLGURU_API_KEY
      },
      body: JSON.stringify({
        source: "mapbox",
        polyline: _polyline,
      })
    },
    (e, r, body) => {
      console.log(e);
      console.log(body)
    }
  )
}

getRoute(handleRoute);
