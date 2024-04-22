'use strict';

import * as routeManager from '../routeManager.js'
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
    

    function deleteUser(userHash) {
        let request = ajaxClient.del(userManagementWebServiceUrl + `?userHash=${userHash}`, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response)
                }
                return response.json()
            }).then(function (response) {
                // Move to lli page
                alert("User Successfully Deleted")
                window.localStorage.clear()
                routeManager.loadPage(routeManager.PAGES.homePage);
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    function setupDeleteUser(userHash) {
        let deleteButton = document.getElementById('delete-user')
        deleteButton.addEventListener('click', function() {
            deleteUser(userHash)
        })
    }


    async function fetchConfig() {
        const response = await fetch('../lifelog-config.url.json');
        const data = await response.json();

        userManagementWebServiceUrl = data.LifelogUrlConfig.UserManagement.UserManagementWebService
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

            let timeAccessed = performance.now()
            routeManager.setupHeaderLinks(routeManager.PAGES.userFormPage, timeAccessed, jwtToken);
        }
    }

    init()
}
