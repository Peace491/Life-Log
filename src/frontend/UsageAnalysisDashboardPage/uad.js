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
    let lliCountUrl = ""

    function getTopNVisitedPage(numOfPage, period) {
        let request = ajaxClient.get(topNVisitedPageUrl + `?numOfPage=${numOfPage}`, jwtToken)

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
        let request = ajaxClient.get(topNLongestPageVisitUrl + `?numOfPage=${numOfPage}`, jwtToken)

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
        let request = ajaxClient.get(loginLogsCountUrl + `?type=${type}`, jwtToken)

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
        let request = ajaxClient.get(regLogsCountUrl + `?type=${type}`, jwtToken)

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

    function getMostExpensiveLLI() {
        let request = ajaxClient.get(mostExpensiveLLIUrl, jwtToken)

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

    function getLLICount() {
        let request = ajaxClient.get(lliCountUrl, jwtToken)

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

    function createLoginStatGraph(sixMonthData, twelveMonthData, twentyFourMonthData, div, title) {
        // Get the canvas element
        var canvas = document.createElement('canvas');
        canvas.width = 600; // Adjust width as needed
        canvas.height = 400; // Adjust height as needed
        var ctx = canvas.getContext('2d');
    
        // Clear the previous content of the div
        div.innerHTML = '';
    
        // Insert canvas into the div
        div.appendChild(canvas);
    
        // Calculate the maximum data value
        var maxData = Math.max(sixMonthData, twelveMonthData, twentyFourMonthData);
    
        // Calculate the proportional height for each data point
        var proportionalHeightSix = (sixMonthData / maxData) * (canvas.height - 140); // subtracting 120 for padding
        var proportionalHeightTwelve = (twelveMonthData / maxData) * (canvas.height - 140);
        var proportionalHeightTwentyFour = (twentyFourMonthData / maxData) * (canvas.height - 140);
    
        // Your graph drawing code here
        // Draw light background
        ctx.fillStyle = '#f0f0f0'; // light gray color
        ctx.fillRect(0, 0, canvas.width, canvas.height);
    
        // Draw the graph title
        ctx.fillStyle = 'black';
        ctx.font = '20px Arial';
        ctx.fillText(title + " (By Month)", (canvas.width - ctx.measureText(title + " (By Month)").width) / 2, 30); // Centered title
    
        // For simplicity, let's just draw three bars representing the data
        var barWidth = 50;
        var barMargin = 50; // Increased spacing between bars by 10 pixels
        var startX = 100;
        var startY = canvas.height - 70; // Adjusted starting Y position
    
        // Draw bar for six month data
        ctx.fillStyle = 'blue';
        ctx.fillRect(startX, startY - proportionalHeightSix, barWidth, proportionalHeightSix);
        ctx.fillStyle = 'black';
        ctx.fillText(sixMonthData, startX + barWidth / 2 - 10, startY - proportionalHeightSix - 10); // Text inside the bar
    
        // Draw bar for twelve month data
        ctx.fillStyle = 'green';
        ctx.fillRect(startX + barWidth + barMargin, startY - proportionalHeightTwelve, barWidth, proportionalHeightTwelve);
        ctx.fillStyle = 'black';
        ctx.fillText(twelveMonthData, startX + barWidth + barMargin + barWidth / 2 - 10, startY - proportionalHeightTwelve - 10); // Text inside the bar
    
        // Draw bar for twenty-four month data
        ctx.fillStyle = 'red';
        ctx.fillRect(startX + 2 * (barWidth + barMargin), startY - proportionalHeightTwentyFour, barWidth, proportionalHeightTwentyFour);
        ctx.fillStyle = 'black';
        ctx.fillText(twentyFourMonthData, startX + 2 * (barWidth + barMargin) + barWidth / 2 - 10, startY - proportionalHeightTwentyFour - 10); // Text inside the bar
    
        // Draw the month labels
        ctx.fillStyle = 'black';
        ctx.fillText('6 Months', startX + barWidth / 2 - ctx.measureText('6 Months').width / 2, canvas.height - 40); // Label for six months
        ctx.fillText('12 Months', startX + barWidth + barMargin + barWidth / 2 - ctx.measureText('12 Months').width / 2, canvas.height - 40); // Label for twelve months
        ctx.fillText('24 Months', startX + 2 * (barWidth + barMargin) + barWidth / 2 - ctx.measureText('24 Months').width / 2, canvas.height - 40); // Label for twenty-four months
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
        let lliCount = await getLLICount()

        //document.getElementById("login-success").innerText = "Successful Login Attempts: " + successLoginCount.output[0][0];
        let successLoginDiv = document.getElementById("login-success")
        let failureLoginDiv = document.getElementById("login-failure")
        let successRegDiv = document.getElementById("reg-success")
        let failureRegDiv = document.getElementById("reg-failure")
        createLoginStatGraph(successLoginCount.output[0], successLoginCount.output[1], successLoginCount.output[2], successLoginDiv, "Successful login attempts")
        createLoginStatGraph(failureLoginCount.output[0], failureLoginCount.output[1], failureLoginCount.output[2], failureLoginDiv, "Failure login attempts")
        createLoginStatGraph(successRegCount.output[0], successRegCount.output[1], successRegCount.output[2], successRegDiv, "Successful registration attempts")
        createLoginStatGraph(failureRegCount.output[0], failureRegCount.output[1], failureRegCount.output[2], failureRegDiv, "Failure registration attempts")

        document.getElementById("longest-page-visit-6").innerText = "6 Month Page: " + longestPageVisit.output[0][0] + " ,Time: " + longestPageVisit.output[0][1] + " seconds"
        document.getElementById("longest-page-visit-12").innerText = "12 Month Page: " + longestPageVisit.output[1][0] + " ,Time: " + longestPageVisit.output[1][1] + " seconds"
        document.getElementById("longest-page-visit-24").innerText = "24 Month Page: " + longestPageVisit.output[2][0] + " ,Time: " + longestPageVisit.output[2][1] + " seconds"

        document.getElementById("most-used-feature-6").innerText = "6 Month Feature: " + mostVisitedPage.output[0][0] + " ,Num Of Visits: " + mostVisitedPage.output[0][1]
        document.getElementById("most-used-feature-12").innerText = "12 Month Feature: " + mostVisitedPage.output[1][0] + " ,Num Of Visits: " + mostVisitedPage.output[1][1]
        document.getElementById("most-used-feature-24").innerText = "24 Month Feature: " + mostVisitedPage.output[2][0] + " ,Num Of Visits: " + mostVisitedPage.output[2][1]
        
        document.getElementById("lli-count-6").innerText = "6 Month LLI Count: " + lliCount.output[0][0][0]
        document.getElementById("lli-count-12").innerText = "12 Month LLI Count: " + lliCount.output[1][0][0]
        document.getElementById("lli-count-24").innerText = "24 Month LLI Count: " + lliCount.output[2][0][0]


        document.getElementById("most-expensive-lli-6").innerText = "6 Month LLI: " + mostExpensiveLLI.output[0][0][0] + ": $" + mostExpensiveLLI.output[0][0][1]
        document.getElementById("most-expensive-lli-12").innerText = "12 Month LLI: " + mostExpensiveLLI.output[1][0][0] + ": $" + mostExpensiveLLI.output[1][0][1]
        document.getElementById("most-expensive-lli-24").innerText = "24 Month LLI: " + mostExpensiveLLI.output[2][0][0] + ": $" + mostExpensiveLLI.output[2][0][1]
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
        lliCountUrl = lliWebServiceUrl + data.LifelogUrlConfig.LLI.LLICount
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


            let timeAccessed = performance.now()
            routeManager.setupHeaderLinks(routeManager.PAGES.userFormPage, timeAccessed, jwtToken);
        }
    }

    init()
}
