'use strict';
import * as routeManager from '../routeManager.js'
//Get Recommendation
export function getLocationRecommendation(url, jwtToken) {
    let geturl = url;
    let request = ajaxClient.get(geturl, jwtToken)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            if (response.status != 200) {
                throw new Error(response.statusText)
            }

            return response.json()
        }).then(function (data) {
            alert('The Recommendation is successfully created.')
            if (data.output != null) {
                let cluster = data.output[0];
                let center = data.output[1];
                let radii = data.output[2];
                //let output = data.Output;s
                //location.reload()
                resolve({ cluster, center, radii });
            }

        }).catch(function (error) {
            alert(error)
            reject(error)
        })
    })
}
//Get Recommendation
export function viewLocationRecommendation(url, jwtToken) {
    let geturl = url;
    let request = ajaxClient.get(geturl, jwtToken)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            if (response.status != 200) {
                throw new Error(response.statusText)
            }

            return response.json()
        }).then(function (response) {
            alert('The Recommendation is successfully created.')
            location.reload()
            resolve(response)
        }).catch(function (error) {
            alert(error)
            reject(error)
        })
    })
}

//update log
export function updateLog(url, jwtToken) {
    let geturl = url;
    let request = ajaxClient.get(geturl, jwtToken)
    return new Promise((resolve, reject) => {
        request.then(function (response) {
            if (response.status != 200) {
                throw new Error(response.statusText)
            }
        }).catch(function (error) {
            alert(error)
            reject(error);
        });
    });
}