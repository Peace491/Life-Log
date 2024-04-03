'use strict';

import * as routeManager from '../routeManager.js'
import * as userFormService from './userFormServices.js'

export function loadUserFormPage(root, ajaxClient, userFormAction="Create") {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    let jwtToken;

    let principal = {};

    function setupSubmitUserForm() {
        let submitButton = document.getElementById('submit-ranking-button')

        submitButton.addEventListener('click', function() {
            let values = {
                principal: principal,
                mentalHealthRating: parseInt(document.getElementById('mental-health-rank').value),
                physicalHealthRating: parseInt(document.getElementById('physical-health-rank').value),
                outdoorRating: parseInt(document.getElementById('outdoor-rank').value),
                sportRating: parseInt(document.getElementById('sport-rank').value),
                artRating: parseInt(document.getElementById('art-rank').value),
                hobbyRating: parseInt(document.getElementById('hobby-rank').value),
                thrillRating: parseInt(document.getElementById('thrill-rank').value),
                travelRating: parseInt(document.getElementById('travel-rank').value),
                volunteeringRating: parseInt(document.getElementById('volunteering-rank').value),
                foodRating: parseInt(document.getElementById('food-rank').value)
            };


            if (userFormAction == "Create") {
                userFormService.createUserForm(values, jwtToken)
            }
    
            if (userFormAction == "Update") {
                userFormService.updateUserForm(values, jwtToken)
            }
        })
        
        
    }

    function init() {
        jwtToken = localStorage["token-local"]

        if (jwtToken == null) { // User not logged in
            routeManager.loadPage(routeManager.PAGES.homePage)
        } else {
            let jwtTokenObject = JSON.parse(jwtToken); 
            console.log(jwtTokenObject);
            principal = {
                userId: jwtTokenObject.Payload.UserHash,
                claims: jwtTokenObject.Payload.Claims,
            };
            console.log(principal);
            setupSubmitUserForm()
        }
    }

    init()
}
