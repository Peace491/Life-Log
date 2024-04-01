'use strict';

import * as validator from './reInputValidation.js';

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

    const webServiceUrl = 'http://localhost:8086/re'; // into config

    root.myApp = root.myApp || {};

    function getRecommendations(numRecs) {
        if (!validator.validateNumRecs(numRecs)) {
            alert("Invalid number of recommendations");
            return Promise.reject(new Error("Invalid number of recommendations"));
        }
    
        const url = `${webServiceUrl}/NumRecs`;
        return ajaxClient.post(url, {numRecs: numRecs})
            .then(response => response.json())
            .catch(error => Promise.reject(error));
    }
    


    function setupGetNumRecomendations() {
        let recomendationButton = document.getElementById("getRecommendation");

        recomendationButton.addEventListener('click', function () {
            let numRecs = document.getElementById("numberRecs").value;
            let response = getRecommendations(numRecs);
            
            response.then(function (recommendationList) {
                let recommendationContainer = document.getElementsByClassName("recommendation-container")[0];
                recommendationContainer.innerHTML = "";
                for (let recommendation of recommendationList) {
                    let lliDiv = createLLIDiv(recommendation)
                    console.log(lliDiv)
                    recommendationContainer.appendChild(lliDiv);
                }
                
            }).catch(function (error) {
                console.log(error);
            });
        
        })
    }

    function createLLIDiv(recommendation) {
        // Create the main recommendation container
        const recommendationDiv = document.createElement('div');
        recommendationDiv.className = 'lli-recommendation';

        titleDiv.appendChild(createTitleDiv(recommendation));

        categoriesDiv.appendChild(createCategoryDiv(recommendation));

        deadlineDiv.appendChild(createDeadlineInputDiv());
        
        // Append the title and categories divs to the LLI recomendations container
        recommendationDiv.appendChild(titleDiv);
        recommendationDiv.appendChild(categoriesDiv);
        return recommendationDiv;
    }
    
    function createTitleDiv(recommendation) {  
        // Create and setup the title div and its content
        const titleDiv = document.createElement('div');
        titleDiv.className = 'lli-recommendation-title';
        const titleH2 = document.createElement('h2');
        titleH2.innerText = recommendation.Title;
        return titleDiv;
    }

    function createCategoryDiv(recommendation) {
        // Create and setup the categories div and its content
        const categoriesDiv = document.createElement('div');
        categoriesDiv.className = 'lli-recommendation-categories';
        const categoriesH2 = document.createElement('h2');

        categoriesH2.innerText = recommendation.Category1;
        if (recommendation.Category2 != null && recommendation.Category2 != "") {
            categoriesH2.innerText +=  ", " + recommendation.Category2;
        }
        if (recommendation.Category3 != null && recommendation.Category3 != "") {
            categoriesH2.innerText += ", " + recommendation.Category3;
        }
        return categoriesDiv;
    }

    function createDeadlineInputDiv() {
        // Create and setup the deadline input div and its content
        const deadlineDiv = document.createElement('div');
        deadlineDiv.className = 'lli-recommendation-deadline';

        const deadlineLabel = document.createElement('label');
        deadlineLabel.for = 'deadline';
        deadlineLabel.innerText = 'Deadline: ';

        const deadlineInput = document.createElement('input');
        deadlineInput.type = 'date';
        deadlineInput.id = 'deadline';

        // Append the label and input to the deadline div
        deadlineDiv.appendChild(deadlineLabel);
        deadlineDiv.appendChild(deadlineInput);

        return deadlineDiv;
    }


    // Initialize the current view by setting up data and attaching event handlers 
    function init() {
        jwtToken = localStorage["token-local"]
        setupGetNumRecomendations();
    }

    init();

})(window, window.ajaxClient);







