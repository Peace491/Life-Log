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
    console.log(url)
    console.log(jwtToken)
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
export function deletePin(url, values, jwtToken){
    let request = ajaxClient.delete(url, values, jwtToken)

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
//View Pin
export function viewPin(url,jwtToken){
    let request = ajaxClient.get(url, jwtToken)
    console.log("works")
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
//Edit LLI
export function editLLI(url) {

    let titleElement = document.getElementById("edit-title-input")
    let llititle = titleElement.textContent

    let deadlineElement = document.getElementById("edit-date-input")
    let llideadline = deadlineElement.value


    let getLLIID = document.getElementById(`insert-llievent-${llideadline.split("-")[2]}`).querySelector(".lli-btn.event");
    let lliid = getLLIID.id.split("-")[2]


    let descriptionElement = document.getElementById("edit-paragraph-input")
    let llidescription = descriptionElement.textContent

    let statusElement = document.getElementById("edit-status-input")
    let llistatus = statusElement.value

    let visibilityElement = document.getElementById("edit-visibility-input")
    let llivisibility = visibilityElement.value

    let costElement = document.getElementById("edit-cost-input")
    let llicost = 100
    if (costElement) {
        llicost = costElement.textContent
    }
    // else{
    //     alert('unable to update cost')
    // }


    let categoryElement = document.getElementById("edit-category-input")
    let category1 = categoryElement.value
    let category2 = categoryElement.value
    let category3 = categoryElement.value

    let recurrenceElement = document.getElementById("edit-recurrence-input")
    let recurrence = recurrenceElement.value
    let recurrenceStatus = "Off"
    let recurrenceFrequency = "None"
    if (recurrence in ["Weekly", "Monthly", "Yearly"]) {
        recurrenceFrequency = recurrence
        recurrenceStatus = "On"
    }


    let options = {
        lliID: lliid,
        title: llititle,
        description: llidescription,
        category1: category1,
        category2: category2,
        category3: category3,
        status: llistatus,
        visibility: llivisibility,
        deadline: llideadline,
        cost: llicost,
        recurrenceStatus: recurrenceStatus,
        recurrenceFrequency: recurrenceFrequency
    }
    updateLLI(options, url)
}

function updateLLI(options, LLIurl) {

    let isValidOption = validateLLIOptions(options)
    if (!isValidOption) {
        return
    }

    let request = ajaxClient.put(LLIurl, options, jwtToken)

    return new Promise(function (resolve, reject) {
        request.then(function (response) {
            if (response.status != 200) {
                throw new Error(response.statusText)
            }
            return response.json()
        }).then(function (response) {
            alert('The LLI is successfully updated.')
            location.reload()
            resolve(response)
        }).catch(function (error) {
            alert(error)
            reject(error)
        })
    })
}
