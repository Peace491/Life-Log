//Get Recommendation
export function getLocationRecommendation(url, jwtToken) {
    let request = ajaxClient.get(url, jwtToken)

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
//Get Recommendation
export function viewLocationRecommendation(url, jwtToken) {
    let request = ajaxClient.get(url, jwtToken)

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
export function updateLog(url, jwtToken){
    let request = ajaxClient.get(url, jwtToken)
    return new Promise((resolve, reject) => {
        request.then(function (response) {
            if (response.status != 200) {
                throw new Error(response.statusText)
            }
            return response.json();
        }).then(function (data) {
            let output = data.output;
            resolve(output);
        }).catch(function (error) {
            alert(error)
            reject(error);
        });
    });
}