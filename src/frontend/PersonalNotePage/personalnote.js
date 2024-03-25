'use strict';
import Router from '../routes.js';
// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    const MAX_CONTENT_LENGTH = 1200
    const MAX_DATE = 2100
    const EARLIEST_NOTE_YEAR = 1960
    const LATEST_NOTE_YEAR = new Date().getFullYear();

    let jwtToken = ""

    const webServiceUrl = 'http://localhost:8083/personalnote';

    // NOT exposed to the global object ("Private" functions)
    function createNote(options) {
        let createPersonalNoteUrl = webServiceUrl + '/postPersonalNote'

        let isValidOption = validatePersonalNoteOptions(options)
        if (!isValidOption) {
            return
        }

        let request = ajaxClient.post(createPersonalNoteUrl, options, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response.statusText)
                }

                return response.json()
            }).then(function (response) {
                alert('The Personal Note is successfully created.')
                location.reload()
                resolve(response)
            }).catch(function (error) {
                alert(error)
                reject(error)
            })
        })
    }

    // // NOT exposed to the global object ("Private" functions)
    // function getAllLLI() {
    //     let getUrl = webServiceUrl + '/getAllLLI';
    //     let request = ajaxClient.get(getUrl, jwtToken);

    //     return new Promise((resolve, reject) => {
    //         request.then(function (response) {
    //             if (response.status != 200) {
    //                 throw new Error(response.statusText)
    //             }

    //             return response.json();
    //         }).then(function (data) {
    //             let output = data.output;
    //             resolve(output);
    //         }).catch(function (error) {
    //             alert(error)
    //             reject(error);
    //         });
    //     });
    // }

    // function updateLLI(options) {
    //     let updateLLIUrl = webServiceUrl + '/putLLI'

    //     let isValidOption = validateLLIOptions(options)
    //     if (!isValidOption) {
    //         return
    //     }

    //     let request = ajaxClient.put(updateLLIUrl, options, jwtToken)

    //     return new Promise(function (resolve, reject) {
    //         request.then(function (response) {
    //             if (response.status != 200) {
    //                 throw new Error(response.statusText)
    //             }

    //             return response.json()
    //         }).then(function (response) {
    //             alert('The LLI is successfully updated.')
    //             location.reload()
    //             resolve(response)
    //         }).catch(function (error) {
    //             alert(error)
    //             reject(error)
    //         })
    //     })
    // }

    // function deleteLLI(lliid) {
    //     let deleteUrl = webServiceUrl + '/deleteLLI?lliid=' + lliid;
    //     let request = ajaxClient.del(deleteUrl, jwtToken);

    //     return new Promise((resolve, reject) => {
    //         request.then(function (response) {
    //             if (response.status != 200) {
    //                 throw new Error(response.statusText)
    //             }

    //             return response.json();
    //         }).then(function (data) {
    //             alert('The LLI is successfully deleted.')
    //             location.reload()
    //             resolve(data);
    //         }).catch(function (error) {
    //             alert(error)
    //             reject(error)
    //         })
    //     })
    // }

    function validatePersonalNoteOptions(option) {
        // Input Validation
        if (option.notecontent == "" || option.notecontent.length > MAX_CONTENT_LENGTH || !/^[a-zA-Z0-9]+$/.test(option.notecontent.replaceAll(' ', ''))) {
            alert('The note must only contain  alphanumeric values between 1-50 characters long, please try again.')
            return false
        }

        let year = parseInt(option.date.substring(0, 4))
        if (year < EARLIEST_NOTE_YEAR || year > LATEST_NOTE_YEAR) {
            alert(`The LLI deadline must be between 01/01/${EARLIEST_NOTE_YEAR} and 12/31/${LATEST_NOTE_YEAR}, please try again.`)
            return false
        }



        return true
    }

    // function setupCreatePerTemplate() {
    //     let addButton = document.getElementById('add-lli-button')
    //     var addLLITemplate = document.getElementById('create-lli-template')

    //     addButton.addEventListener('click', function () {
    //         addLLITemplate.classList.remove('hidden')
    //     })

    //     let closeButton = document.getElementById('close-create-lli-button')
    //     closeButton.addEventListener('click', function () {
    //         addLLITemplate.classList.add('hidden')
    //     })
    // }

    // function setupCreateLLISubmit() {
    //     let createButton = document.getElementById('create-lli-button')
    //     createButton.addEventListener('click', function () {
    //         let title = document.getElementById('create-title-input').textContent
    //         let deadline = document.getElementById('create-date-input').value
    //         let categories = document.getElementById('create-category-input')

    //         var selectedCategories = [];
    //         for (var i = 0; i < categories.options.length; i++) {
    //             var option = categories.options[i];
    //             if (option.selected) {
    //                 selectedCategories.push(option.value);
    //             }
    //         }

    //         let description = document.getElementById('create-paragraph-input').textContent
    //         let status = document.getElementById('create-status-input').value
    //         let visibility = document.getElementById('create-visibility-input').value
    //         let cost = document.getElementById('create-cost-input').textContent
    //         let recurrence = document.getElementById('create-recurrence-input').value

    //         let recurrenceStatus;
    //         let recurrenceFrequency;
    //         if (recurrence === "Off") {
    //             recurrenceStatus = "Off"
    //             recurrenceFrequency = "None"
    //         }
    //         else {
    //             recurrenceStatus = "On"
    //             recurrenceFrequency = recurrence
    //         }

    //         let options = {
    //             title: title,
    //             deadline: deadline,
    //             categories: selectedCategories,
    //             description: description,
    //             status: status,
    //             visibility: visibility,
    //             deadline: deadline,
    //             cost: cost,
    //             recurrenceStatus: recurrenceStatus,
    //             recurrenceFrequency: recurrenceFrequency
    //         }

    //         createNote(options)
    //     })
    // }

    function setupLogout() {
        let logoutInput = document.getElementById('logout')

        logoutInput.addEventListener('click', function () {
            window.localStorage.clear()
            location.reload()
        })
    }


    root.myApp = root.myApp || {};

    // Initialize the current view by setting up data and attaching event handlers 
    function init() {
        jwtToken = localStorage["token-local"]

        if (jwtToken == null) {
            window.location = '../HomePage/index.html'
        } else {
            // Set up event handlers
            // setupCreateLLITemplate();
            // setupCreateLLISubmit();
            // setupFilterSelect();
            setupLogout();

            // Get data
            // showLLI();

            //navigate 
            const router = new Router;
            router.navigatePages();
        }
    }

    document.addEventListener('DOMContentLoaded', function() {
        init();
    });

})(window, window.ajaxClient);





