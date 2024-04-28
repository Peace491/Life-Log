'use strict';

import * as routeManager from '../routeManager.js'

function deleteUser(url, values, jwtToken) {
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