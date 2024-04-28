'use strict';

import * as routeManager from '../routeManager.js'
import * as log from '../Log/log.js'

export function loadUADPage(root, ajaxClient) {
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
    let topNVisitedPageUrl = ""
    let topNLongestPageVisitUrl = ""
    let loginLogsCountUrl = ""
    let regLogsCountUrl = ""
    let lliWebServiceUrl = ""
    let mostCommonLLICategoryUrl = ""
    let mostExpensiveLLIUrl = ""

    function getTopNVisitedPage(numOfPage, period) {
        let request = ajaxClient.get(topNVisitedPageUrl + `?numOfPage=${numOfPage}&periodInMonth=${period}`, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response)
                }
                return response.json()
            }).then(function (response) {
                // Move to lli page
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    function getTopNLongestPageVisit(numOfPage, period) {
        let request = ajaxClient.get(topNLongestPageVisitUrl + `?numOfPage=${numOfPage}&periodInMonth=${period}`, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response)
                }
                return response.json()
            }).then(function (response) {
                // Move to lli page
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    function getLoginLogsCount(type, period) {
        let request = ajaxClient.get(loginLogsCountUrl + `?type=${type}&period=${period}`, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response)
                }
                return response.json()
            }).then(function (response) {
                // Move to lli page
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    function getRegLogsCount(type, period) {
        let request = ajaxClient.get(regLogsCountUrl + `?type=${type}&period=${period}`, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response)
                }
                return response.json()
            }).then(function (response) {
                // Move to lli page
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    function getMostCommonLLICategory(period) {
        let request = ajaxClient.get(mostCommonLLICategoryUrl + `?period=${period}`, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response)
                }
                return response.json()
            }).then(function (response) {
                // Move to lli page
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    function getMostExpensiveLLI(period) {
        let request = ajaxClient.get(mostExpensiveLLIUrl + `?period=${period}`, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response)
                }
                return response.json()
            }).then(function (response) {
                // Move to lli page
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    async function setupData(period) {
        let mostVisitedPage = await getTopNVisitedPage(1, period)
        let longestPageVisit = await getTopNLongestPageVisit(1, period)
        let successLoginCount = await getLoginLogsCount("Success", period)
        let failureLoginCount = await getLoginLogsCount("Failure", period)
        let successRegCount = await getRegLogsCount("Success", period)
        let failureRegCount = await getRegLogsCount("Failure", period)
        let mostCommonLLICategory = await getMostCommonLLICategory(period)
        let mostExpensiveLLI = await getMostExpensiveLLI(period)

        document.getElementById("login-success").innerText = "Successful Login Attempts: " + successLoginCount.output[0][0];
        document.getElementById("login-failure").innerText = "Failure Login Attempts: " + failureLoginCount.output[0][0];
        document.getElementById("reg-success").innerText = "Successful Registration Attempts: " + successRegCount.output[0][0];
        document.getElementById("reg-failure").innerText = "Failure Registration Attempts: " + failureRegCount.output[0][0];
        document.getElementById("longest-page-visit").innerText = "Longest Page Visit: " + longestPageVisit.output[0][0];
        document.getElementById("most-used-feature").innerText = "Most Used Feature: " + mostVisitedPage.output[0][0];
        document.getElementById("most-common-category").innerText = "Most Common LLI Category: " + mostCommonLLICategory.output[0][0]
        document.getElementById("most-expensive-lli").innerHTML = "Most Expensive LLI: " + mostExpensiveLLI.output[0][0] + ": $" + mostExpensiveLLI.output[0][1]
    }

    function setupPeriodSelect() {
        let select = document.getElementById('period')
        select.addEventListener('click', function() {
            let period = select.value
            setupData(period)
        })

    }

    async function fetchConfig() {
        const response = await fetch('../lifelog-config.url.json');
        const data = await response.json();

        logWebServiceUrl = data.LifelogUrlConfig.Log.LogWebService
        topNVisitedPageUrl = logWebServiceUrl + data.LifelogUrlConfig.Log.TopNVisitedPage
        topNLongestPageVisitUrl = logWebServiceUrl + data.LifelogUrlConfig.Log.TopNLongestPageVisit
        loginLogsCountUrl = logWebServiceUrl + data.LifelogUrlConfig.Log.LoginLogsCount
        regLogsCountUrl = logWebServiceUrl + data.LifelogUrlConfig.Log.RegLogsCount

        lliWebServiceUrl = data.LifelogUrlConfig.LLI.LLIWebService
        mostCommonLLICategoryUrl = lliWebServiceUrl + data.LifelogUrlConfig.LLI.MostCommonLLICategory
        mostExpensiveLLIUrl = lliWebServiceUrl + data.LifelogUrlConfig.LLI.MostExpensiveLLI
    }

    async function init() {
        jwtToken = localStorage["token-local"]

        if (jwtToken == null) { // User not logged in
            routeManager.loadPage(routeManager.PAGES.homePage)
        } else {
            var userHash = JSON.parse(jwtToken).Payload.UserHash
            log.logPageAccess(userHash, routeManager.PAGES.uadPage, jwtToken)

            let jwtTokenObject = JSON.parse(jwtToken);
            principal = {
                userId: jwtTokenObject.Payload.UserHash,
                claims: jwtTokenObject.Payload.Claims,
            };
            window.name = routeManager.PAGES.uadPage
            await fetchConfig()

            setupData(6)

            setupPeriodSelect()

            let timeAccessed = performance.now()
            routeManager.setupHeaderLinks(routeManager.PAGES.userFormPage, timeAccessed, jwtToken);
        }
    }

    init()
}
