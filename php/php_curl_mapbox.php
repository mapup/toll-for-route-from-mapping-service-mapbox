<?php
//using mapbox API

//Source and Destination Coordinates..
$source_longitude='-96.79448';
$source_latitude='32.78165';
$destination_longitude='-96.818';
$destination_latitude='32.95399';
$key = 'mapbox.token';

$url='https://api.mapbox.com/directions/v5/mapbox/driving/'.$source_longitude.','.$source_latitude.';'.$destination_longitude.','.$destination_latitude.'?geometries=polyline&access_token='.$key.'&overview=full';

//connection..
$mapbox = curl_init();

curl_setopt($mapbox, CURLOPT_SSL_VERIFYHOST, false);
curl_setopt($mapbox, CURLOPT_SSL_VERIFYPEER, false);

curl_setopt($mapbox, CURLOPT_URL, $url);
curl_setopt($mapbox, CURLOPT_RETURNTRANSFER, true);

//getting response from mapboxapis..
$response = curl_exec($mapbox);
$err = curl_error($mapbox);

curl_close($mapbox);

if ($err) {
	  echo "cURL Error #:" . $err;
} else {
	  echo "200 : OK\n";
}

//extracting polyline from the JSON response..
$data_mapbox = json_decode($response, true);

//polyline..
$polyline_mapbox = $data_mapbox['routes']['0']['geometry'];


//using tollguru API..
$curl = curl_init();

curl_setopt($curl, CURLOPT_SSL_VERIFYHOST, false);
curl_setopt($curl, CURLOPT_SSL_VERIFYPEER, false);


$postdata = array(
	"source" => "gmaps",
	"polyline" => $polyline_mapbox
);

//json encoding source and polyline to send as postfields..
$encode_postData = json_encode($postdata);

curl_setopt_array($curl, array(
CURLOPT_URL => "https://dev.tollguru.com/v1/calc/route",
CURLOPT_RETURNTRANSFER => true,
CURLOPT_ENCODING => "",
CURLOPT_MAXREDIRS => 10,
CURLOPT_TIMEOUT => 30,
CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
CURLOPT_CUSTOMREQUEST => "POST",


//sending mapbox polyline to tollguru
CURLOPT_POSTFIELDS => $encode_postData,
CURLOPT_HTTPHEADER => array(
				      "content-type: application/json",
				      "x-api-key: tollguru.api"),
));

$response = curl_exec($curl);
$err = curl_error($curl);

curl_close($curl);

if ($err) {
	  echo "cURL Error #:" . $err;
} else {
	  echo "200 : OK\n";
}

//response from tollguru..
$data = var_dump(json_decode($response, true));
print_r($data);
?>