'use strict';
import * as lliDomManip from './lli-dom-manipulation.js'
import * as routeManager from '../routeManager.js';

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
export function loadLLIPage(root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    const MAX_TITLE_LENGTH = 50
    const EARLIEST_DEADLINE = 1960
    const LATEST_DEADLINE = 2100
    const CATEGORIES_LIST = new Set([
        "Mental Health",
        "Physical Health",
        "Outdoor",
        "Sport",
        "Art",
        "Hobby",
        "Thrill",
        "Travel",
        "Volunteering",
        "Food"
    ]);
    const MAX_DESC_LENGTH = 200
    const STATUS_LIST = new Set([
        "Active",
        "Completed",
        "Postponed"
    ])
    const VISIBILITY_LIST = new Set([
        "Public",
        "Private"
    ])
    const MIN_COST = 0
    const RECCURENCE_STATUS_LIST = new Set([
        "On",
        "Off"
    ])
    const RECCURENCE_FREQUENCY_LIST = new Set([
        "Weekly",
        "Monthly",
        "Yearly"
    ])
    const MAX_NUM_OF_CATEGORIES = 3;

    let jwtToken = ""

    const webServiceUrl = 'http://localhost:8080/lli';

    // NOT exposed to the global object ("Private" functions)
    function createLLI(options) {
        let createLLIUrl = webServiceUrl + '/postLLI'

        let isValidOption = validateLLIOptions(options)
        if (!isValidOption) {
            return
        }

        let request = ajaxClient.post(createLLIUrl, options, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response.statusText)
                }

                return response.json()
            }).then(function (response) {
                alert('The LLI is successfully created.')
                location.reload()
                resolve(response)
            }).catch(function (error) {
                alert(error)
                reject(error)
            })
        })
    }

    // NOT exposed to the global object ("Private" functions)
    function getAllLLI() {
        let getUrl = webServiceUrl + '/getAllLLI';
        let request = ajaxClient.get(getUrl, jwtToken);

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

    function updateLLI(options) {
        let updateLLIUrl = webServiceUrl + '/putLLI'

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
                alert('The LLI is successfully updated.')
                location.reload()
                resolve(response)
            }).catch(function (error) {
                alert(error)
                reject(error)
            })
        })
    }

    function deleteLLI(lliid) {
        let deleteUrl = webServiceUrl + '/deleteLLI?lliid=' + lliid;
        let request = ajaxClient.del(deleteUrl, jwtToken);

        return new Promise((resolve, reject) => {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response.statusText)
                }

                return response.json();
            }).then(function (data) {
                alert('The LLI is successfully deleted.')
                location.reload()
                resolve(data);
            }).catch(function (error) {
                alert(error)
                reject(error)
            })
        })
    }

    function validateLLIOptions(option) {

        // Input Validation
        if (option.title == "" || option.title.length > MAX_TITLE_LENGTH || !/^[a-zA-Z0-9]+$/.test(option.title.replaceAll(' ', ''))) {
            alert('The LLI title must only contain  alphanumeric values between 1-50 characters long, please try again.')
            return false
        }

        if (option.category1 == null) {
            alert('The category must not be empty')
            return false
        }

        if (!CATEGORIES_LIST.has(option.category1)) {
            alert('The LLI category must be valid, please try again.')
            return false
        }

        if (option.category2 != null && !CATEGORIES_LIST.has(option.category2)) {
            alert('The LLI category must be valid, please try again.')
            return false
        }
        
        if (option.category3 != null && !CATEGORIES_LIST.has(option.category3)) {
            alert('The LLI category must be valid, please try again.')
            return false
        }

        if (option.description == "" || option.description.length > MAX_DESC_LENGTH || !/^[a-zA-Z0-9]+$/.test(option.description.replaceAll(' ', ''))) {
            alert('The LLI description must only contain  alphanumeric values between 1-200 characters long, please try again.')
            return false
        }

        if (!STATUS_LIST.has(option.status)) {
            alert('The LLI status must be either “Active”, “Completed”, or “Postponed”, please try again.')
            return false
        }

        if (!VISIBILITY_LIST.has(option.visibility)) {
            alert('The LLI visibility must be either “Public” or “Private”, please try again.')
            return false
        }

        let year = parseInt(option.deadline.substring(0, 4))
        if (year < EARLIEST_DEADLINE || year > LATEST_DEADLINE) {
            alert(`The LLI deadline must be between 01/01/${EARLIEST_DEADLINE} and 12/31/${LATEST_DEADLINE}, please try again.`)
            return false
        }

        if (option.cost < MIN_COST || isNaN(option.cost)) {
            alert("The LLI cost must be a numerical value greater or equal to $0 USD, please try again.")
            return false
        }

        if (!RECCURENCE_STATUS_LIST.has(option.recurrenceStatus)) {
            alert('The LLI reccurrence must be either “On” or “Off”, please try again.')
            return false
        }

        if (option.recurrenceStatus == "On" && !RECCURENCE_FREQUENCY_LIST.has(option.recurrenceFrequency)) {
            alert('The LLI occurrence frequency must be “Weekly”, “Monthly”, or “Yearly”, please try again.')
            return false
        }

        return true
    }

    function setupCreateLLITemplate() {
        let addButton = document.getElementById('add-lli-button')
        var addLLITemplate = document.getElementById('create-lli-template')

        addButton.addEventListener('click', function () {
            addLLITemplate.classList.remove('hidden')
        })

        let closeButton = document.getElementById('close-create-lli-button')
        closeButton.addEventListener('click', function () {
            addLLITemplate.classList.add('hidden')
        })
    }

    function setupCreateLLISubmit() {
        let createButton = document.getElementById('create-lli-button')
        createButton.addEventListener('click', function () {
            let title = document.getElementById('create-title-input').textContent
            let deadline = document.getElementById('create-date-input').value
            let categories = document.getElementById('create-category-input')

            var selectedCategories = [];
            for (var i = 0; i < categories.options.length; i++) {
                var option = categories.options[i];
                if (option.selected && selectedCategories.length < MAX_NUM_OF_CATEGORIES) {
                    selectedCategories.push(option.value);
                }
            }

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
                title: title,
                deadline: deadline,
                category1: selectedCategories[0],
                category2: selectedCategories[1],
                category3: selectedCategories[2],
                description: description,
                status: status,
                visibility: visibility,
                deadline: deadline,
                cost: cost,
                recurrenceStatus: recurrenceStatus,
                recurrenceFrequency: recurrenceFrequency
            }

            createLLI(options)
        })
    }

    function setupFilter() {
        let currentFilter = document.getElementById('current-lli-filter-options')

        let showCurrent = true;

        currentFilter.addEventListener('click', function () {
            setupCheckBox('current', currentFilter, showCurrent)

            if (showCurrent == true) {
                showCurrent = false
            } else {
                showCurrent = true
            }
        })

        let finishedFilter = document.getElementById('finished-lli-filter-options')

        let showFinished = true;

        finishedFilter.addEventListener('click', function () {
            setupCheckBox('finished', finishedFilter, showFinished)

            if (showFinished == true) {
                showFinished = false
            } else {
                showFinished = true
            }
        })

    }

    function setupSearch() {
        let currentSearchBar = document.getElementById('current-lli-search-bar')
        currentSearchBar.addEventListener('keydown', function (e) {
            if (e.key == 'Enter') {
                let searchQuery = currentSearchBar.value

                let searchOption = {
                    label: 'Search',
                    values: searchQuery
                }
                filterLLI(searchOption, currentSearchBar)
            }
        })

        let finishedSearchBar = document.getElementById('finished-lli-search-bar')
        finishedSearchBar.addEventListener('keydown', function (e) {
            if (e.key == 'Enter') {
                let searchQuery = finishedSearchBar.value

                let searchOption = {
                    label: 'Search',
                    values: searchQuery
                }
                filterLLI(searchOption, finishedSearchBar)
            }
        })
    }

    function setupCheckBox(containerType, currentFilter, show) {
        let checkboxesContainer = document.getElementById(containerType + "-lli-filter-checkboxes");

        // Uncheck all checkboxes

        let checkboxesDiv = checkboxesContainer.querySelectorAll('.filter-input');

        const checkedValues = []

        if (show) {
            checkboxesContainer.classList.remove('hidden')



            // Iterate through each checkbox
            checkboxesDiv.forEach(checkbox => {
                checkbox.addEventListener('click', function (event) {
                    const checkbox = event.target;
                    const value = checkbox.value;

                    if (checkbox.checked) {
                        // If checkbox is checked, add its value to the checkedValues array
                        checkedValues.push(value);
                    } else {
                        // If checkbox is unchecked, remove its value from the checkedValues array
                        const index = checkedValues.indexOf(value);
                        if (index !== -1) {
                            checkedValues.splice(index, 1);
                        }
                    }

                    let currentFilterOption = {}

                    // Log the array of checked values
                    if (checkedValues.length == 0) {
                        currentFilterOption = {
                            label: "Filter",
                            values: "None"
                        }
                    } else {
                        currentFilterOption = {
                            label: "Filter",
                            values: checkedValues
                        }
                    }

                    filterLLI(currentFilterOption, currentFilter)

                });
            });
            show = false;
        } else {
            checkboxesContainer.classList.add('hidden')

            show = true;
        }
    }

    function setupLogout() {
        let logoutInput = document.getElementById('logout')

        logoutInput.addEventListener('click', function () {
            window.localStorage.clear()
            routeManager.loadPage(routeManager.PAGES.homePage)
        })
    }


    function showLLI() {
        let lliContentContainer = document.getElementsByClassName("current-lli-content-container")[0]
        let finishedLLIContentContainer = document.getElementsByClassName("finished-lli-content-container")[0]

        // Get initial lli
        getAllLLI().then(function (completedLLIList) {
            if (!completedLLIList) return
            completedLLIList.reverse().forEach(lli => {
                let lliHTML = lliDomManip.createLLIComponents(lli, createLLI, getAllLLI, updateLLI, deleteLLI);
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
        jwtToken = localStorage["token-local"]

        if (jwtToken == null) {
            routeManager.loadPage(routeManager.PAGES.homePage)
        } else {
            // Set up event handlers
            setupCreateLLITemplate();
            setupCreateLLISubmit();
            // setupFilterSelect();
            setupFilter();
            setupSearch();
            setupLogout();
            
            // Get data
            showLLI();

            //navigate 
            routeManager.setupHeaderLinks();
        }
    }

    init();

}





