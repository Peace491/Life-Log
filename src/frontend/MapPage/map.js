'use strict';
import * as routeManager from '../routeManager.js'
import * as mapService from './mapServices.js'

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
export function LoadMapPage (root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    const MAX_PINS = 20

    let jwtToken = ""
    let principal = { };

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

    let apiKey = ""
   
    // Pin variables
    let pinStatus;
    var currentMap;
    let pins = { };

    // Define initMap function


    //------------------------PINS----------------------------------------

    // Show Pins
    function showPins(){
        let url = webServiceUrl + getAllPinUrl

        //Get all Pins, and render them on map
        if (Object.keys(pins).length === 0) {
            mapService.getAllPinFromUser(url, {appPrincipal: principal}, jwtToken).then( function(pinList){
                if (!pinList) return
                pinList.reverse().forEach(pin => {
                    let location = {lat: pin.latitude, lng: pin.longitude}
                    pins = location // Add the pins to the dict
                    var marker = new google.maps.Marker({
                        position: location, // Set the position of the marker to the specified coordinates
                        map: currentMap, // Set the map on which to display the marker
                        title: 'Hello World!' // Set a title for the marker
                    });
                });
            });
        }
        else
        {
            // Render the pins on the map
            for (var pin in pins)
            {
                var marker = new google.maps.Marker({
                    position: pins[pin], // Set the position of the marker to the specified coordinates
                    map: currentMap, // Set the map on which to display the marker
                    title: 'Hello World!' // Set a title for the marker
                });
            }

        }
    }
    
    //load pin
    function createPinComponent(){
        // Open modal when add-pin-button is clicked
        const addPinButton = document.getElementById('add-pin-button');
        addPinButton.addEventListener('click', () => {
            //#region  Populate the Dropdown with the LLI 
            let getLLIUrl = webServiceUrl + getAllLLIUrl
            mapService.getAllLLI(getLLIUrl, {appPrincipal: principal}, jwtToken).then(function (completedLLIList) {
                if (!completedLLIList) return
                // Get a reference to the select element
                const selectElement = document.getElementById('create-status-input');

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

            //On submit we render the pin 
            let submitButton = document.getElementById('create-pin-btn');
            submitButton.addEventListener('click' , function(){
                renderPinOnSubmit();
            })
        });
    }

    // On submit render Pin to map
    function renderPinOnSubmit(){

        let option = document.getElementById('create-status-input');
        var lliId = selectElement.options[selectElement.selectedIndex];
        let count = "";

        // Get the address
        let addressField = document.getElementById('create-paragraph-input-pin');

        let createPinUrl = webServiceUrl + createUrl
        

        // initialize the pin Status

        mapService.getPinStatus(statusUrl, {appPrincipal: principal, lliId: lliId}, jwtToken).then(function(pinStatusCollection) {
            // Iterate over the pin status collection using forEach
            // Check if pinStatusCollection is not empty
            if (pinStatusCollection.length > 0) {
                // Access the count of the first pin status object and store it
                count = pinStatusCollection[0].count;
            } else {
                return
            }
        })
        .catch(function(error) {
            // Handle any errors
            console.error('Error fetching pin status:', error);
        });

        if (parseInt(count) < MAX_PINS)
        {
            try
            {
                // Geo code the address
                let location = geocodeAddress()

                //Populate in backend
                let values = {
                    appPrincipal: principal,
                    LLIId: lliId.text, 
                    Address: addressField.textContent,
                    Latitude: location.coordinates.lat,
                    Longitude: location.coordinates.lng
                }
                mapService.createPin(createPinUrl, values, jwtToken)

                //render the pin on the map
                var marker = new google.maps.Marker({
                    position: location, // Set the position of the marker to the specified coordinates
                    map: currentMap, // Set the map on which to display the marker
                    title: 'Hello World!' // Set a title for the marker
                });

                //TODO add the pin to the pinStatus and pin
                pins[lliId.text + "z"] = location
                showPins()

            }
            catch (error){
                console.log(error)
                alert("Unable to render pin, please try again")
            }
        }
        else
        {
            alert("You have reached the maximum limit of " + MAX_PINS + " pins for this LLI");
        }
    }

    //Render tool tip
    function renderTooltipOnClick(){


    }
    //----------------------------------------------------------------------

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
            modal.classList.add('active');
            overlay.classList.add('active');
        }
    
        function closeModal(modal) {
            if (modal == null) return;
            modal.classList.remove('active');
            overlay.classList.remove('active');
        }
    }
    

    //----------------------------------------------------------------------
    
   // Assuming you have a function to create a pin
    function createPin(lli, address) {
        // Implement pin creation logic here
    }


    //Validate Pin options
    function validatePinOptions(options){
        // Input Validation

    }

    //Switch View to Location Reccomendation
    function switchView(){
        let locRecViewButton = document.getElementById('loc-rec-view')
        let interMapViewButton = document.getElementById('inter-view')

        if(!interMapViewButton.classList.contains('currentView'))
        {
            interMapViewButton.addEventListener('click', function() {
                initInteractiveMap();
                interMapViewButton.classList.add('currentView')
            })

            locRecViewButton.classList.remove('currentView')
        }

        if(!locRecViewButton.classList.contains('currentView'))
        {
            locRecViewButton.addEventListener('click', function() {
                initLocRecMap();
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

    function initMap(){
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

    function initInteractiveMap(){
        var interMap = new google.maps.Map(document.getElementById('map'), {
            center: { lat: 51.505, lng: -0.09 },
            zoom: 2 // Set the initial zoom level
        });

        currentMap = interMap;

        let interMapViewButton = document.getElementById('inter-view')
        interMapViewButton.classList.add('currentView')
    }

    function initLocRecMap(){
        var locRecMap = new google.maps.Map(document.getElementById('map'), {
            center: { lat: 53.505, lng: -0.09 },
            zoom: 4 // Set the initial zoom level
        });

        currentMap = locRecMap;

        let locRecViewButton = document.getElementById('loc-rec-view')
        locRecViewButton.classList.add('currentView')
    }

    function attachInitMapAsGloablVar() {
        window.initMap = function(apiKey) {
            initInteractiveMap();
        };
    }

    function geocodeAddress() {
        // Get the address input value
        let addressField = document.getElementById('create-paragraph-input-pin').value;

        // Create a Geocoder object
        var geocoder = new google.maps.Geocoder();

        // Make a geocoding request
        geocoder.geocode({ 'address': addressField }, function(results, status) {
            if (status === 'OK') {
                // Get the latitude and longitude from the first result
                var location = results[0].geometry.location;
                var latitude = location.lat();
                var longitude = location.lng();

                let coordinates = {lat: latitude, lng: longitude}
                
                // return the latitude and longitude
                return coordinates
            } else {
                // Handle any errors
                alert('Enter an Valid Address');
            }
        });
    }
    //-----------------------------------------------------------------------

    
    /*
    */

    
    

    // Fetch URL config
    async function fetchConfig() {
        // fetch all Url's
        const response = await fetch('../lifelog-config.url.json');
        const data = await response.json();
        webServiceUrl = data.LifelogUrlConfig.Map.MapWebservice;
        createUrl = data.LifelogUrlConfig.Map.MapPinCreate;
        getAllPinUrl = data.LifelogUrlConfig.Map.MapGetAllPin;
        getAllLLIUrl = data.LifelogUrlConfig.Map.MapGetAllLLI;
        updateUrl = data.LifelogUrlConfig.Map.MapPinUpdate;
        deleteUrl = data.LifelogUrlConfig.Map.MapPinDelete;
        viewUrl = data.LifelogUrlConfig.Map.MapPinView;
        getPinStatusUrl = data.LifelogUrlConfig.Map.MapPinStatusGet;
        editPinLLIUrl = data.LifelogUrlConfig.Map.MapPinLLIEdit;
        viewChangeUrl = data.LifelogUrlConfig.Map.MapViewUpdate;
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
            //initMap();
            //attachInitMapAsGloablVar();
            //showPins()

            window.name = routeManager.PAGES.mapPage
            // Set up event handlers
            switchView();
            createPinComponent();
            // Get data
            
            //navigate 
            routeManager.setupHeaderLinks();
        }
    }

    init()

}