'use strict';

import * as routeManager from '../routeManager.js'


export function submitReminderForm(url, userHash, jwtToken, formData) {
    const content = formData.content;
    const frequency = formData.frequency;
    const requestBody = JSON.stringify({ content, frequency });

    let request = ajaxClient.put(url, userHash, requestBody, jwtToken)

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
