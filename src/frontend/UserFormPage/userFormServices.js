
import * as routeManager from '../routeManager.js'

export function createUserForm(url, values, jwtToken) {
    let request = ajaxClient.post(url, values, jwtToken)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            return response.json()
        }).then(function (response) {
            alert('The User Form is successfully created.')
            // Move to lli page
            resolve(response)
            routeManager.loadPage(routeManager.PAGES.lliManagementPage)
        }).catch(function (error) {
            reject(error)
        })
    })
}

export function getUserFormRankings(url, principal, jwtToken) {
    let request = ajaxClient.get(url + `?userHash=${principal.userId}&role=${principal.claims["Role"]}`, jwtToken)

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

export function updateUserForm(url, values, jwtToken) {
    let request = ajaxClient.put(url, values, jwtToken)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            if (response.status != 200) {
                throw new Error(response.statusText)
            }
            return response.json()
        }).then(function (response) {
            alert('The User Form is successfully updated.')
            // Move to lli page
            resolve(response)
        }).catch(function (error) {
            reject(error)
        })
    })
}

export function getUserFormCompletionStatus(url, userHash, jwtToken) {
    let request = ajaxClient.get(url + `?UserHash=${userHash}`, jwtToken)

    return new Promise((resolve, reject) => {
        request.then(function (response) {
            return response.json();
        }).then(function (jwtToken) {
            resolve(JSON.stringify(jwtToken));
        }).catch(function (error) {
            reject(error);
        });
    });
}