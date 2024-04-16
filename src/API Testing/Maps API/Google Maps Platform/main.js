// Define the initMap function
function initMap(apiKey) {
    // Start the timer
    var startTime = performance.now();

    // Create a map object and specify the DOM element for display
    var map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 51.505, lng: -0.09 },
        zoom: 1 // Set the initial zoom level
    });

    var totalPinTime = 0;
    var pinCount = 20;

    // Generate 20 random pins
    for (var i = 0; i < pinCount; i++) {
        var randomLng = (Math.random() * 360) - 180;
        var randomLat = (Math.random() * 180) - 90;

        // Start the timer for each pin
        var pinStartTime = performance.now();

        // Create a marker at the random coordinates
        new google.maps.Marker({
            position: { lat: randomLat, lng: randomLng },
            map: map
        });

        // Stop the timer for each pin
        var pinEndTime = performance.now();
        var pinTime = pinEndTime - pinStartTime;
        totalPinTime += pinTime;
        console.log("Time taken to place pin " + (i + 1) + ": " + pinTime + " milliseconds.");
    }

    console.log("The total time it took to place all pins: " + totalPinTime + " milliseconds.");
    var averagePinTime = totalPinTime / pinCount;
    console.log("Average time taken to place all pins: " + averagePinTime + " milliseconds.");
    
    // Calculate the responses per second (RPS)
    var totalTimeInSeconds = totalPinTime / 1000; // Convert total time to seconds
    var rps = pinCount / totalTimeInSeconds;
    console.log("Responses per second (RPS): " + rps);

    // Stop the timer when the map is fully loaded
    google.maps.event.addListenerOnce(map, 'tilesloaded', function () {
        var endTime = performance.now();
        console.log("Response time: " + (endTime - startTime) + " milliseconds.");
    });
}

// Fetch the API key from lifelog-config.tokens.json
async function fetchConfigAndInitMap() {
    try {
        const response = await fetch('../lifelog-config.tokens.json');
        const data = await response.json();
        const apiKey = data.LifelogTokenConfig.Maps.GoogleMaps;

        // Dynamically create a script tag to load the Google Maps API
        var script = document.createElement('script');
        script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}&callback=initMap`;
        script.defer = true;

        // Append the script tag to the document body
        document.body.appendChild(script);
    } catch (error) {
        console.error("Error fetching or parsing API key:", error);
    }
}

// Call fetchConfigAndInitMap to start the process
fetchConfigAndInitMap();
