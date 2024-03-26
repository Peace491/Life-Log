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
        let createPersonalNoteUrl = webServiceUrl + '/postPN'

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
                resolve(response)
            }).catch(function (error) {
                alert(error)
                reject(error)
            })
        })
    }

    // NOT exposed to the global object ("Private" functions)
    function getNote(options) {
        let getUrl = webServiceUrl + '/getPN';

        let isValidOption = validatePersonalNoteOptions(options)
        if (!isValidOption) {
            return
        }
        let request = ajaxClient.get(getUrl, options, jwtToken);

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

    function updateNote(options) {
        let updateLLIUrl = webServiceUrl + '/putPN'

        let isValidOption = validateLLIOptions(options)
        if (!isValidOption) {
            return
        }

        let request = ajaxClient.put(updateLLIUrl, options, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response.statusText)
                }

                return response.json()
            }).then(function (response) {
                alert('The Note was successfully updated.')
                location.reload()
                resolve(response)
            }).catch(function (error) {
                alert(error)
                reject(error)
            })
        })
    }

    function deleteNote(lliid) {
        let deleteUrl = webServiceUrl + '/deleteLLI?lliid=' + lliid;
        let request = ajaxClient.del(deleteUrl, jwtToken);

        return new Promise((resolve, reject) => {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response.statusText)
                }

                return response.json();
            }).then(function (data) {
                alert('The Note was successfully deleted.')
                location.reload()
                resolve(data);
            }).catch(function (error) {
                alert(error)
                reject(error)
            })
        })
    }

    function validatePersonalNoteOptions(option) {
        // Input Validation
        if (option.notecontent == "" || option.notecontent.length > MAX_CONTENT_LENGTH || !/^[a-zA-Z0-9]+$/.test(option.notecontent.replaceAll(' ', ''))) {
            alert('The note must only contain  alphanumeric values between 1-50 characters long, please try again.')
            return false
        }

        let year = parseInt(option.notedate.substring(0, 4))
        if (year < EARLIEST_NOTE_YEAR || year > LATEST_NOTE_YEAR) {
            alert(`The LLI deadline must be between 01/01/${EARLIEST_NOTE_YEAR} and 12/31/${LATEST_NOTE_YEAR}, please try again.`)
            return false
        }



        return true
    }

    function setupCreateNoteSubmit() {
        let createButton = document.getElementById('submit-note-button')
        createButton.addEventListener('click', function () {
            let content = document.getElementById('create-paragraph-input').textContent
            let date = document.getElementById('create-date-input').value

            let options = {
                notedate: date,
                notecontent: content
            }

            createNote(options)
        })
    }

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
            setupCreateNoteSubmit();
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





