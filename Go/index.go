package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"os"
	"time"
)

//Source Coordinates
const (
	source_longitude float64 = -99.13205
	source_latitude float64 = 19.4151
)

// Destination Coordinates
const (
	destination_longitude float64 = -98.22568
	destination_latitude float64 = 19.0806
)
func main() {

	token := os.Getenv("MAPBOX_TOKEN")

	url := fmt.Sprintf("https://api.mapbox.com/directions/v5/mapbox/driving/%v,%v;%v,%v?geometries=polyline&access_token=%s&overview=full", source_longitude,source_latitude,destination_longitude,destination_latitude,token)
	spaceClient := http.Client{
		Timeout: time.Second * 15, // Timeout after 2 seconds
	}

	req, err := http.NewRequest(http.MethodGet, url, nil)
	if err != nil {
		log.Fatal(err)
	}

	req.Header.Set("User-Agent", "spacecount-tutorial")

	res, getErr := spaceClient.Do(req)
	if getErr != nil {
		log.Fatal(getErr)
	}

	if res.Body != nil {
		defer res.Body.Close()
	}

	body, readErr := ioutil.ReadAll(res.Body)
	if readErr != nil {
		log.Fatal(readErr)
	}
	var result map[string]interface{}

	jsonErr := json.Unmarshal(body, &result)
	if jsonErr != nil {
		log.Fatal(result)
	}

	polyline := result["routes"].([]interface{})[0].(map[string]interface{})["geometry"].(string)
	fmt.Printf("%v\n\n", polyline)

	// Tollguru API request


	url_tollguru := "https://dev.tollguru.com/v1/calc/route"

	token_tollguru := os.Getenv("Tollgurukey")

	requestBody, err := json.Marshal(map[string]string{
		"source":         "mapbox",
		"polyline":       polyline,
		"vehicleType":    "2AxlesAuto",
		"departure_time": "2021-01-05T09:46:08Z",
	})

	request, err := http.NewRequest("POST", url_tollguru, bytes.NewBuffer(requestBody))
	request.Header.Set("x-api-key", token_tollguru)
	request.Header.Set("Content-Type", "application/json")

	client := &http.Client{}
	resp, err := client.Do(request)
	if err != nil {
		panic(err)
	}
	defer resp.Body.Close()

	body, error := ioutil.ReadAll(resp.Body)
	if error != nil {
		log.Fatal(err)
	}

	fmt.Println("response Body:", string(body))
}






