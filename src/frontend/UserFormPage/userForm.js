'use strict';

import * as routeManager from '../routeManager.js'
import * as userFormService from './userFormServices.js'
import * as log from '../Log/log.js'

export function loadUserFormPage(root, ajaxClient, userFormAction = "Create") {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    let jwtToken;
    let principal = {};

    // Urls
    let webServiceUrl = ""
    let userFormCompletionStatusUrl = ""

    async function setupUserFormRanking() {
        let userFormRankingResponse
        try {
            userFormRankingResponse = await userFormService.getUserFormRankings(webServiceUrl, principal, jwtToken)
        } catch (error) {
            userFormAction = "Create"
            console.error(error)
            return
        }

        if (userFormRankingResponse == null) {
            userFormAction = "Create"
            return
        }

        let userFormRanking = userFormRankingResponse.Output[0]

        document.getElementById('mental-health-rank').value = userFormRanking.MentalHealthRating;
        document.getElementById('physical-health-rank').value = userFormRanking.PhysicalHealthRating;
        document.getElementById('outdoor-rank').value = userFormRanking.OutdoorRating;
        document.getElementById('sport-rank').value = userFormRanking.SportRating;
        document.getElementById('art-rank').value = userFormRanking.ArtRating;
        document.getElementById('hobby-rank').value = userFormRanking.HobbyRating;
        document.getElementById('thrill-rank').value = userFormRanking.ThrillRating;
        document.getElementById('travel-rank').value = userFormRanking.TravelRating;
        document.getElementById('volunteering-rank').value = userFormRanking.VolunteeringRating;
        document.getElementById('food-rank').value = userFormRanking.FoodRating;
    }

    function setupSubmitUserForm() {
        let submitButton = document.getElementById('submit-ranking-button')

        submitButton.addEventListener('click', async function () {
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
                try {
                    await userFormService.createUserForm(webServiceUrl, values, jwtToken)
                }
                catch (error) {
                    console.error(error)
                }
            }

            if (userFormAction == "Update") {
                try {
                    await userFormService.updateUserForm(webServiceUrl, values, jwtToken)
                } catch (error) {
                    console.error(error)
                }
                
            }
        })
    }

    async function fetchConfig() {
        const response = await fetch('../lifelog-config.url.json');
        const data = await response.json();
        webServiceUrl = data.LifelogUrlConfig.UserManagement.UserForm.UserFormWebService;
        userFormCompletionStatusUrl = webServiceUrl + data.LifelogUrlConfig.UserManagement.UserForm.UserFormCompletionStatus;
    }

    async function init() {
        jwtToken = localStorage["token-local"]

        if (jwtToken == null) { // User not logged in
            routeManager.loadPage(routeManager.PAGES.homePage)
        } else {
            var userHash = JSON.parse(jwtToken).Payload.UserHash
            log.logPageAccess(userHash, routeManager.PAGES.userFormPage, jwtToken)

            let jwtTokenObject = JSON.parse(jwtToken);
            principal = {
                userId: jwtTokenObject.Payload.UserHash,
                claims: jwtTokenObject.Payload.Claims,
            };

            window.name = routeManager.PAGES.userFormPage
            await fetchConfig()
            if (userFormAction == "Update") {
                await setupUserFormRanking()    
            }
            setupSubmitUserForm()

            let timeAccessed = performance.now()
            routeManager.setupHeaderLinks(routeManager.PAGES.userFormPage, timeAccessed, jwtToken);
        }
    }

    init()
}
