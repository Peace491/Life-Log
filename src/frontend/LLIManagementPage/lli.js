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

    let jwtToken = ""

    const webServiceUrl = 'http://localhost:8080/lli';

    // NOT exposed to the global object ("Private" functions)
    function createLLI() {
        let createLLIUrl = webServiceUrl + '/postLLI'

        let title = document.getElementById('create-title-input').textContent
        let deadline = document.getElementById('create-date-input').value
        let categories = document.getElementById('create-category-input')

        var selectedCategories = [];
        for (var i = 0; i < categories.options.length; i++) {
            var option = categories.options[i];
            if (option.selected) {
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
            categories: selectedCategories,
            description: description,
            status: status,
            visibility: visibility,
            deadline: deadline,
            cost: cost,
            recurrenceStatus: recurrenceStatus,
            recurrenceFrequency: recurrenceFrequency
        }

        let request = ajaxClient.post(createLLIUrl, options, jwtToken)

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

    // NOT exposed to the global object ("Private" functions)
    function getAllLLI() {

        let getUrl = webServiceUrl + '/getAllLLI';
        let request = ajaxClient.get(getUrl, jwtToken);

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

        let request = ajaxClient.put(updateLLIUrl, options, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                return response.json()
            }).then(function (response) {
                //location.reload()
                resolve(response)
            }).catch(function (error) {
                reject(error)
            })
        })
    }

    function deleteLLI(lliid) {
        let deleteUrl = webServiceUrl + '/deleteLLI?lliid=' + lliid;
        let request = ajaxClient.del(deleteUrl, jwtToken);

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
        createButton.addEventListener('click', createLLI)
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

        logoutInput.addEventListener('click', function() {
            window.localStorage.clear()
            location.reload()
        })
    }

    function showLLI() {
        let lliContentContainer = document.getElementsByClassName("current-lli-content-container")[0]
        let finishedLLIContentContainer = document.getElementsByClassName("finished-lli-content-container")[0]

        // Get initial lli
        getAllLLI().then(function (completedLLIList) {
            if (!completedLLIList) return
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
        jwtToken = localStorage["token-local"]

        // Set up event handlers
        setupCreateLLITemplate();
        setupCreateLLISubmit();
        // setupFilterSelect();
        setupFilter();
        setupSearch();
        setupLogout();

        // Get data
        showLLI();

    }

    init();

})(window, window.ajaxClient);





