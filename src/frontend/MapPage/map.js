'use strict';
import * as routeManager from '../routeManager.js'
import * as mapService from './mapServices.js'
import * as locationRecommendationService from './locationRecommendationService.js'

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
export function LoadMapPage(root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    const MAX_PINS = 20

    let jwtToken = ""
    let apiKey = ""
    let principal = {};

    // Config Variables
    let webServiceUrl = ""
    let createUrl = ""
    let getAllPinUrl = ""
    let getAllLLIUrl = ""
    let updateUrl = ""
    let deleteUrl = ""
    let viewUrl = ""
    let getPinStatusUrl = ""
    let editPinLLIUrl = ""
    let viewChangeUrl = ""
    let webServiceUrlLLI = ""
    let webServiceUrlLocRec = ""
    let getRecommendationUrl = ""
    let viewRecommendationUrl = ""
    let updateLogUrl = ""

    // Pin variables
    var currentMap;
    let pins = {};
    var markers = [];
    let geocoder;

    //------------------------PINS----------------------------------------

    // Show Pins
    function showPins() {
        let url = webServiceUrl + getAllPinUrl

        //Get all Pins, and render them on map
        if (Object.keys(pins).length === 0) {
            mapService.getAllPinFromUser(url, jwtToken).then(function (pinList) {
                if (!pinList) return
                pinList.reverse().forEach(pin => {
                    let location = { lat: parseFloat(pin.latitude), lng: parseFloat(pin.longitude) };
                    pins[pin.pinId] = location // Add the pins to the dict
                    var marker = new google.maps.Marker({
                        position: location, // Set the position of the marker to the specified coordinates
                    });
                    //add the markers to the list 
                    markers.push(marker)
                    // Set the markers 
                    setMapOnAll(currentMap)
                    // append to the array of markers
                    renderTooltipOnClick(marker, pin)
                });
            });
        }
        else {
            setMapOnAll(currentMap)
        }
    }

    // set the markers
    function setMapOnAll(map) {
        for (let i = 0; i < markers.length; i++) {
          markers[i].setMap(map);
        }

    }
      

    //load pin
    function createPinComponent() {
        // Open modal when add-pin-button is clicked
        const addPinButton = document.getElementById('add-pin-button');
        // Get a reference to the select element
        const selectElement = document.getElementById('create-status-input');

        addPinButton.addEventListener('click', () => {
            //#region  Populate the Dropdown with the LLI 
            let getLLIUrl = webServiceUrlLLI + getAllLLIUrl

            mapService.getAllLLI(getLLIUrl, jwtToken).then(function (completedLLIList) {
                if (!completedLLIList) return

                // Clear existing options
                selectElement.innerHTML = '';

                // Reverse the completedLLIList array to display the most recent LLI first
                completedLLIList.reverse().forEach(lli => {
                    // Create a new option element
                    const option = document.createElement('option');
                    // Set the text content of the option to the LLI title
                    option.textContent = lli.title;

                    // Set the value of the option to the LLI ID
                    option.value = lli.lliid;

                    // Append the option to the select element
                    selectElement.appendChild(option);
                });
            });
            //#endregion

            //Render the modal
            renderModals();
        });

        //On submit we render the pin 
        let submitButton = document.getElementById('create-pin-btn');
        submitButton.addEventListener('click', function () {
            renderPinOnSubmit(selectElement);
        })
    }

    // On submit render Pin to map
    function renderPinOnSubmit(element) {
        var lliId = element.options[element.selectedIndex];
        let count = 0;
        let statusUrl = webServiceUrl + getPinStatusUrl + lliId.value

        // initialize the pin Status

        mapService.getPinStatus(statusUrl, jwtToken).then(function (pinStatusCollection) {
            // Iterate over the pin status collection using forEach
            // Check if pinStatusCollection is not empty
            if (pinStatusCollection != null) {
                if (pinStatusCollection.length) {
                    // Access the count of the first pin status object and store it
                    count = parseInt(pinStatusCollection[0].count);
                }
            }
            else {
                count = 0
            }

        })
            .catch(function (error) {
                // Handle any errors
                console.error('Error fetching pin status:', error);
            });

        if (count < MAX_PINS) {
            try {
                // Geocode the Address and Render it 
                geocodeAddress()
                count++
            }
            catch (error) {
                alert("Unable to render pin, please try again")
            }
        }
        else {
            alert("You have reached the maximum limit of " + MAX_PINS + " pins for this LLI");
        }
    }

    //Render tool tip
    function renderTooltipOnClick(marker, pin) {
        let viewPinUrl = webServiceUrl + viewUrl + pin.pinId;

        let llititle = ""
        let llideadline = ""
        let llidescription = ""
        let llistatus = ""
        let visibility = ""
        let cost = 0
        let cat1 = ""
        let cat2 = ""
        let cat3 = ""

        mapService.viewPin(viewPinUrl, jwtToken).then(function (LLIList) {
            if (!LLIList) return
            let lli = LLIList[0]
            llititle = lli.title;
            llideadline = lli.deadline.split(" ")[0]
            llidescription = lli.description
            llistatus = lli.status
            visibility = lli.visibility
            cost = lli.cost
            cat1 = lli.category1
            cat2 = lli.category2
            cat3 = lli.category3


            let markerContent = `
            <div id="container" style="display: flex; flex-direction: column; width:100%; ">
                <div id="title-date-status-visibility-container" style="display: flex; justify-content: space-between;">
                    <div id="title-date-container" style="display: flex; justify-content: space-between; flex-grow: 2; flex-direction: column;">
                        <div id="title-input" style="font-size: large;font-weight: bold;"><h1 id="firstHeading" class="firstHeading">${llititle}</h1></div>
                        <div id="date-input">Date: ${llideadline.split("/")[2]}-${llideadline.split("/")[0]}-${llideadline.split("/")[1]}</div>
                    </div>
                    <div id="status-visibility-container" style="display: flex; justify-content: space-between; gap: 8px; padding-top: 3px;">
                        <div id="status-input">${llistatus}</div>
                        <div id="visibility-input">${visibility}</div>
                
        `;

            if (cost) {
                markerContent += `<div id="cost-input">$${cost}</div>`;
            }

            markerContent += `
                </div>
            </div>
            <div id="category-input">${cat1}, ${cat2}, ${cat3}</div>
            <div id="paragraph-input" style="padding-top: 5px;font-size: medium;">${llidescription}</div>
            <button id="delete-button-pin" style="background-color: #EBA755; color: white;font-weight: 400;border-radius: 10px; border-color: white;">Delete Pin</button>
            </div>
            `;


            // Create an InfoWindow
            var infowindow = new google.maps.InfoWindow({
                content: markerContent, // replace with your content
                maxWidth: 800
            });

            // Add a 'click' event listener to the marker
            marker.addListener('click', function () {
                // Open the InfoWindow
                infowindow.open(currentMap, marker);
                attachDeleteButtonListener(pin, marker)
            });

        });

    }

    // Function to attach event listener to delete button
    function attachDeleteButtonListener(pin, marker) {
        let deletePinBtn = document.getElementById('delete-button-pin');
        if (deletePinBtn) {
            deletePinBtn.addEventListener('click', function(){
                removePin(pin.pinId, marker);
            });
        } else {
            // Button not found in DOM, wait and try again
            setTimeout(function() {
                attachDeleteButtonListener(pin, marker);
            }, 100);
        }
    }

    function renderPin(coordinates) {
        var marker = new google.maps.Marker({
            position: { lat: latitude, lng: longitude },
            map: currentMap
        });
    }

    function renderNewPin(latitude, longitude) {

        // Get the lliId
        const selectElement = document.getElementById('create-status-input');
        var lliId = selectElement.options[selectElement.selectedIndex];

        // Get the address
        let addressField = document.getElementById('create-paragraph-input-pin');
        let createPinUrl = webServiceUrl + createUrl

        var marker = new google.maps.Marker({
            position: { lat: latitude, lng: longitude },
            map: currentMap
        });
        // Add pins to the marker list
        markers.push(marker)

        // Populate in backend
        let values = {
            AppPrincipal: principal,
            LLIId: lliId.value,
            Address: addressField.value,
            Latitude: latitude,
            Longitude: longitude
        }

        mapService.createPin(createPinUrl, values, jwtToken)

        //Add Pins
        if (pins[lliId.value]) {
            pins[lliId.value + "new"] = { lat: latitude, lng: longitude }
        }
        else {
            pins[lliId.value] = { lat: latitude, lng: longitude }
        }
    }

    function removePin(pinId, marker) {
        let url = webServiceUrl + deleteUrl + pinId
        if (pinId)
        {
            try {
                markers.splice(markers.indexOf(marker), 1);
                mapService.deletePin(url, jwtToken);
                pins[pinId] = 0
                setMapOnAll(currentMap)
                showPins()
            } catch (error) {
                alert("Unable to delete pin, try again.");
            }
        }
        else 
        {
            alert("Invalid action")
        }

    }

    //----------------------------------------------------------------------

    function getRecommendation(){
        let url = webServiceUrlLocRec + getRecommendationUrl
        try {
            locationRecommendationService.getLocationRecommendation(url, jwtToken);
            setMapOnAll(currentMap)
            showPins()
        } catch (error) {
            alert("Unable to retrieve Recommendation, try again.");
        }
    }
    function viewRecommendation(){
        let url = webServiceUrlLocRec + viewRecommendationUrl
        try {
            locationRecommendationService.viewLocationRecommendation(url, jwtToken);
            setMapOnAll(currentMap)
            showPins()
        } catch (error) {
            alert("Unable to retrieve Recommendation, try again.");
        }
    }
    function updateLog(){
        let url = webServiceUrlLocRec + updateLogUrl
        try {
            locationRecommendationService.updateLog(url, jwtToken);
            setMapOnAll(currentMap)
            showPins()
        } catch (error) {
            alert("Unable to log switching view, try again.");
        }
    }

    //-----------------------Modal------------------------------------------
    function renderModals() {
        const openModalButtons = document.querySelectorAll('[data-modal-target]');
        const overlay = document.getElementById('overlay');

        openModalButtons.forEach(button => {
            button.addEventListener('click', () => {
                const modalId = button.dataset.modalTarget;
                const modal = document.querySelector(modalId);
                openModal(modal);
            });
        });

        overlay.addEventListener('click', () => {
            const modals = document.querySelectorAll('.modal.active');
            modals.forEach(modal => {
                closeModal(modal);
            });
        });

        function openModal(modal) {
            if (modal == null) {
                return;
            }
            modal.style.visibility = 'visible';
            modal.classList.add('active');
            overlay.classList.add('active');
        }

        function closeModal(modal) {
            if (modal == null) return;
            modal.classList.remove('active');
            overlay.classList.remove('active');
            modal.style.visibility = "hidden";

        }
    }


    //----------------------------------------------------------------------
    
    //Validate Pin options
    function validatePinOptions(options) {
        // Input Validation
        //TODO
    }

    //Switch View to Location Reccomendation
    function switchView() {
        let locRecViewButton = document.getElementById('loc-rec-view')
        let interMapViewButton = document.getElementById('inter-view')

        if (!interMapViewButton.classList.contains('currentView')) {
            interMapViewButton.addEventListener('click', function () {
                initInteractiveMap();
                showPins()
                updateLog()
                interMapViewButton.classList.add('currentView')
            })

            locRecViewButton.classList.remove('currentView')
        }

        if (!locRecViewButton.classList.contains('currentView')) {
            locRecViewButton.addEventListener('click', function () {
                initLocRecMap();
                getRecommendation()
                viewRecommendation()
                showPins()
                locRecViewButton.classList.add('currentView')
            })

            interMapViewButton.classList.remove('currentView')
        }

    }


    //---------------------------API--------------------------------------
    // Fetch the API key 
    async function fetchConfigTokens() {
        try {
            const response = await fetch('../lifelog-config.tokens.json');
            const data = await response.json();
            apiKey = data.LifelogTokenConfig.Maps.GoogleMaps;


        } catch (error) {
            console.error("Error fetching:", error);
        }
    }

    function initMap() {
        try {
            // Dynamically create a script tag to load the Google Maps API
            var script = document.createElement('script');
            script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}&callback=initMap&libraries=places`;
            script.defer = true;

            // Append the script tag to the document body
            document.body.appendChild(script);
        } catch (error) {
            console.error("Error parsing API key:", error);
        }

    }

    function initInteractiveMap() {
        var interMap = new google.maps.Map(document.getElementById('map'), {
            center: { lat: 51.505, lng: -0.09 },
            zoom: 2 // Set the initial zoom level
        });

        currentMap = interMap;
        geocoder = new google.maps.Geocoder();

        let interMapViewButton = document.getElementById('inter-view')
        interMapViewButton.classList.add('currentView')
    }

    function initLocRecMap() {
        var locRecMap = new google.maps.Map(document.getElementById('map'), {
            center: { lat: 53.505, lng: -0.09 },
            zoom: 4 // Set the initial zoom level
        });

        currentMap = locRecMap;

        let locRecViewButton = document.getElementById('loc-rec-view')
        locRecViewButton.classList.add('currentView')
    }

    function attachInitMapAsGloablVar() {
        window.initMap = function (apiKey) {
            initInteractiveMap();
        };
    }

    function geocodeAddress() {
        const location = document.getElementById('create-paragraph-input-pin').value; // Example location

        // Construct the URL for the Geocoding API request
        const url = `https://maps.googleapis.com/maps/api/geocode/json?address=${encodeURIComponent(location)}&key=${apiKey}`;

        // Send the HTTP GET request and return the Promise
        return fetch(url)
            .then(response => response.json())
            .then(data => {
                // Check if the request was successful
                if (data.status === "OK") {
                    // Extract latitude and longitude from the first result
                    const latitude = data.results[0].geometry.location.lat;
                    const longitude = data.results[0].geometry.location.lng;

                    // render the pin
                    renderNewPin(latitude, longitude)

                    return
                } else {
                    // Handle error cases
                    console.error('Error:', data.status);
                    throw new Error(data.status);
                }
            })
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });
    }
    //-----------------------------------------------------------------------


    //---------------------------Config--------------------------------------
    // Fetch URL config
    async function fetchConfig() {
        // fetch all Url's
        const response = await fetch('../lifelog-config.url.json');
        const data = await response.json();
        webServiceUrl = data.LifelogUrlConfig.Map.MapWebService;
        webServiceUrlLLI = data.LifelogUrlConfig.LLI.LLIWebService;
        createUrl = data.LifelogUrlConfig.Map.MapPinCreate;
        getAllPinUrl = data.LifelogUrlConfig.Map.MapGetAllPin;
        getAllLLIUrl = data.LifelogUrlConfig.LLI.GetAllLLI;
        updateUrl = data.LifelogUrlConfig.Map.MapPinUpdate;
        deleteUrl = data.LifelogUrlConfig.Map.MapPinDelete;
        viewUrl = data.LifelogUrlConfig.Map.MapPinView;
        getPinStatusUrl = data.LifelogUrlConfig.Map.MapPinStatusGet;
        editPinLLIUrl = data.LifelogUrlConfig.Map.MapPinLLIEdit;
        viewChangeUrl = data.LifelogUrlConfig.Map.MapViewUpdate;

        webServiceUrlLocRec = data.LifelogUrlConfig.LocationRecommendation.LocRecWebService;
        getRecommendationUrl = data.LifelogUrlConfig.LocationRecommendation.GetLocationRecommendation;
        viewRecommendationUrl = data.LifelogUrlConfig.LocationRecommendation.ViewLocationRecommendation;
        updateLogUrl = data.LifelogUrlConfig.LocationRecommendation.UpdateLog;
    }

    root.myApp = root.myApp || {};

    // Initialize the current view by setting up data and attaching event handlers 
    async function init() {
        jwtToken = localStorage["token-local"]
        if (jwtToken == null) {
            routeManager.loadPage(routeManager.PAGES.homePage)
        } else {
            await fetchConfig();
            await fetchConfigTokens();
            let jwtTokenObject = JSON.parse(jwtToken);
            principal = {
                userId: jwtTokenObject.Payload.UserHash,
                claims: jwtTokenObject.Payload.Claims,
            };
            initMap();
            attachInitMapAsGloablVar();
            showPins();

            window.name = routeManager.PAGES.mapPage
            // Set up event handlers
            switchView();
            showPins();
            createPinComponent();
            // Get data

            //navigate 
            let timeAccessed = performance.now()
            routeManager.setupHeaderLinks(routeManager.PAGES.mapPage, timeAccessed, jwtToken);
        }
    }

    init()

}