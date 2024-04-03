const webServiceUrl = 'http://localhost:8081/userForm';

import * as routeManager from '../routeManager.js'

export function createUserForm(values, jwtToken) {
    let request = ajaxClient.post(webServiceUrl, values, jwtToken)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            return response.json()
        }).then(function (response) {
            alert('The User Form is successfully created.')
            // Move to lli page
            resolve(response)
            routeManager.loadPage(routeManager.PAGES.lliManagementPage)
        }).catch(function (error) {
            alert(error)
            reject(error)
        })
    })
}

export function updateUserForm(values, jwtToken) {
    let request = ajaxClient.put(webServiceUrl, values, jwtToken)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            return response.json()
        }).then(function (response) {
            alert('The User Form is successfully updated.')
            // Move to lli page
            resolve(response)
        }).catch(function (error) {
            alert(error)
            reject(error)
        })
    })
}

export function getUserFormCompletionStatus(jwtToken) {
    const getUrl = webServiceUrl + '/isUserFormCompleted'

    let request = ajaxClient.get(getUrl, jwtToken)

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