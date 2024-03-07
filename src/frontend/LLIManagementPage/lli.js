'use strict';

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if(!isValid){
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    const webServiceUrl = 'http://localhost:8080/lli';

    // NOT exposed to the global object ("Private" functions)
    function getAllLLI() {
        
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
    
    function deleteLLI(lliid) {
        let deleteUrl = webServiceUrl + '/deleteLLI?userHash=System&lliID=' + lliid;
        console.log(deleteUrl)
        let request = ajaxClient.del(deleteUrl);
        
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

    function createLLIHTML(lli) {
        const container = document.createElement('div');
        container.className = 'lli';
    
        const nonHiddenContent = document.createElement('div');
        nonHiddenContent.className = 'lli-non-hidden-content';
    
        const titleContainer = document.createElement('div');
        titleContainer.className = 'lli-title-container';
        const title = document.createElement('h2');
        title.textContent = lli.title;
        titleContainer.appendChild(title);
        nonHiddenContent.appendChild(titleContainer);
    
        const mainContentContainer = document.createElement('div');
        mainContentContainer.className = 'lli-main-content-container';
    
        const deadlineContainer = document.createElement('div');
        deadlineContainer.className = 'lli-deadline-container';
        const deadline = document.createElement('h2');
        deadline.innerHTML = '<span style="font-weight: 600;">Deadline:</span> ' + lli.deadline.substring(0, lli.deadline.indexOf(" "));
        deadlineContainer.appendChild(deadline);
        mainContentContainer.appendChild(deadlineContainer);
    
        const categoryContainer = document.createElement('div');
        categoryContainer.className = 'lli-category-container';
        const category = document.createElement('h2');
        category.innerHTML = '<span style="font-weight: 600;">' + lli.category + '</span>';
        categoryContainer.appendChild(category);
        mainContentContainer.appendChild(categoryContainer);
    
        nonHiddenContent.appendChild(mainContentContainer);
    
        const descriptionContainer = document.createElement('div');
        descriptionContainer.className = 'lli-description-container';
        const descriptionTitle = document.createElement('h2');
        descriptionTitle.textContent = 'Description';
        const description = document.createElement('p');
        description.textContent = lli.description;
        descriptionContainer.appendChild(descriptionTitle);
        descriptionContainer.appendChild(description);
        nonHiddenContent.appendChild(descriptionContainer);
    
        container.appendChild(nonHiddenContent);
    
        const hiddenContent = document.createElement('div');
        hiddenContent.className = 'lli-hidden-content hidden';
    
        const buttonContainer = document.createElement('div');
        buttonContainer.className = 'lli-button-container';
        const deleteButton = document.createElement('button');
        deleteButton.id = 'delete-lli-button';
        const editButton = document.createElement('button');
        editButton.id = 'edit-lli-button';
        const closeButton = document.createElement('button');
        closeButton.id = 'close-lli-button';
        buttonContainer.appendChild(deleteButton);
        buttonContainer.appendChild(editButton);
        buttonContainer.appendChild(closeButton);
        hiddenContent.appendChild(buttonContainer);
    
        const hiddenFieldsContainer = document.createElement('div');
        hiddenFieldsContainer.className = 'lli-hidden-fields-container';
    
        const hiddenRequiredFieldsContainer = document.createElement('div');
        hiddenRequiredFieldsContainer.className = 'lli-hidden-required-fields-container';
        const status = document.createElement('h2');
        status.innerHTML = '<span style="font-weight: 600;">Status:</span> ' + lli.status;
        const visibility = document.createElement('h2');
        visibility.innerHTML = '<span style="font-weight: 600;">Visibility:</span> ' + lli.visibility;
        hiddenRequiredFieldsContainer.appendChild(status);
        hiddenRequiredFieldsContainer.appendChild(visibility);
        hiddenFieldsContainer.appendChild(hiddenRequiredFieldsContainer);
    
        const hiddenNonRequiredFieldsContainer = document.createElement('div');
        hiddenNonRequiredFieldsContainer.className = 'lli-hidden-non-required-fields-container';
        const cost = document.createElement('h2');
        cost.innerHTML = '<span style="font-weight: 600;">Cost:</span> $' + lli.cost;
        const recurrencyFrequency = document.createElement('h2');
        recurrencyFrequency.innerHTML = '<span style="font-weight: 600;">Recurrency Frequency:</span> ' + lli.recurrencyFrequency;
        hiddenNonRequiredFieldsContainer.appendChild(cost);
        hiddenNonRequiredFieldsContainer.appendChild(recurrencyFrequency);
        hiddenFieldsContainer.appendChild(hiddenNonRequiredFieldsContainer);
    
        hiddenContent.appendChild(hiddenFieldsContainer);
    
        const mediaContainer = document.createElement('div');
        mediaContainer.className = 'lli-media-container';
        const image = document.createElement('img');
        image.src = './Assets/default-pic.svg';
        image.alt = '';
        mediaContainer.appendChild(image);
        hiddenContent.appendChild(mediaContainer);
    
        container.appendChild(hiddenContent);
    
        // Add event listeners
        // Expand the lli
        container.addEventListener('click', expandDiv)
    
        // Close the lli
        closeButton.addEventListener('click', async function() {
            encloseDiv()
            
            await new Promise(r => setTimeout(r, 100)) // Sleep for 1 ms so the function doesnt get call right away
            container.addEventListener('click', expandDiv)
        })
    
        function expandDiv() {
            container.className = 'lli expanded-lli'
            hiddenContent.className = 'lli-hidden-content'
            container.removeEventListener('click',expandDiv)
        }
    
        function encloseDiv() {
            container.className = 'lli'
            hiddenContent.className = 'lli-hidden-content hidden'
        }

        deleteButton.addEventListener('click', function() {
            let response = deleteLLI(lli.lliid).then(function(output) {
                if (output.hasError == false) {
                    container.parentNode.removeChild(container)
                }
                
            })
            // Need to check if the previous call succeed before calling this
            
            
            
        })
        
        return container;
    }


    root.myApp = root.myApp || {};

    // Show or Hide private functions
    //root.myApp.getData = getDataHandler;
    //root.myApp.sendData = sendDataHandler;

    // Initialize the current view by attaching event handlers 
    function init() {
        let lliContentContainer = document.getElementsByClassName("current-lli-content-container")[0]
        let finishedLLIContentContainer = document.getElementsByClassName("finished-lli-content-container")[0]

        getAllLLI().then(function (completedLLIList) {
            completedLLIList.reverse().forEach(lli => {
                let lliHTML = createLLIHTML(lli);
                if (lli.status != "Completed") {
                    lliContentContainer.append(lliHTML);
                }
                else {
                    finishedLLIContentContainer.append(lliHTML);
                }
            });
        });
    }

    init();

})(window, window.ajaxClient);

function expandLLIHTML(container, hiddenContent) {
    

}





