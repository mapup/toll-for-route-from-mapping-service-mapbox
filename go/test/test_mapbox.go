package main

import (
	"bytes"
	"encoding/csv"

	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"os"

	//"os"
	"time"
)
var (
	source_longitude float32
	source_latitude float32
)

// Destination Coordinates
var (
	destination_longitude float32
	destination_latitude float32
)

func readCsvFile(filePath string) [][]string {
	// Open the file
	csvfile, err := os.Open(filePath)
	if err != nil {
		log.Fatalln("Couldn't open the csv file", err)
	}

	// Parse the file
	r := csv.NewReader(csvfile)
	records, err := r.ReadAll()
	if err != nil {
		log.Fatal("Unable to parse file as CSV for " , err)
	}

	return records

}

func main()  {
	records := readCsvFile("File_Path")
	token := os.Getenv("MAPBOX_TOKEN")
	token_tollguru := os.Getenv("Tollgurukey")

	for i := 1; i < len(records); i++ {
		origin_address := records[i][1]

		url := fmt.Sprintf("https://api.mapbox.com/geocoding/v5/mapbox.places/%s.json?access_token=%s", origin_address, token)
		spaceClient := http.Client{
			Timeout: time.Second * 200, // Timeout after 15 seconds
		}
		//fmt.Println(records)
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

		Origin_longitute := result["features"].([]interface{})[0].(map[string]interface{})["geometry"].(map[string]interface{})["coordinates"].([]interface{})[0]
		Origin_latitude := result["features"].([]interface{})[0].(map[string]interface{})["geometry"].(map[string]interface{})["coordinates"].([]interface{})[1]

			destination_address := records[i][2]

			url_add := fmt.Sprintf("https://api.mapbox.com/geocoding/v5/mapbox.places/%s.json?access_token=%s", destination_address, token)


			//fmt.Println(records)
			req_add, err := http.NewRequest(http.MethodGet, url_add, nil)
			if err != nil {
				log.Fatal(err)
			}

			req_add.Header.Set("User-Agent", "spacecount-tutorial")

			res_add, getErr := spaceClient.Do(req_add)
			if getErr != nil {
				log.Fatal(getErr)
			}

			if res_add.Body != nil {
				defer res_add.Body.Close()
			}

			body_add, readErr := ioutil.ReadAll(res_add.Body)
			if readErr != nil {
				log.Fatal(readErr)
			}
			var result_add map[string]interface{}

			jsonerr := json.Unmarshal(body_add, &result_add)
			if jsonerr != nil {
				log.Fatal(result_add)
			}

			Destination_longitute := result_add["features"].([]interface{})[0].(map[string]interface{})["geometry"].(map[string]interface{})["coordinates"].([]interface{})[0]
			Destination_latitute := result_add["features"].([]interface{})[0].(map[string]interface{})["geometry"].(map[string]interface{})["coordinates"].([]interface{})[1]

				url_mapbox := fmt.Sprintf("https://api.mapbox.com/directions/v5/mapbox/driving/%v,%v;%v,%v?geometries=polyline&access_token=%s&overview=full", Origin_longitute,Origin_latitude,Destination_longitute,Destination_latitute,token)
				spaceClient1 := http.Client{
					Timeout: time.Second * 15, // Timeout after 15 seconds
				}

				req1, err := http.NewRequest(http.MethodGet, url_mapbox, nil)
				if err != nil {
					log.Fatal(err)
				}

				req1.Header.Set("User-Agent", "spacecount-tutorial")

				res1, getErr := spaceClient1.Do(req1)
				if getErr != nil {
					log.Fatal(getErr)
				}

				if res1.Body != nil {
					defer res1.Body.Close()
				}

				body1, readErr := ioutil.ReadAll(res1.Body)
				if readErr != nil {
					log.Fatal(readErr)
				}
				var result_mapbox map[string]interface{}

				jsonEr := json.Unmarshal(body1, &result_mapbox)
				if jsonEr != nil {
					log.Fatal(result)
				}

				polyline := result_mapbox["routes"].([]interface{})[0].(map[string]interface{})["geometry"].(string)


				// Tollguru API request


				url_tollguru := "https://dev.tollguru.com/v1/calc/route"


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

				var cost map[string]interface{}
				json_err := json.Unmarshal([]byte(body), &cost)
				if json_err != nil {
					log.Fatal(result)
				}

				toll := cost["route"].(map[string]interface{})["costs"].(map[string]interface{})["tag"]
				fmt.Printf("\nThe toll rate for Source Longitude: %v,  Source Latitude: %v, Destination Longitude: %v, Destination Latitude: %v is %v\n",Origin_longitute,Origin_latitude,Destination_longitute,Destination_latitute,toll)

			}}






