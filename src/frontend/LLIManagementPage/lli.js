'use strict';

import { createLLIComponents } from './lli-dom-manipulation.js'

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    const webServiceUrl = 'http://localhost:8080/lli';

    // NOT exposed to the global object ("Private" functions)
    function createLLI() {
        let createLLIUrl = webServiceUrl + '/postLLI'

        let title = document.getElementById('create-title-input').textContent
        let deadline = document.getElementById('create-date-input').value
        let category = document.getElementById('create-category-input').value
        let description = document.getElementById('create-paragraph-input').textContent
        let status = document.getElementById('create-status-input').value
        let visibility = document.getElementById('create-visibility-input').value
        let cost = document.getElementById('create-cost-input').textContent
        let recurrence = document.getElementById('create-recurrence-input').value

        let recurrenceStatus;
        let recurrenceFrequency;
        if (recurrence === "Off") {
            recurrenceStatus = "Off"
            recurrenceFrequency = "None"
        }
        else {
            recurrenceStatus = "On"
            recurrenceFrequency = recurrence
        }

        let options = {
            userHash: "System",
            title: title,
            deadline: deadline,
            category: category,
            description: description,
            status: status,
            visibility: visibility,
            deadline: deadline,
            cost: cost,
            recurrenceStatus: recurrenceStatus,
            recurrenceFrequency: recurrenceFrequency
        }


        let request = ajaxClient.post(createLLIUrl, options)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                return response.json()
            }).then(function (response) {
                console.log(response)
                location.reload()
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    // NOT exposed to the global object ("Private" functions)
    function getAllLLI() {

        let getUrl = webServiceUrl + '/getAllLLI?userHash=System';
        let request = ajaxClient.get(getUrl);

        return new Promise((resolve, reject) => {
            request.then(function (response) {
                return response.json();
            }).then(function (data) {
                let output = data.output;
                resolve(output);
            }).catch(function (error) {
                reject(error);
            });
        });
    }

    function updateLLI(options) {
        let updateLLIUrl = webServiceUrl + '/putLLI'

        let request = ajaxClient.put(updateLLIUrl, options)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                return response.json()
            }).then(function (response) {
                location.reload()
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    function deleteLLI(lliid) {
        let deleteUrl = webServiceUrl + '/deleteLLI?userHash=System&lliID=' + lliid;
        let request = ajaxClient.del(deleteUrl);

        return new Promise((resolve, reject) => {
            request.then(function (response) {
                return response.json();
            }).then(function (data) {
                location.reload()
                resolve(data);
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    function setupAddButton() {
        let addButton = document.getElementById('add-lli-button')
        addButton.addEventListener('click', function () {
            var addLLITemplate = document.getElementById('create-lli-template')
            addLLITemplate.classList.remove('hidden')
        })
    }

    function setupCreateLLISubmit() {
        let createButton = document.getElementById('create-lli-button')
        createButton.addEventListener('click', createLLI)
    }

    function showLLI() {
        let lliContentContainer = document.getElementsByClassName("current-lli-content-container")[0]
        let finishedLLIContentContainer = document.getElementsByClassName("finished-lli-content-container")[0]

        // Get initial lli
        getAllLLI().then(function (completedLLIList) {
            completedLLIList.reverse().forEach(lli => {
                let lliHTML = createLLIComponents(lli, createLLI, getAllLLI, updateLLI, deleteLLI);
                if (lli.status != "Completed") {
                    lliContentContainer.append(lliHTML);
                }
                else {
                    finishedLLIContentContainer.append(lliHTML);
                }
            });
        });
    }

    root.myApp = root.myApp || {};

    // Initialize the current view by setting up data and attaching event handlers 
    function init() {
        // Set up event handlers
        setupAddButton();
        setupCreateLLISubmit();

        // Get data
        showLLI();

    }

    init();

})(window, window.ajaxClient);

function expandLLIHTML(container, hiddenContent) {


}





