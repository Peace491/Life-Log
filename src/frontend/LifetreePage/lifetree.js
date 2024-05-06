'use strict';

import * as routeManager from '../routeManager.js'
import * as log from '../Log/log.js'

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
export function loadLifetreePage(root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    //PN constants
    const MAX_CONTENT_LENGTH = 1200
    const MAX_DATE = 2100
    const EARLIEST_NOTE_YEAR = 1960
    const LATEST_NOTE_YEAR = new Date().getFullYear();

    let jwtToken = ""

    const lifetreeServiceUrl = 'http://localhost:8092/lifetreeService';


    // NOT exposed to the global object ("Private" functions)

    // HTTP REQUESTS -----------------------------------------------------------
    function getCompletedLLI() {
        let getDataUrl = lifetreeServiceUrl + "/getCompletedLLI"

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
        let getUrl = lifetreeServiceUrl + '/getPN?notedate=' + notedate;

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


    function createNote(options) {
        let createPersonalNoteUrl = lifetreeServiceUrl + '/postPN'

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

    // -------------VALIDATION LLI PN END -----------------------------

    // add your functions here
    function showLifetree() {

        //make document fragment for segments for the lifetree

        getCompletedLLI().then(function (data) {

            // process each lli like this
            data.output.forEach(function (lli, index) {

                let treeFragment = document.createDocumentFragment()

                let div = document.createElement('div')
                div.classList.add("tree-container")

                let img = document.createElement('img')
                img.classList.add("tree-segment")
                img.classList.add("left-branch")
                img.src = "Assets/Left Branch Topdown.svg"
                img.alt = 'Left Branch'

                let button = document.createElement('button')
                button.classList.add("lli-title-text")
                button.classList.add("left-text")
                button.classList.add("left-branch")
                button.classList.add("btn")

                let span = document.createElement('span')
                span.classList.add("overflow")
                span.textContent = lli.title

                button.appendChild(span)

                div.appendChild(img)
                div.appendChild(button)

                treeFragment.appendChild(div)
            })

            const container = document.getElementById('main-container')
            container.appendChild(treeFragment)

        })


    }


    function showNote() {
        let dateInput = document.getElementById("create-date-input");
        let selectedDate = dateInput.value;
        // Validate Date
        let year = parseInt(selectedDate.substring(0, 4));
        let month = parseInt(selectedDate.substring(5, 7)) - 1; // Subtract 1 because months are zero-based
        let day = parseInt(selectedDate.substring(8, 10));

        let noteDate = new Date();
        noteDate.setDate(day);
        noteDate.setMonth(month);
        noteDate.setFullYear(year);

        if (noteDate < EARLIEST_NOTE_DATE || noteDate > LATEST_NOTE_DATE) {
            alert(`You can only select datess within the past 6 months. (${EARLIEST_NOTE_DATE.toDateString()} and ${LATEST_NOTE_DATE.toDateString()}), please try again.`)
            location.reload()
            return false
        }

        // Call getNote function to retrieve note data
        getNote(selectedDate).then(function (noteText) {
            let noteParagraph = document.getElementById("create-paragraph-input");
            if(noteText == null)
            {
                noteParagraph.textContent = '';
                noteParagraph.placeholder = "What are you thinking?......";
                noteParagraph.classList.add("NewNote");
                noteParagraph.setAttribute("noteid", "");
            }
            else
            {
                // Update paragraph content with the retrieved note text
                noteParagraph.textContent = noteText[0]["noteContent"];
                if (noteParagraph.classList.contains("NewNote"))
                {
                    noteParagraph.classList.remove("NewNote");
                }
                noteParagraph.setAttribute("noteid", noteText[0]["noteId"]);
            }
        })
        .catch(function (error) {
            // Handle error if retrieval fails
            alert("Error retrieving note: " + error);
        });
    }

    

    function pnCreation() { // pn create

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

        if (jwtToken == null) { // change to ==
            alert("Unauthorized User In View")
            routeManager.loadPage(routeManager.PAGES.homePage)
        } else {
            
            //add main calls here
            showLifetree()
            

        }
    }

    init();

    // let nextMonth = document.getElementById("get-next-month")
    // nextMonth.addEventListener("click", onClickGetNextMonth)

}



