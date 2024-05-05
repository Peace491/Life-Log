//Create Pin
export function createPin(url, values, jwtToken) {
    let request = ajaxClient.post(url, values, jwtToken)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            if (response.status != 200) {
                throw new Error(response.statusText)
            }

            return response.json()
        }).then(function (response) {
            alert('The Pin is successfully created.')
            location.reload()
            resolve(response)
        }).catch(function (error) {
            alert(error)
            reject(error)
        })
    })
}
//Get Pin Status
export function getPinStatus(url, values, jwtToken){
    let request = ajaxClient.get(url, values, jwtToken)

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
//Get All Pins for the User
export function getAllPinFromUser(url, jwtToken){
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
//Get All LLI
export function getAllLLI(url, jwtToken) {
    let request = ajaxClient.get(url, jwtToken);

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


//Delete Pin
export function deletePin(url, jwtToken){
    let request = ajaxClient.del(url, jwtToken)

    return new Promise((resolve, reject) => {
        request.then(function (response) {
            if (response.status != 200) {
                throw new Error(response.statusText)
            }
            return response.json();
        }).then(function (data) {
            alert('The Pin is successfully deleted.')
            location.reload()
            let output = data.output;
            resolve(output);
        }).catch(function (error) {
            alert(error)
            reject(error);
        });
    });
}
//View Pin
export function viewPin(url,jwtToken){
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
//Update Pin Location 
export function updatePin(url, values, jwtToken){
    let request = ajaxClient.put(url, values, jwtToken)

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
