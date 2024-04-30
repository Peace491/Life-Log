'use strict';

import * as routeManager from '../routeManager.js'
import * as umService from './umServices.js'
import * as lfService from './lifelogReminderServices.js'
import * as log from '../Log/log.js'

export function loadUMPage(root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    let jwtToken;
    let principal = {};

    // Urls
    let userManagementWebServiceUrl = ""
    let lifelogReminderServiceUrl = ""

    function setupDeleteUser(userHash) {
        let deleteButton = document.getElementById('delete-user')
        deleteButton.addEventListener('click', function() {
            try{
                umService.deleteUser(userManagementWebServiceUrl, userHash, jwtToken)
            }
            catch (error) {
                alert(error)
            }
        })
    }
    
    function submitReminderForm(userHash) {
        const form = document.getElementById('selectionForm');
        form.addEventListener('submit', function(event) {
            event.preventDefault();
            const formData = {
                content: form.querySelector('#content').value,
                frequency: form.querySelector('#frequency').value
            };
            lfService.submitReminderForm(lifelogReminderServiceUrl, userHash, jwtToken, formData);
        });
    }


    async function fetchConfig() {
        const response = await fetch('../lifelog-config.url.json');
        const data = await response.json();

        userManagementWebServiceUrl = data.LifelogUrlConfig.UserManagement.UserManagementWebService
        lifelogReminderServiceUrl = data.LifelogUrlConfig.UserManagement.LifelogReminder.LifelogReminderWebService
    }

    async function init() {
        jwtToken = localStorage["token-local"]

        if (jwtToken == null) { // User not logged in
            routeManager.loadPage(routeManager.PAGES.homePage)
        } else {
            var userHash = JSON.parse(jwtToken).Payload.UserHash
            log.logPageAccess(userHash, routeManager.PAGES.uadPage, jwtToken)
            
            
            window.name = routeManager.PAGES.umPage
            await fetchConfig()

            setupDeleteUser(userHash)

            submitReminderForm(userHash)

            let timeAccessed = performance.now()
            routeManager.setupHeaderLinks(routeManager.PAGES.userFormPage, timeAccessed, jwtToken);
        }
    }

    init()
}
