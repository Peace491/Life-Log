mapboxgl.accessToken = "";

async function fetchConfig() {
    // fetch all tokens
    const response = await fetch('../lifelog-config.tokens.json');
    const data = await response.json();
    mapboxgl.accessToken = data.LifelogTokenConfig.Maps.Mapbox;
}

async function init() {
    await fetchConfig();

    // Start the timer
    var startTime = performance.now();

    // Fetch the map style JSON
    fetch(`https://api.mapbox.com/styles/v1/mapbox/streets-v11?access_token=${mapboxgl.accessToken}`)
        .then(response => response.json())
        .then(data => {
            // Stop the timer when the response has been received
            var endTime = performance.now();
            console.log("Response time: " + (endTime - startTime) + " milliseconds.");

            var map = new mapboxgl.Map({
                container: 'map',
                style: data, // Use the fetched map style
                center: [-0.09, 51.505], // Longitude, Latitude
                zoom: 0
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
                new mapboxgl.Marker()
                    .setLngLat([randomLng, randomLat])
                    .addTo(map);

                // Stop the timer for each pin
                var pinEndTime = performance.now();
                var pinTime = pinEndTime - pinStartTime;
                totalPinTime += pinTime;
                console.log("Time taken to place pin " + (i + 1) + ": " + pinTime + " milliseconds.");
            };
            console.log("The total time it took to place all pins: " + totalPinTime + " milliseconds.")
            var averagePinTime = totalPinTime / pinCount;
            console.log("Average time taken to place all pins: " + averagePinTime + " milliseconds.");
        });
}

init();