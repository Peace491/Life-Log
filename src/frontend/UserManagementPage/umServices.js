'use strict';

import * as routeManager from '../routeManager.js'

export function deleteUser(url, userHash, jwtToken) {
        let request = ajaxClient.del(url + `?userHash=${userHash}`, jwtToken)

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

export function viewPII(url, userHash, jwtToken) {
    // todo
    console.log("View PII pre request")
    let request = ajaxClient.post(url + "/ViewPII", {userHash}, jwtToken)
    console.log("View PII post request")
    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            if (response.status != 200) {
                throw new Error(response)
            }
            return response.json()
        }).then(function (response) {
            // alert usr of success
            alert("PII sent to user email!")
            resolve(response)
        }).catch(function (error) {
            reject(error)
        })
    })
}