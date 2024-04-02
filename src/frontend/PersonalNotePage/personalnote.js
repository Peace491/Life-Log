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
    function getNote(notedate) {
        let getUrl = webServiceUrl + '/getPN?notedate=' + notedate;
        console.log(getUrl)
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
                    throw new Error(response.statusText)
                }
                console.log(response.body)
                return response.json();
            }).then(function (data) {
                console.log("Note Data: ", data.output);
                let output = data.output;
                resolve(output);
            }).catch(function (error) {
                console.log("error" + error)
                alert(error)
                reject(error);
            });
        });
    }

    function updateNote(options) {
        let updateLLIUrl = webServiceUrl + '/putPN'

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

    function deleteNote(noteid) {
        let deleteUrl = webServiceUrl + '/deletePN?noteid=' + noteid;
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
            alert(`The Note date must be between 01/01/${EARLIEST_NOTE_YEAR} and 12/31/${LATEST_NOTE_YEAR}, please try again.`)
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
                console.log("this updated")
                updateNote(options);
            }
            
        })
    }

    function showNote() {
        let dateInput = document.getElementById("create-date-input");
        let selectedDate = dateInput.value;
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
            location.reload()
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
    root.myApp = root.myApp || {};

    // Initialize the current view by setting up data and attaching event handlers 
    function init() {
        jwtToken = localStorage["token-local"]
        console.log("Jwt Token: "+jwtToken)
        if (jwtToken == null) {
            window.location = '../HomePage/index.html'
        } else {
            // Set up event handlers
            setupCreateNoteSubmit();
            setupDeleteNote();
            setupLogout();
            countCharacters();

            // Get data
            showNote();
            currNote();

            //navigate 
            const router = new Router;
            router.navigatePages();
        }
    }

    document.addEventListener('DOMContentLoaded', function() {
        init();
    });

})(window, window.ajaxClient);





