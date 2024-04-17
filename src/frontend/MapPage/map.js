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
    let webServiceUrl = ""
    let createUrl = ""
    let getUrl = ""
    let updateUrl = ""
    let deleteUrl = ""
    let apiKey = ""

    // Define initMap function
    
    //Create Pin

    //Delete Pin

    //View Pin

    //Update Pin Location 

    //Edit LLI

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
            script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}&callback=initMap`;
            script.defer = true;
    
            // Append the script tag to the document body
            document.body.appendChild(script);
        } catch (error) {
            console.error("Error parsing API key:", error);
        }

    }

    function initInteractiveMap(){
        var map = new google.maps.Map(document.getElementById('map'), {
            center: { lat: 51.505, lng: -0.09 },
            zoom: 1 // Set the initial zoom level
        });

        let interMapViewButton = document.getElementById('inter-view')
        interMapViewButton.classList.add('currentView')
    }

    function initLocRecMap(){
        var map = new google.maps.Map(document.getElementById('map'), {
            center: { lat: 53.505, lng: -0.09 },
            zoom: 4 // Set the initial zoom level
        });

        let locRecViewButton = document.getElementById('loc-rec-view')
        locRecViewButton.classList.add('currentView')
    }

    function attachInitMapAsGloablVar() {
        window.initMap = function(apiKey) {
            initInteractiveMap();
        };
    }

    


    

    // Fetch URL config
    async function fetchConfig() {
        // fetch all Url's
        const response = await fetch('../lifelog-config.url.json');
        const data = await response.json();
        webServiceUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteWebService;
        createUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteCreate;
        getUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteGet;
        updateUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteUpdate;
        deleteUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteDelete;
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
            initMap();
            attachInitMapAsGloablVar();

            window.name = routeManager.PAGES.mapPage
            // Set up event handlers
            switchView();
            // Get data
            
            //navigate 
            routeManager.setupHeaderLinks();
        }
    }

    init()

}