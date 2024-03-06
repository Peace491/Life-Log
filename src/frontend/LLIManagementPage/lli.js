'use strict';

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    console.log(root)
    console.log(ajaxClient)

    if(!isValid){
        // Handle missing dependencies
        alert("Missing dependencies");
    }


    // NOT exposed to the global object ("Private" functions)
    function getAllLLI() {
        const webServiceUrl = 'http://localhost:8080/lli';
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


    root.myApp = root.myApp || {};

    // Show or Hide private functions
    //root.myApp.getData = getDataHandler;
    //root.myApp.sendData = sendDataHandler;

    // Initialize the current view by attaching event handlers 
    function init() {
        let lliContentContainer = document.getElementsByClassName("current-lli-content-container")[0]

        getAllLLI().then(function (completedLLIList) {
            completedLLIList.reverse().forEach(lli => {
                let lliHTML = createLLIHTML(lli);
                lliContentContainer.append(lliHTML);
            });
        });
    }

    init();

})(window, window.ajaxClient);

function createLLIHTML(lli) {
    // Create elements
    var lliContainer = document.createElement("div");
    lliContainer.classList.add("lli");
    lliContainer.id = lli.lliid

    var lliNonHiddenContentContainer = document.createElement("div")
    lliNonHiddenContentContainer.classList.add("lli-non-hidden-content")

    var titleContainer = document.createElement("div");
    titleContainer.classList.add("lli-title-container");

    var titleElement = document.createElement("h2");
    titleElement.textContent = lli.title;

    var mainContentContainer = document.createElement("div");
    mainContentContainer.classList.add("lli-main-content-container");

    var deadlineContainer = document.createElement("div");
    deadlineContainer.classList.add("lli-deadline-container");

    var deadlineElement = document.createElement("h2");
    deadlineElement.textContent = "Deadline: " + lli.deadline.substring(0, lli.deadline.indexOf(" "));

    var categoryContainer = document.createElement("div");
    categoryContainer.classList.add("lli-category-container");

    var categoryElement = document.createElement("h2");
    categoryElement.textContent = lli.category;

    var descriptionContainer = document.createElement("div");
    descriptionContainer.classList.add("lli-description-container");

    var descriptionTitle = document.createElement("h2");
    descriptionTitle.textContent = "Description";

    var descriptionParagraph = document.createElement("p");
    descriptionParagraph.textContent = lli.description;

    // Append elements
    descriptionContainer.appendChild(descriptionTitle);
    descriptionContainer.appendChild(descriptionParagraph);

    deadlineContainer.appendChild(deadlineElement);
    categoryContainer.appendChild(categoryElement);

    mainContentContainer.appendChild(deadlineContainer);
    mainContentContainer.appendChild(categoryContainer);

    titleContainer.appendChild(titleElement);

    lliNonHiddenContentContainer.appendChild(titleContainer);
    lliNonHiddenContentContainer.appendChild(mainContentContainer);
    lliNonHiddenContentContainer.appendChild(descriptionContainer);

    lliContainer.appendChild(lliNonHiddenContentContainer)

    // Return the created element
    return lliContainer;
}
