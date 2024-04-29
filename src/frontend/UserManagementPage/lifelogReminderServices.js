'use strict';

import * as routeManager from '../routeManager.js'


export function submitReminderForm(url, userHash, jwtToken, formData) {
    const content = formData.content;
    const frequency = formData.frequency;
    let requestBody = 
    {
        UserHash: userHash,
        Content: content,
        Frequency: frequency
    }
    //const requestBody = JSON.stringify({ content, frequency, userHash, jwtToken});

    //let request = ajaxClient.put(url, userHash, requestBody, jwtToken)
    let request = ajaxClient.put(url, requestBody, jwtToken)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            if (response.status != 200) {
                throw new Error(response)
            }
            return response.json()
        }).then(function (response) {
            // Move to lli page
            alert("User Successfully Submitted Reminder Form")
            window.localStorage.clear()
            routeManager.loadPage(routeManager.PAGES.homePage);
            resolve(response)
        }).catch(function (error) {
            reject(error)
        })
    })
}

export function sendEmailToUser(url, userHash, jwtToken) {
    let getUrl = url + "?userHash=" + userHash
    let request = ajaxClient.get(getUrl, jwtToken)
    //let request = ajaxClient.get(url, userHash, jwtToken)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            if (response.status != 200) {
                throw new Error(response)
            }
            return response.json()
        }).then(function (response) {
            // Move to lli page
            alert("Successfully Sent a Reminder to the User")
            window.localStorage.clear()
            routeManager.loadPage(routeManager.PAGES.homePage);
            resolve(response)
        }).catch(function (error) {
            reject(error)
        })
    })
}
