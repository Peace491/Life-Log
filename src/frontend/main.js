'use strict';

import * as routeManager from './routeManager.js';
import * as userFormService from './UserFormPage/userFormServices.js'
import * as lifelogReminderService from './UserManagementPage/lifelogReminderServices.js'

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root) {
    // Dependency check
    const isValid = root;

    let jwtToken;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

     // Urls
     let userFormCompletionStatusUrl = ""
     let lifelogReminderSendUrl = ""
 
     async function fetchConfig() {
         const response = await fetch('./lifelog-config.url.json');
         const data = await response.json();
         let webServiceUrl = data.LifelogUrlConfig.UserManagement.UserForm.UserFormWebService;
         userFormCompletionStatusUrl = webServiceUrl + data.LifelogUrlConfig.UserManagement.UserForm.UserFormCompletionStatus;
        //  lifelogReminderSendUrl = data.LifelogUrlConfig.UserManagement.LifelogReminder.LifelogReminderWebService;
     }

    root.myApp = root.myApp || {};

    // Initialize the current view by setting up data and attaching event handlers 
    async function init() {
        if (localStorage.length != 0) {
            jwtToken = localStorage["token-local"]
        }

        if (!jwtToken) {
            routeManager.loadPage(routeManager.PAGES.homePage)
        } 
        else if(window.name) {
            if (window.name == routeManager.PAGES.userFormPage) {
                routeManager.loadPage(window.name, "Update")
            }
            else {
                routeManager.loadPage(window.name)
            }
        }
        else {
            await fetchConfig()

            var userHash = JSON.parse(jwtToken).Payload.UserHash; 


            var userFormIsCompleted = 'true'
            await userFormService.getUserFormCompletionStatus(userFormCompletionStatusUrl, userHash, jwtToken);

            // var lifelogReminderEmailSent = await lifelogReminderService.sendEmailToUser(lifelogReminderSendUrl, userHash, jwtToken);

            if (userFormIsCompleted == 'true') {
                routeManager.loadPage(routeManager.PAGES.lliManagementPage)
            } else {
                routeManager.loadPage(routeManager.PAGES.userFormPage, "Create")
            }
        }
    }

    init();

})(window, window.ajaxClient);




