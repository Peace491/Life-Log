import { createLLIHTML, convertLLIToEditMode } from './lli-dom-manipulation.js'

'use strict';

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

    // NOT exposed to the global object ("Private" functions)
    function sendDataHandler(url) {
        var request = ajaxClient.send(url, {
            data: ['Alice', 'Bob', 'John'] // Hard-coding data
        });

        const contentElement = document.getElementsByClassName('dynamic-content')[0];

        request
            .then(function (response) {

                const timestamp = new Date();

                contentElement.innerHTML = response.data + " " + timestamp.toString();
            })
            .catch(function (error) {
                console.log("Send", url, error.response.data); // Payload error message

                contentElement.innerHTML = error; // Only showing top level error message
            });
    }

    function deleteLLI(lliid) {
        let deleteUrl = webServiceUrl + '/deleteLLI?userHash=System&lliID=' + lliid;
        console.log(deleteUrl)
        let request = ajaxClient.del(deleteUrl);

        return new Promise((resolve, reject) => {
            request.then(function (response) {
                return response.json();
            }).then(function (data) {
                resolve(data);
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    function showLLI() {
        let lliContentContainer = document.getElementsByClassName("current-lli-content-container")[0]
        let finishedLLIContentContainer = document.getElementsByClassName("finished-lli-content-container")[0]

        // Get initial lli
        getAllLLI().then(function (completedLLIList) {
            completedLLIList.reverse().forEach(lli => {
                let lliHTML = createLLIHTML(lli);
                if (lli.status != "Completed") {
                    lliContentContainer.append(lliHTML);
                }
                else {
                    finishedLLIContentContainer.append(lliHTML);
                }
            });
        });
    }

    function setupAddButton() {
        let addButton = document.getElementById('add-lli-button')
        addButton.addEventListener('click', function() {
            var addLLITemplate = document.getElementById('create-lli-template')
            addLLITemplate.classList.remove('hidden')
        })
    }

    root.myApp = root.myApp || {};

    // Initialize the current view by setting up data and attaching event handlers 
    function init() {
        setupAddButton();
        showLLI();

    }

    init();

})(window, window.ajaxClient);

function expandLLIHTML(container, hiddenContent) {


}





