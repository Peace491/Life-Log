'use strict';
import * as routeManager from '../routeManager.js'
import * as personalNoteDomManipulator from './personalnote-dom-manipulation.js'
import * as log from '../Log/log.js'
// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
export function LoadPersonalNotePage (root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    const MAX_CONTENT_LENGTH = 1200
    // To get the date six months earlier
    let early_date = new Date();
    early_date.setMonth(early_date.getMonth() - 6);

    const EARLIEST_NOTE_DATE = early_date;
    const LATEST_NOTE_DATE = new Date();
    LATEST_NOTE_DATE.setDate(LATEST_NOTE_DATE.getDate() + 1);

    let jwtToken = ""
    let webServiceUrl = ""
    let createUrl = ""
    let getUrl = ""
    let updateUrl = ""
    let deleteUrl = ""

    // NOT exposed to the global object ("Private" functions)
    function createNote(options) {
        let createPersonalNoteUrl = webServiceUrl + createUrl;

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
    function getNote(notedate) {
        let get_Url = webServiceUrl + getUrl + notedate;
        let request = ajaxClient.get(get_Url, jwtToken);

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
        let Url = webServiceUrl + updateUrl;

        let isValidOption = validatePersonalNoteOptions(options)
        if (!isValidOption) {
            return
        }

        let request = ajaxClient.put(Url, options, jwtToken)

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

    function deleteNote(noteid) {
        let Url = webServiceUrl + deleteUrl + noteid;
        let request = ajaxClient.del(Url, jwtToken);

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
            alert("The note must only contain  alphanumeric values between 1-" + MAX_CONTENT_LENGTH + ", please try again.")
            return false
        }

        let year = parseInt(option.notedate.substring(0, 4));
        let month = parseInt(option.notedate.substring(5, 7)) - 1; // Subtract 1 because months are zero-based
        let day = parseInt(option.notedate.substring(8, 10));

        let noteDate = new Date();
        noteDate.setDate(day);
        noteDate.setMonth(month);
        noteDate.setFullYear(year);

        if (noteDate < EARLIEST_NOTE_DATE || noteDate > LATEST_NOTE_DATE) {
            alert(`You can only select dates within the past 6 months. (${EARLIEST_NOTE_DATE.toDateString()} and ${LATEST_NOTE_DATE.toDateString()}), please try again.`)
            location.reload()
            return false
        }



        return true
    }

    function setupCreateNoteSubmit() {
        let createButton = document.getElementById('submit-note-button')
        createButton.addEventListener('click', function () {
            let charContainer = document.getElementById('indicator');
            let noteParagraph = document.getElementById("create-paragraph-input")
            let content = noteParagraph.textContent
            let date = document.getElementById('create-date-input').value
            let options = {
                noteid : noteParagraph.getAttribute("noteid"),
                notedate: date,
                notecontent: content
            }
            charContainer.style.visibility = "hidden";

            if (noteParagraph.classList.contains("NewNote"))
            {
                createNote(options)
                .then(function (){
                    getNote(date)
                    .then(function(noteText){
                        noteParagraph.setAttribute("noteid", noteText[0]["noteId"]);
                        noteParagraph.classList.remove("NewNote")
                    })
                    .catch(function (error) {
                        // Handle error if retrieval fails
                        alert("Error retrieving note: " + error);
                    });
                })
                
            }
            else
            {
                updateNote(options);
            }
            
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
        getNote(selectedDate)
        .then(function (noteText) {
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

    // Attach event listener to the date input element, to always show latest change
    function currNote () {
        let date_input = document.getElementById("create-date-input");
        
        date_input.onchange = function (){
            showNote();
        }
    }

    function countCharacters() {
        let noteParagraph = document.getElementById("create-paragraph-input");
        noteParagraph.oninput = function () {
            let modifiedParagraph = document.getElementById("create-paragraph-input");
            const characterCount = modifiedParagraph.innerText.length;
            const maxCharacters = 1200;
            let charContainer = document.getElementById('indicator');

            if (characterCount > maxCharacters){
                alert("Character Limit of " + maxCharacters + " Exceeded!")
            }
            charContainer.textContent = `(${characterCount}/${maxCharacters})`;

            // Show or hide the character indicator based on whether there is input
            if (characterCount > 0) {
                charContainer.style.visibility = "visible";
            } else {
                charContainer.style.visibility = "hidden";
            }
        };
    }
    

    function setupLogout() {
        let logoutInput = document.getElementById('logout')

        logoutInput.addEventListener('click', function () {
            window.localStorage.clear()
            routeManager.loadPage(routeManager.PAGES.homePage)
        })
    }

    function setupDeleteNote(){
        let deleteButton = document.getElementById("note-delete-button");
        deleteButton.addEventListener('click', function () {
            let noteParagraph = document.getElementById("create-paragraph-input");
            
            if (noteParagraph.classList.contains("NewNote") && !noteParagraph.textContent.trim()){
                alert("Cannot delete empty note!");
            }
            else{
                let noteid = noteParagraph.getAttribute("noteid");
                deleteNote(noteid);
            }
        })
    }

    async function fetchConfig() {
        // fetch all Url's
        const response = await fetch('../lifelog-config.url.json');
        const data = await response.json();
        webServiceUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteWebService;
        createUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteCreate;
        getUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteGet;
        updateUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteUpdate;
        deleteUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteDelete;
    }

    root.myApp = root.myApp || {};

    // Initialize the current view by setting up data and attaching event handlers 
    async function init() {
        jwtToken = localStorage["token-local"]
        if (jwtToken == null) {
            routeManager.loadPage(routeManager.PAGES.homePage)
        } else {
            var userHash = JSON.parse(jwtToken).Payload.UserHash
            log.logPageAccess(userHash, routeManager.PAGES.personalNotePage, jwtToken)

            await fetchConfig();
            personalNoteDomManipulator.setUp();
            window.name = routeManager.PAGES.personalNotePage
            // Set up event handlers
            setupCreateNoteSubmit();
            setupDeleteNote();
            setupLogout();
            countCharacters();

            // Get data
            showNote();
            currNote();
            //navigate 
            routeManager.setupHeaderLinks();
        }
    }

    init()

}





