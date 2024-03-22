'use strict';

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    let jwtToken = ""

    const webServiceUrl = 'http://localhost:8086/re';

    root.myApp = root.myApp || {};

    function getRecommendations(numRecs) {
        const url = `${webServiceUrl}/getNumRecs`;
        let request = ajaxClient.post(url, {numRecs: numRecs});
    }

    function setupGetNumRecomendations() {
        let recomendationButton = document.getElementById("getRecommendation");

        recomendationButton.addEventListener('click', function () {
            let numRecs = document.getElementById("numberRecs").value;
            getRecommendations(numRecs);
        
        })
    }


    // Initialize the current view by setting up data and attaching event handlers 
    function init() {
        jwtToken = localStorage["token-local"]
        setupGetNumRecomendations();

    }

    init();

})(window, window.ajaxClient);





