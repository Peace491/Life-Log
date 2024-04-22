// Initialize the map
function initMap() {
    // Start the timer
    var startTime = performance.now();

    // Create a map object and specify the DOM element for display
    var map = L.map('map').setView([51.505, -0.09], 10); // Set the initial view

    // Add the OpenStreetMap tile layer
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors',
        maxZoom: 19 // Maximum zoom level
    }).addTo(map);

    // Generate random pins
    generatePins(map, startTime);
}

// Function to generate random pins
function generatePins(map, startTime) {
    var totalPinTime = 0;
    var pinCount = 20;

    // Generate 20 random pins
    for (var i = 0; i < pinCount; i++) {
        var randomLat = (Math.random() * 180) - 90;
        var randomLng = (Math.random() * 360) - 180;

        // Start the timer for each pin
        var pinStartTime = performance.now();

        // Create a marker at the random coordinates
        L.marker([randomLat, randomLng]).addTo(map);

        // Stop the timer for each pin
        var pinEndTime = performance.now();
        var pinTime = pinEndTime - pinStartTime;
        totalPinTime += pinTime;
        console.log("Time taken to place pin " + (i + 1) + ": " + pinTime + " milliseconds.");
    }

    console.log("The total time it took to place all pins: " + totalPinTime + " milliseconds.");
    var averagePinTime = totalPinTime / pinCount;
    console.log("Average time taken to place all pins: " + averagePinTime + " milliseconds.");

    // Calculate the RPS
    var endTime = performance.now();
    var totalTimeInSeconds = (endTime - startTime) / 1000; // Convert total time to seconds
    var rps = pinCount / totalTimeInSeconds;
    console.log("Number of Requests per Second (RPS): " + rps);
}

// Call the initMap function to initialize the map
initMap();
