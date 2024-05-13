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
    let early_date = new Date();
    early_date.setMonth(early_date.getMonth() - 6);
    const EARLIEST_NOTE_DATE = early_date;
    const LATEST_NOTE_DATE = new Date();
    LATEST_NOTE_DATE.setDate(LATEST_NOTE_DATE.getDate() + 1);
    const MAX_CONTENT_LENGTH = 1200
    const MAX_DATE = 2100
    const EARLIEST_NOTE_YEAR = 1960
    const LATEST_NOTE_YEAR = new Date().getFullYear();

    let jwtToken = ""

    let personalNotewebServiceUrl = ""
    let personalNotegetUrl = ""
    let personalNotecreateUrl = ""

    let lifetreewebServiceUrl = ""
    let getCompletedLLIUrl = ""


    // NOT exposed to the global object ("Private" functions)

    // HTTP REQUESTS -----------------------------------------------------------
    function getCompletedLLI() {
        let getDataUrl = lifetreewebServiceUrl + getCompletedLLIUrl

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
        let get_Url = personalNotewebServiceUrl + personalNotegetUrl + notedate;
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


    function createNote(options) {
        let createPersonalNoteUrl = personalNotewebServiceUrl + personalNotecreateUrl;

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

            let leftrightBranch = {
                "left" : ["left-branch", "Assets/Left Branch Topdown.svg", "Left Branch", "left-text", "left-branch" , "btn"], 
                "right" : ["right-branch", "Assets/Right Branch Topdown.svg", "Right Branch", "right-text", "right-branch", "right-btn"]
            }

            let treeFragment = document.createDocumentFragment()

            // process each lli like this
            data.output.forEach(function (lli, index) {
                let leftorright
                if (index % 2 == 1){
                    leftorright = "right"
                }
                else{
                    leftorright = "left"
                }

                

                let div = document.createElement('div')
                div.classList.add("tree-container")

                let img = document.createElement('img')
                img.classList.add("tree-segment")
                img.classList.add(leftrightBranch[leftorright][0])
                img.src = leftrightBranch[leftorright][1]
                img.alt = leftrightBranch[leftorright][2]

                let button = document.createElement('button')
                button.id = `button-${lli.lliid}`
                button.setAttribute('data-modal-target', '#lli-modal-edit');
                button.classList.add("lli-title-text")
                button.classList.add(leftrightBranch[leftorright][3])
                button.classList.add(leftrightBranch[leftorright][4])
                button.classList.add(leftrightBranch[leftorright][5])

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

            // render modal js inside this thread
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
                renderModals()

            // user story 2 
            let selectAllButtons = document.querySelectorAll("button.lli-title-text")
            selectAllButtons.forEach((eachButton) => {
                eachButton.addEventListener('click', () => {

                    let lliidToFind = eachButton.id.split("-")[1];


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
            })

        });
    }


    function showNote() {
        let dateInput = document.getElementById("create-date-input-pn");
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
        console.log(selectedDate)

        // Call getNote function to retrieve note data
        getNote(selectedDate).then(function (noteText) {
            let noteParagraph = document.getElementById("create-paragraph-input-pn");
            let noteButton = document.getElementById("create-pn-btn");
            if(noteText == null)
            {
                noteParagraph.textContent = '';
                noteParagraph.placeholder = "What are you thinking?......";
                
                noteParagraph.classList.add("NewNote");
                noteParagraph.setAttribute("noteid", "");

                noteButton.disabled = false;
                
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

                
                noteButton.disabled = true;
                
            }
        })
        .catch(function (error) {
            // Handle error if retrieval fails
            alert("Error retrieving note: " + error);
        });
    }

    function currNote () {
        let date_input = document.getElementById("create-date-input-pn");
        
        date_input.onchange = function (){
            showNote();
        }
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

        let noteParagraph = document.getElementById("create-paragraph-input-pn");
        let noteButton = document.getElementById("create-pn-btn");

        noteParagraph.classList.remove("NewNote");
        
        if (!noteParagraph.classList.contains("NewNote")){
            noteButton.disabled = true;
        }



    }

    function setupLogout() {
        let logoutInput = document.getElementById('logout')

        logoutInput.addEventListener('click', function () {
            window.localStorage.clear()
            routeManager.loadPage(routeManager.PAGES.homePage)
        })
    }

    root.myApp = root.myApp || {};

    async function fetchConfig() {
        // fetch all Url's
        const response = await fetch('../lifelog-config.url.json');
        const data = await response.json();
        personalNotewebServiceUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteWebService;
        personalNotegetUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteGet;
        personalNotecreateUrl = data.LifelogUrlConfig.PersonalNote.PersonalNoteCreate;

        lifetreewebServiceUrl = data.LifelogUrlConfig.Lifetree.LifetreeWebService;
        getCompletedLLIUrl = data.LifelogUrlConfig.Lifetree.GetCompletedLLI;
        
    }

    // Initialize the current view by attaching event handlers 
    async function init() {

        jwtToken = localStorage["token-local"]

        if (jwtToken == null) { // change to ==
            alert("Unauthorized User In View")
            routeManager.loadPage(routeManager.PAGES.homePage)
        } else {
            let timeAccessed = performance.now()
            routeManager.setupHeaderLinks(routeManager.PAGES.lifetreePage, timeAccessed, jwtToken);

            //add main calls here
            await fetchConfig();
            showLifetree()

            let viewPNBtn = document.getElementById("pn-button")
            viewPNBtn.addEventListener('click', () => {

                showNote()
                currNote()

            })

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
            renderModals()

            let createPNBtn = document.getElementById("create-pn-btn")
            createPNBtn.addEventListener('click', () => {

                pnCreation()

            })
            

        }
    }

    init();

    // let nextMonth = document.getElementById("get-next-month")
    // nextMonth.addEventListener("click", onClickGetNextMonth)

}



