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
    let getAllNormalUserUrl = ""
    let getAllNonRootUserUrl = ""
    let updateRoleToAdminUrl = ""
    let updateRoleToNormalUrl = ""

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

    async function getAllUsers(url) {
        let request = ajaxClient.get(url, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error("Failed to get user accounts")
                }
                return response.json()
            }).then(function (response) {
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    async function updateRoleToAdmin(userId) {
        let body = {
            principal: principal,
            userId: userId
        }

        let request = ajaxClient.post(updateRoleToAdminUrl, body, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error("Failed to update role to admin")
                }
                return response.json()
            }).then(function (response) {
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    async function updateRoleToNormal(userId) {
        let body = {
            principal: principal,
            userId: userId
        }

        let request = ajaxClient.post(updateRoleToNormalUrl, body, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error("Failed to update role to normal")
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

            let getAllUserResponse
            if (principal.claims["Role"] == "Admin") {
                getAllUserResponse = await getAllUsers(getAllNormalUserUrl)
            } else if (principal.claims["Role"] == "Root") {
                getAllUserResponse = await getAllUsers(getAllNonRootUserUrl)
            }

            let userAccountList = document.getElementById('user-account-list')

            if (getAllUserResponse != null && getAllUserResponse.output != null) {
                getAllUserResponse.output.forEach(user => {
                    let requestDiv = document.createElement('div')
                    requestDiv.innerText = user[0]

                    userAccountList.appendChild(requestDiv)
                })
            }

            if (principal.claims["Role"] == "Root") {
                setupMakeAdminInput()

                let makeAdminInput = document.getElementById('make-admin-button')
                makeAdminInput.addEventListener('click', async function() {
                    let input = document.getElementById('admin-input').value
                    let makeAdminResponse = await updateRoleToAdmin(input)

                    if (makeAdminResponse.hasError == false) {
                        alert("User successfully updated to admin")
                    } else {
                        alert("Failed to update user")
                    }
                    
                })

                let removeAdminInput = document.getElementById('remove-admin-button')
                removeAdminInput.addEventListener('click', async function() {
                    let input = document.getElementById('admin-input').value
                    let removeAdminResponse = await updateRoleToNormal(input)
                    
                    if (removeAdminResponse.hasError == false) {
                        alert("User successfully updated to normal")
                    } else {
                        alert("Failed to update user")
                    }
                })


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

    function setupMakeAdminInput() {
        // Create container div
        var containerDiv = document.createElement("div");
        containerDiv.className = "make-user-admin-container";

        // Create input element
        var inputElement = document.createElement("input");
        inputElement.type = "text";
        inputElement.id = "admin-input";

        // Create button element
        var createAdminButtonElement = document.createElement("button");
        createAdminButtonElement.textContent = "Make User Admin"
        createAdminButtonElement.id = "make-admin-button";

        var removeAdminButtonElement = document.createElement("button");
        removeAdminButtonElement.textContent = "Remove admin privileges from user"
        removeAdminButtonElement.id = "remove-admin-button";

        // Append input and button elements to the container div
        containerDiv.appendChild(inputElement);
        containerDiv.appendChild(createAdminButtonElement);
        containerDiv.appendChild(removeAdminButtonElement)

        // Get reference to the main element
        var mainElement = document.querySelector("main");

        // Append the container div to the main element
        mainElement.appendChild(containerDiv);

    }

    async function fetchConfig() {
        const response = await fetch('../lifelog-config.url.json');
        const data = await response.json();

        logWebServiceUrl = data.LifelogUrlConfig.Log.LogWebService
        recoverAccountUrl = data.LifelogUrlConfig.UserManagement.UserManagementWebService + data.LifelogUrlConfig.UserManagement.RecoverUser
        getAllNonRootUserUrl = data.LifelogUrlConfig.UserManagement.UserManagementWebService + data.LifelogUrlConfig.UserManagement.GetAllNonRootUser
        getAllNormalUserUrl = data.LifelogUrlConfig.UserManagement.UserManagementWebService + data.LifelogUrlConfig.UserManagement.GetAllNormalUser
        updateRoleToAdminUrl = data.LifelogUrlConfig.UserManagement.UserManagementWebService + data.LifelogUrlConfig.UserManagement.UpdateRoleToAdmin
        updateRoleToNormalUrl = data.LifelogUrlConfig.UserManagement.UserManagementWebService + data.LifelogUrlConfig.UserManagement.UpdateRoleToNormal
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
