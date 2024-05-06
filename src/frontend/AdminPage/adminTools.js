'use strict';

import * as routeManager from '../routeManager.js'
import * as log from '../Log/log.js'

export function loadAdminToolPage(root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    let jwtToken;
    let principal = {};

    // Urls
    let logWebServiceUrl = ""
    let recoverAccountUrl = ""

    async function recoverAccount(principal, userId) {
        let recoverRequest = {
            principal: principal,
            userId: userId
        }

        let request = ajaxClient.put(recoverAccountUrl, recoverRequest, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response)
                }
                return response.json()
            }).then(function (response) {
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    async function getRecoverAccountRequests() {
        let request = ajaxClient.get(recoverAccountUrl, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error("Failed to get recover account requests")
                }
                return response.json()
            }).then(function (response) {
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    async function setupTools() {
        try {
            let response = await getRecoverAccountRequests()

            let recoverAccountRequestsContainer = document.getElementById('recover-account-requests-container')

            if (response != null && response.output != null) {
                response.output.forEach(request => {
                    let requestDiv = document.createElement('div')
                    requestDiv.innerText = request[0]
    
                    recoverAccountRequestsContainer.appendChild(requestDiv)
                });
            }

        } catch (error) {
            alert(error)
            console.error(error)
        }

        let recoverAccountInputButton = document.getElementById('recover-account-button')

        recoverAccountInputButton.addEventListener('click', async function () {
            try {
                let userId = document.getElementById('recover-account-input').value

                let response = await recoverAccount(principal, userId)
                alert("Successfully recover user account")
            } catch (error) {
                alert(error)
                console.error(error)
            }

        })
    }

    async function fetchConfig() {
        const response = await fetch('../lifelog-config.url.json');
        const data = await response.json();

        logWebServiceUrl = data.LifelogUrlConfig.Log.LogWebService
        recoverAccountUrl = data.LifelogUrlConfig.UserManagement.UserManagementWebService + data.LifelogUrlConfig.UserManagement.RecoverUser
    }

    async function init() {
        jwtToken = localStorage["token-local"]

        if (jwtToken == null) { // User not logged in
            routeManager.loadPage(routeManager.PAGES.homePage)
        } else {
            var userHash = JSON.parse(jwtToken).Payload.UserHash
            log.logPageAccess(userHash, routeManager.PAGES.adminToolPage, jwtToken)

            let jwtTokenObject = JSON.parse(jwtToken);
            principal = {
                userId: jwtTokenObject.Payload.UserHash,
                claims: jwtTokenObject.Payload.Claims,
            };
            window.name = routeManager.PAGES.adminToolPage
            await fetchConfig()

            setupTools()
            routeManager.setupHeaderLinks()
        }
    }

    init()
}
