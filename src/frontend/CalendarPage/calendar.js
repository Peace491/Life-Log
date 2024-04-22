'use strict';

import * as routeManager from '../routeManager.js'
import * as calendarDomManipulation from './calendar-dom-manipulation.js'
import * as log from '../Log/log.js'

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
export function loadCalendarPage(root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    //LLI Constants
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

    //PN constants
    const MAX_CONTENT_LENGTH = 1200
    const MAX_DATE = 2100
    const EARLIEST_NOTE_YEAR = 1960
    const LATEST_NOTE_YEAR = new Date().getFullYear();

    let jwtToken = ""

    const calendarServiceUrl = 'http://localhost:8087/calendarService';


    // NOT exposed to the global object ("Private" functions)
    // HTTP REQUESTS -----------------------------------------------------------

    function getMonthLLI(month, year) {
        let getDataUrl = calendarServiceUrl + "/getMonthLLI?month=" + month + "&&year=" + year

        let request = ajaxClient.get(getDataUrl, jwtToken)

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

    function getNote(notedate) {
        let getUrl = calendarServiceUrl + '/getMonthPN?notedate=' + notedate;

        // validate the date
        let year = parseInt(notedate.substring(0, 4))
        if (year < EARLIEST_NOTE_YEAR || year > LATEST_NOTE_YEAR) {
            alert(`The Note date must be between 01/01/${EARLIEST_NOTE_YEAR} and 12/31/${LATEST_NOTE_YEAR}, please try again.`)
            return
        }

        let request = ajaxClient.get(getUrl, jwtToken);

        return new Promise((resolve, reject) => {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response.statusText + " Cannot Write or view Note on future dates")
                }

                return response.json();
            }).then(function (data) {
                resolve(data);
            }).catch(function (error) {
                alert(error)
                reject(error);
            });
        });
    }

    function createLLI(options) {
        let createLLIUrl = calendarServiceUrl + '/postLLI'

        let isValidOption = validateLLIOptions(options) // add this method
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

    function updateLLI(options) {
        let updateLLIUrl = calendarServiceUrl + '/putLLI'

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

    function createNote(options) {
        let createPersonalNoteUrl = calendarServiceUrl + '/postPN'

        let isValidOption = validatePersonalNoteOptions(options) // add this method
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

    function updateNote(options) {
        let updateLLIUrl = calendarServiceUrl + '/putPN'

        let isValidOption = validatePersonalNoteOptions(options)
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
                resolve(response)
            }).catch(function (error) {
                alert(error)
                reject(error)
            })
        })
    }

    //-----------------VALIDATION PN LLI----------------------
    function validatePersonalNoteOptions(option) {
        // Input Validation
        if (option.notecontent == "" || option.notecontent.length > MAX_CONTENT_LENGTH || !/^[a-zA-Z0-9]+$/.test(option.notecontent.replaceAll(' ', ''))) {
            alert('The note must only contain  alphanumeric values between 1-50 characters long, please try again.')
            return false
        }

        let year = parseInt(option.notedate.substring(0, 4))
        if (year < EARLIEST_NOTE_YEAR || year > LATEST_NOTE_YEAR) {
            alert(`The Note date must be between 01/01/${EARLIEST_NOTE_YEAR} and 12/31/${LATEST_NOTE_YEAR}, please try again.`)
            return false
        }
        return true
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

    // -------------VALIDATION LLI PN END -----------------------------

    // add your functions here
    function showCalendarLLI(month, year) {

        getMonthLLI(month, year).then(function (data) {

            //display each lli event on calendar
            data.output.forEach(function (lli, index) {

                let lliDeadlineDay = lli.deadline.split("/")[1];


                let lliEventDay = document.getElementById(`insert-llievent-${lliDeadlineDay}`);

                let event = `<button data-modal-target="#lli-modal-edit" class="lli-btn event" id="lli-btn-${lli.lliid}">${lli.title}</button>`

                lliEventDay.innerHTML += event
                renderModals()
                // #region modals 
                function renderModals() {

                    const openModalButtons = document.querySelectorAll('[data-modal-target]')
                    const overlay = document.getElementById('overlay')

                    openModalButtons.forEach(button => {

                        button.addEventListener('click', () => {

                            const modal = document.querySelector(button.dataset.modalTarget)
                            openModal(modal)
                        })
                    })

                    overlay.addEventListener('click', () => {
                        const modals = document.querySelectorAll('.modal.active')
                        modals.forEach(modal => {
                            closeModal(modal)
                        })
                    })


                    function openModal(modal) {
                        if (modal == null) {
                            console.log("null")
                            return
                        }
                        modal.classList.add('active')
                        overlay.classList.add('active')
                    }

                    function closeModal(modal) {
                        if (modal == null) return
                        modal.classList.remove('active')
                        overlay.classList.remove('active')
                    }
                }
                // #endregion

            })

            //add event lister when #lli-btn-${lli.lliid} is clicked
            // Give info to modal
            let lliEventBox = document.querySelectorAll(".lli-events.event")

            lliEventBox.forEach((eachBox) => {
                eachBox.addEventListener('click', (event) => {

                    const isButton = event.target.nodeName === 'BUTTON';
                    if (!isButton) {

                        return;
                    }

                    let lliidToFind = event.target.id.split("-")[2];



                    let llititle = ""
                    let llideadline = ""
                    let llidescription = ""
                    let llistatus = ""
                    let visibility = ""
                    let cost = 0
                    let cat1 = ""
                    let cat2 = ""
                    let cat3 = ""
                    data.output.forEach(function (item) {
                        if (item.lliid === lliidToFind) {
                            llititle = item.title;
                            llideadline = item.deadline.split(" ")[0]
                            llidescription = item.description
                            llistatus = item.status
                            visibility = item.visibility
                            cost = item.cost
                            cat1 = item.category1
                            cat2 = item.category2
                            cat3 = item.category3
                        }
                    })

                    // get all info for that lliid and put into divs 
                    let titleElement = document.getElementById("edit-title-input")
                    titleElement.textContent = `${llititle}`;

                    let deadlineElement = document.getElementById("edit-date-input")
                    deadlineElement.value = `${llideadline.split("/")[2]}-${llideadline.split("/")[0]}-${llideadline.split("/")[1]}`;

                    let descriptionElement = document.getElementById("edit-paragraph-input")
                    descriptionElement.textContent = `${llidescription}`;

                    let statusElement = document.getElementById("edit-status-input")
                    statusElement.value = `${llistatus}`;

                    let visibilityElement = document.getElementById("edit-visibility-input")
                    visibilityElement.value = `${visibility}`;

                    let costElement = document.getElementById("edit-cost-input")

                    if (costElement) {
                        costElement.textContent = `${cost}`;
                    }


                    let categoryElement = document.getElementById("edit-category-input")
                    categoryElement.value = `${cat1}`;
                    categoryElement.value = `${cat2}`;
                    categoryElement.value = `${cat3}`;
                })
            });
        })
    }


    function editLLI() {

        let titleElement = document.getElementById("edit-title-input")
        let llititle = titleElement.textContent

        let deadlineElement = document.getElementById("edit-date-input")
        let llideadline = deadlineElement.value


        let getLLIID = document.getElementById(`insert-llievent-${llideadline.split("-")[2]}`).querySelector(".lli-btn.event");
        console.log("id parent", `insert-llievent-${llideadline.split("-")[2]}`)
        console.log(getLLIID.id)
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
        updateLLI(options)
    }

    function lliCreation() {

        let titleElement = document.getElementById("create-title-input")
        let llititle = titleElement.textContent

        let deadlineElement = document.getElementById("create-date-input")
        let llideadline = deadlineElement.value


        let descriptionElement = document.getElementById("create-paragraph-input")
        let llidescription = descriptionElement.textContent

        let statusElement = document.getElementById("create-status-input")
        let llistatus = statusElement.value

        let visibilityElement = document.getElementById("create-visibility-input")
        let llivisibility = visibilityElement.value

        let costElement = document.getElementById("create-cost-input")
        let llicost = 100
        if (costElement) {
            llicost = costElement.textContent
        }
        // else{
        //     alert('unable to update cost')
        // }


        let categoryElement = document.getElementById("create-category-input")
        let category1 = categoryElement.value
        let category2 = categoryElement.value
        let category3 = categoryElement.value

        let recurrenceElement = document.getElementById("create-recurrence-input")
        let recurrence = recurrenceElement.value
        let recurrenceStatus = "Off"
        let recurrenceFrequency = "None"
        if (recurrence in ["Weekly", "Monthly", "Yearly"]) {
            recurrenceFrequency = recurrence
            recurrenceStatus = "On"
        }


        let options = {
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
        createLLI(options)
    }

    window.PNRetrieval = function () {

        let allPNButtons = document.querySelectorAll(".pn-btn")

        allPNButtons.forEach((PNButton) => {
            PNButton.addEventListener('click', (event) => {

                let pnDateNoFormat = PNButton.id.split("-")[2]
                let formattedDate = `${pnDateNoFormat.split("/")[2]}-${pnDateNoFormat.split("/")[0]}-${pnDateNoFormat.split("/")[1]}`

                getNote(formattedDate).then(function (data) {

                    if (data.output !== null) {

                        let pnDateElement = document.getElementById("create-date-input-pn")
                        pnDateElement.readOnly = true;
                        let pnDesElement = document.getElementById("create-paragraph-input-pn")
                        pnDesElement.contentEditable = "false";
                        let submitBtn = document.getElementById("create-pn-btn")
                        submitBtn.disabled = true;


                        // adding and removing classes for creating/editing a note

                        if (submitBtn.classList.contains("new-note")) {
                            submitBtn.classList.remove("new-note");
                        }
                        submitBtn.classList.add("existing-note")

                        // show the content of note in here div should have this content
                        let personalNote = data.output[0]

                        let noteParagraphElement = document.getElementById("create-paragraph-input-pn")

                        noteParagraphElement.textContent = `${personalNote.noteContent}`;

                    }
                    else {

                        let pnDateElement = document.getElementById("create-date-input-pn")
                        pnDateElement.readOnly = false;
                        pnDateElement.disabled = false;

                        let pnDesElement = document.getElementById("create-paragraph-input-pn")
                        pnDesElement.contentEditable = "true";

                        let submitBtn = document.getElementById("create-pn-btn")
                        submitBtn.disabled = false;

                        let noteParagraphElement = document.getElementById("create-paragraph-input-pn")
                        noteParagraphElement.textContent = '';

                        // adding and removing classes for creating/editing a note
                        let submitPNBtn = document.getElementById("create-pn-btn")
                        if (submitPNBtn.classList.contains("existing-note")) {
                            submitPNBtn.classList.remove("existing-note");
                        }
                        submitPNBtn.classList.add("new-note")

                    }

                })
            })
        })
    }

    function pnCreation() {

        let pnDateElement = document.getElementById("create-date-input-pn")
        let noteDate = pnDateElement.value

        let pnDesElement = document.getElementById("create-paragraph-input-pn")
        let noteContent = pnDesElement.textContent

        let options = {
            notedate: noteDate,
            notecontent: noteContent
        }
        createNote(options)
    }

    function setupLogout() {
        let logoutInput = document.getElementById('logout')

        logoutInput.addEventListener('click', function () {
            window.localStorage.clear()
            routeManager.loadPage(routeManager.PAGES.homePage)
        })
    }





    root.myApp = root.myApp || {};

    // Show or Hide private functions
    //root.myApp.getData = getDataHandler;
    //root.myApp.sendData = sendDataHandler;

    // Initialize the current view by attaching event handlers 
    function init() {

        jwtToken = localStorage["token-local"]

        if (jwtToken == null) {
            alert("Unauthorized User In View")
            routeManager.loadPage(routeManager.PAGES.homePage)
        } else {
            var userHash = JSON.parse(jwtToken).Payload.UserHash
            log.logPageAccess(userHash, routeManager.PAGES.calendarPage, jwtToken)

            // call functions here 
            let editLLIBtn = document.getElementById("edit-lli-button")
            editLLIBtn.addEventListener('click', () => {
                editLLI()
            })

            let createLLIBtn = document.getElementById("create-lli-button")
            createLLIBtn.addEventListener('click', () => {
                lliCreation()
            })

            let createPNBtn = document.getElementById("create-pn-btn")
            createPNBtn.addEventListener('click', () => {

                pnCreation()

            })

            calendarDomManipulation.renderCalendar(showCalendarLLI)
            calendarDomManipulation.renderModals()

            let timeAccessed = performance.now()
            routeManager.setupHeaderLinks(routeManager.PAGES.calendarPage, timeAccessed, jwtToken);

        }
    }

    init();

    // let nextMonth = document.getElementById("get-next-month")
    // nextMonth.addEventListener("click", onClickGetNextMonth)

}



