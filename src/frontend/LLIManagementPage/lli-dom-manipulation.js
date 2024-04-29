import * as log from '../Log/log.js'
export function createLLIComponents(lli, createLLI, getAllLLI, updateLLI, deleteLLI, jwtToken, principal, uploadURL, deleteURL) {
    // Create div element with class "lli" and "expanded-lli"
    const lliDiv = document.createElement('div');
    lliDiv.classList.add('lli');
    lliDiv.id = lli.lliid;

    // Create non-hidden content div
    const nonHiddenContentDiv = document.createElement('div');
    nonHiddenContentDiv.classList.add('lli-non-hidden-content');

    // Title container
    const titleContainer = document.createElement('div');
    titleContainer.classList.add('lli-title-container');
    const titleHeading = document.createElement('h2');
    titleHeading.id = 'title' + lli.lliid;
    titleHeading.textContent = lli.title || 'Enter Title';
    titleContainer.appendChild(titleHeading);

    // Main content container
    const mainContentContainer = document.createElement('div');
    mainContentContainer.classList.add('lli-main-content-container');

    // Deadline container
    const deadlineContainer = document.createElement('div');
    deadlineContainer.classList.add('lli-deadline-container');
    const deadlineHeading = document.createElement('h2');
    deadlineHeading.innerHTML = `<span style="font-weight: 600;">Deadline:</span> <span id="deadline${lli.lliid}"}>${lli.deadline.substring(0, lli.deadline.indexOf(" "))}</span>`;
    deadlineContainer.appendChild(deadlineHeading);

    // Category container
    const categoryContainer = document.createElement('div');
    categoryContainer.classList.add('lli-category-container');
    const categoryHeading = document.createElement('h2');
    categoryHeading.id = 'category' + lli.lliid;

    let categories = ''
    categories += lli.category1
    
    if (lli.category2 != null && lli.category2 != "") {
        categories += `, ${lli.category2}`
    }

    if (lli.category3 != null && lli.category3 != "") {
        categories += `, ${lli.category3}`
    }

    categoryHeading.innerHTML = `<span style="font-weight: 600;"> ${categories|| 'Mental Health'}`
    categoryContainer.appendChild(categoryHeading);

    // Append deadline and category containers to main content container
    mainContentContainer.appendChild(deadlineContainer);
    mainContentContainer.appendChild(categoryContainer);

    // Description container
    const descriptionContainer = document.createElement('div');
    descriptionContainer.classList.add('lli-description-container');
    const descriptionHeading = document.createElement('h2');
    descriptionHeading.textContent = 'Description';
    const descriptionParagraph = document.createElement('p');
    descriptionParagraph.textContent = lli.description || 'A paragraph';
    descriptionParagraph.id = 'description' + lli.lliid
    descriptionContainer.appendChild(descriptionHeading);
    descriptionContainer.appendChild(descriptionParagraph);

    // Append title, main content, and description containers to non-hidden content div
    nonHiddenContentDiv.appendChild(titleContainer);
    nonHiddenContentDiv.appendChild(mainContentContainer);
    nonHiddenContentDiv.appendChild(descriptionContainer);

    // Create hidden content div
    const hiddenContentDiv = document.createElement('div');
    hiddenContentDiv.classList.add('lli-hidden-content', 'hidden');

    // Button container
    const buttonContainer = document.createElement('div');
    buttonContainer.classList.add('lli-button-container');

    // Delete button
    const deleteButton = document.createElement('button');
    deleteButton.id = 'delete-lli-button';

    buttonContainer.appendChild(deleteButton);

    // Edit button
    const editButton = document.createElement('button');
    editButton.id = 'edit-lli-button';

    buttonContainer.appendChild(editButton);

    // Close button
    const closeButton = document.createElement('button');
    closeButton.id = 'close-lli-button';

    buttonContainer.appendChild(closeButton);

    // Append button container to hidden content div
    hiddenContentDiv.appendChild(buttonContainer);

    // Hidden fields container
    const hiddenFieldsContainer = document.createElement('div');
    hiddenFieldsContainer.classList.add('lli-hidden-fields-container');

    // Hidden required fields container
    const hiddenRequiredFieldsContainer = document.createElement('div');
    hiddenRequiredFieldsContainer.classList.add('lli-hidden-required-fields-container')

    // Status container
    const statusContainer = document.createElement('div');
    statusContainer.classList.add('lli-status-container');
    const statusHeading = document.createElement('h2');
    statusHeading.innerHTML = `<span style="font-weight: 600;">Status: </span>`;
    const statusValue = document.createElement('h2');
    statusValue.id = 'status' + lli.lliid;
    statusValue.textContent = lli.status || 'Active';
    statusContainer.appendChild(statusHeading);
    statusContainer.appendChild(statusValue);

    // Visibility container
    const visibilityContainer = document.createElement('div');
    visibilityContainer.classList.add('lli-visibility-container');
    const visibilityHeading = document.createElement('h2');
    visibilityHeading.innerHTML = `<span style="font-weight: 600;">Visibility: </span>`;
    const visibilityValue = document.createElement('h2');
    visibilityValue.id = 'visibility' + lli.lliid;
    visibilityValue.textContent = lli.visibility || 'Public';
    visibilityContainer.appendChild(visibilityHeading);
    visibilityContainer.appendChild(visibilityValue);

    // Append status and visibility containers to hidden fields container
    hiddenRequiredFieldsContainer.appendChild(statusContainer);
    hiddenRequiredFieldsContainer.appendChild(visibilityContainer);
    hiddenFieldsContainer.appendChild(hiddenRequiredFieldsContainer)

    const hiddenNonRequiredFieldsContainer = document.createElement('div')
    hiddenNonRequiredFieldsContainer.classList.add('lli-hidden-non-required-fields-container')

    // Cost container
    const costHeading = document.createElement('h2');
    costHeading.innerHTML = `<span style="font-weight: 600;">Cost:</span> $<span id="cost-input${lli.lliid}">${lli.cost}</span>`;

    // Recurrence container
    const recurrenceContainer = document.createElement('div')
    recurrenceContainer.classList.add('lli-recurrence-container')

    const recurrenceHeading = document.createElement('h2');
    recurrenceHeading.innerHTML = '<span style="font-weight: 600;">Recurring:</span>';
    const recurrenceValue = document.createElement('h2')
    recurrenceValue.innerHTML = `<span id="recurrence-option${lli.lliid}">${lli.recurrence.frequency || 'None'}</span>`
    recurrenceValue.id = 'lli-recurrence-value' + lli.lliid

    recurrenceContainer.appendChild(recurrenceHeading)
    recurrenceContainer.appendChild(recurrenceValue)

    // Append cost and recurrence containers to hidden fields container

    hiddenNonRequiredFieldsContainer.appendChild(costHeading);
    hiddenNonRequiredFieldsContainer.appendChild(recurrenceContainer);
    hiddenFieldsContainer.appendChild(hiddenNonRequiredFieldsContainer);

    // Get the modal
    var modal = document.getElementById("myModal");

    // Get the <span> element that closes the modal
    var span = document.getElementsByClassName("close")[0];

    // Function to open the modal
    function showAlert(message) {
        document.getElementById('modalText').innerText = message;
        modal.style.display = "block";
    }

    // When the user clicks on <span> (x), close the modal
    span.onclick = function() {
        modal.style.display = "none";
    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function(event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    }

    // Media container
    const mediaContainer = document.createElement('div');
    mediaContainer.classList.add('lli-media-container');

    // Image that acts as the upload button and will display the uploaded image
    const mediaImg = document.createElement('img');
    mediaImg.id = 'img' + lli.lliid;
    mediaImg.className = 'lli-media-img';
    mediaImg.src = './Assets/default-pic.svg'; // Default image source
    // Base64 string from lli.mediaMemento
    if(lli.mediaMemento != null) {
        const base64String = lli.mediaMemento;

        // Create Data URL
        const imageDataUrl = `data:image/png;base64,${base64String}`;

        // Set the Data URL as the image source
        mediaImg.src = imageDataUrl;
        // Set the Data URL as the image source
        mediaImg.src = imageDataUrl;
        mediaImg.alt = 'Uploaded Image';
        mediaContainer.appendChild(mediaImg);

        // Create delete button
        const deleteButton = document.createElement('button');
        deleteButton.id = 'delete-button' + lli.lliid;
        deleteButton.className = 'delete-button';
        deleteButton.textContent = 'X';
        deleteButton.addEventListener('mouseover', () => { deleteButton.style.opacity = '1'; });
        deleteButton.addEventListener('mouseout', () => { deleteButton.style.opacity = '0.8'; });


        deleteButton.onclick = function() {
            mediaImg.src = './Assets/default-pic.svg';
            deleteLLIImage(lli.lliid, jwtToken, principal, deleteURL);
            mediaContainer.removeChild(deleteButton);
        };

    // Append the delete button to the container
    // Append the image to the media container
    mediaContainer.appendChild(mediaImg);
    mediaContainer.appendChild(deleteButton);
    }
    
    mediaImg.alt = 'Upload Image';
    mediaImg.style.cursor = 'pointer'; // Make cursor indicate clickable area
    mediaImg.style.transition = 'opacity 0.3s ease'; // Smooth transition for visual feedback
    mediaImg.addEventListener('click', () => {
        fileInput.click(); // Simulate click on file input when image is clicked
    });
    mediaContainer.appendChild(mediaImg);

    // Hidden file input
    const fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.id = 'file-input' + lli.lliid;
    fileInput.accept = 'image/png, image/jpeg';
    fileInput.style.display = 'none'; // Hide the file input element
    fileInput.addEventListener('change', handleImageUpload); // Updated to use your upload and preview function

    // Append file input to the container (could also be elsewhere in the DOM)
    mediaContainer.appendChild(fileInput);

    // Function to handle image upload and preview (instant show of image after selection)
    function handleImageUpload(event) {
        var file = event.target.files[0];
        if (!file) {
            alert('No file selected.');
            return;
        }
        if (file.size > 5242880) {
            alert('The file is too large. Please select a file smaller than 5MB.');
            return;
        }
        var reader = new FileReader();
        reader.onload = function(e) {
            // Update the src of the mediaImg with the loaded image
            mediaImg.src = e.target.result;
            mediaImg.style.maxWidth = '200px';
            mediaImg.style.maxHeight = '200px';
    
            document.getElementById('imageBase64').value = e.target.result.split(',')[1];
            updateLLIImage(lli.lliid, e.target.result.split(',')[1], jwtToken, principal, uploadURL)
        };
        reader.readAsDataURL(file);
        // Create delete button
        const deleteButton = document.createElement('button');
        deleteButton.id = 'delete-button' + lli.lliid;
        deleteButton.className = 'delete-button';
        deleteButton.textContent = 'X';

        var deleteButtonId = 'delete-button' + lli.lliid;
        var existingDeleteButton = document.getElementById(deleteButtonId);

        if (!existingDeleteButton) {
            const deleteButton = document.createElement('button');
            deleteButton.id = deleteButtonId;
            deleteButton.className = 'delete-button';
            deleteButton.textContent = 'X';
            deleteButton.onclick = function() {
                mediaImg.src = './Assets/default-pic.svg';
                deleteLLIImage(lli.lliid, jwtToken, principal, deleteURL);
                mediaContainer.removeChild(deleteButton);
            }
            mediaContainer.appendChild(deleteButton);
        }
    }

    function updateLLIImage(lliid, image, jwtToken, principal, uploadURL) {
        let url = uploadURL;

        const UploadMediaMementoRequest = {
            LLiId: lliid,
            Binary: image,
            AppPrincipal : principal
        }
        return ajaxClient
        .post(url, UploadMediaMementoRequest, jwtToken)
        .then(response => {
            // Check if the response has an error
            if (response.ok) {
                showAlert('The media was successfully uploaded.');
                log.log(principal.userId, "Info", "View", `Media uploaded for LLI ${lliid}`, jwtToken)
            } else {
                showAlert('Upload failed: ' + response.statusText);
                log.log(principal.userId, "ERROR", "View", `Media failed to be uploaded for LLI ${lliid}`, jwtToken)
            }
        })
        .catch((error) => Promise.reject(error), showAlert('Image upload failed'));
        }
    
        
    function deleteLLIImage(lliid, jwtToken, principal){
        let deleteurl = "http://localhost:8091/mediaMemento/DeleteMedia"
        const DeleteMediaMementoRequest = {
            LLiId: lliid,
            AppPrincipal : principal
        }
        
        return ajaxClient
        .post(deleteurl, DeleteMediaMementoRequest, jwtToken)
        .then(response => {
            // Check if the response has an error
            if (response.ok) {
                showAlert('The media was successfully deleted.');
                log.log(userHash, "Info", "View", `Media deleted for LLI ${lliid}`, jwtToken)
            } else {
                showAlert('Delete failed: ' + response.statusText);
                log.log(userHash, "ERROR", "View", `Media failed to be deleted for LLI ${lliid}`, jwtToken)
            }
        })
        .catch((error) => Promise.reject(error), showAlert('Image deletion failed'));
    }

    const imageBase64 = document.createElement('input');
    imageBase64.type = 'hidden';
    imageBase64.id = 'imageBase64';
    mediaContainer.appendChild(imageBase64);

    // Append the media container to your existing DOM (as needed)
    document.body.appendChild(mediaContainer);

    // Append button, hidden fields, and media containers to hidden content div
    hiddenContentDiv.appendChild(buttonContainer);
    hiddenContentDiv.appendChild(hiddenFieldsContainer);
    hiddenContentDiv.appendChild(mediaContainer);

    // Append non-hidden content and hidden content divs to the main lli div
    lliDiv.appendChild(nonHiddenContentDiv);
    lliDiv.appendChild(hiddenContentDiv);

    // Add event listeners
    // Expand the lli
    lliDiv.addEventListener('click', expandDiv)

    // Close the lli
    closeButton.addEventListener('click', async function () {
        encloseDiv()

        await new Promise(r => setTimeout(r, 100)) // Sleep for 1 ms so the function doesnt get call right away
        lliDiv.addEventListener('click', expandDiv)
    })

    function expandDiv() {
        lliDiv.className = 'lli expanded-lli'
        hiddenContentDiv.className = 'lli-hidden-content'
        lliDiv.removeEventListener('click', expandDiv)
    }

    function encloseDiv() {
        lliDiv.className = 'lli'
        hiddenContentDiv.className = 'lli-hidden-content hidden'
    }

    editButton.addEventListener('click', function () {
        convertLLIToEditMode(lli.lliid, updateLLI)
    })

    deleteButton.addEventListener('click', function () {
        deleteLLI(lli.lliid)
    })

    return lliDiv;
}

export function convertLLIToEditMode(id, updateLLI) {
    // Current value
    let currentDeadline = document.getElementById('deadline' + id).textContent
    var parts = currentDeadline.split('/');
    currentDeadline = parts[2] + '-' + parts[0].padStart(2, '0') + '-' + parts[1].padStart(2, '0')

    let currentCategories = document.getElementById('category' + id).textContent.split(",").map(item => item.trim());
    let currentStatus = document.getElementById('status' + id).textContent
    let currentVisibility = document.getElementById('visibility' + id).textContent
    let currentRecurrence = document.getElementById('recurrence-option' + id).textContent

    const lliDiv = document.getElementById(id);
    if (!lliDiv) {
        console.error('LLI div with specified id not found.');
        return;
    }

    // Change title to contenteditable
    const title = lliDiv.querySelector('#title' + id);
    if (title) {
        title.contentEditable = true;
        title.id = 'update-title-input' + id
    }

    // Change deadline to input type date
    const deadline = lliDiv.querySelector('.lli-deadline-container h2');
    if (deadline) {
        const dateInput = document.createElement('input');
        dateInput.classList.add('date-input')
        dateInput.id = 'update-date-input' + id
        dateInput.type = 'date';
        dateInput.value = currentDeadline
        deadline.innerHTML = '<span style="font-weight: 600;">Deadline:</span> ';
        deadline.appendChild(dateInput);
    }

    // Change category to select options
    const category = lliDiv.querySelector('.lli-category-container');
    if (category) {
        const categorySelect = document.createElement('select');
        categorySelect.id = 'update-category-input' + id;
        categorySelect.setAttribute("multiple", "multiple");
        const categories = ['Mental Health', 'Physical Health', 'Outdoor', 'Sport', 'Art', 'Hobby', 'Thrill', 'Travel', 'Volunteering', 'Food'];
        categories.forEach(cat => {
            const option = document.createElement('option');
            option.value = cat;
            option.textContent = cat;
            if (currentCategories.includes(cat)) {
                option.selected = 'selected'
            }
            categorySelect.appendChild(option);
        });
        category.innerHTML = '';
        category.appendChild(categorySelect);
    }

    // Change description to contenteditable
    const description = lliDiv.querySelector('.lli-description-container p');
    if (description) {
        description.contentEditable = true;
        description.id = 'update-paragraph-input' + id
    }

    // Change status to select options
    const status = lliDiv.querySelector('.lli-status-container');
    if (status) {
        const statusSelect = document.createElement('select');
        statusSelect.classList.add('status-select')
        statusSelect.id = 'update-status-input' + id;
        const statusOptions = ['Active', 'Completed', 'Postponed'];
        statusOptions.forEach(opt => {
            const option = document.createElement('option');
            option.value = opt;
            option.textContent = opt;
            if (opt == currentStatus) {
                option.selected = 'selected'
            }
            statusSelect.appendChild(option);
        });
        status.innerHTML = '<h2><span style="font-weight: 600;">Status: </span></h2>';
        status.appendChild(statusSelect);
    }

    // Change visibility to select options
    const visibility = lliDiv.querySelector('.lli-visibility-container');
    if (visibility) {
        const visibilitySelect = document.createElement('select');
        visibilitySelect.classList.add('visibility-select')
        visibilitySelect.id = 'update-visibility-input' + id;
        const visibilityOptions = ['Public', 'Private'];
        visibilityOptions.forEach(opt => {
            const option = document.createElement('option');
            option.value = opt;
            option.textContent = opt;
            if (opt == currentVisibility) {
                option.selected = 'selected'
            }
            visibilitySelect.appendChild(option);
        });
        visibility.innerHTML = '<h2><span style="font-weight: 600;">Visibility: </span></h2>';
        visibility.appendChild(visibilitySelect);
    }

    // Change cost to contenteditable
    const cost = lliDiv.querySelector('#cost-input' + id);
    if (cost) {
        cost.contentEditable = true;
        cost.id = 'update-cost-input' + id
    }

    // Change recurrence to select options
    const recurrence = lliDiv.querySelector('#lli-recurrence-value' + id);
    if (recurrence) {
        const recurrenceSelect = document.createElement('select');
        recurrenceSelect.classList.add('recurrence-select')
        recurrenceSelect.id = 'update-recurrence-input' + id;
        const recurrenceOptions = ['Off', 'Weekly', 'Monthly', 'Yearly'];
        recurrenceOptions.forEach(opt => {
            const option = document.createElement('option');
            option.value = opt;
            option.textContent = opt;
            if (opt == currentRecurrence || (opt == "Off" && currentRecurrence == "None")) {
                option.selected = 'selected'
            }
            recurrenceSelect.appendChild(option);
        });
        recurrence.parentNode.appendChild(recurrenceSelect);
        recurrence.parentNode.removeChild(recurrence)

    }

    const lliHiddenContentContainer = lliDiv.querySelector('.lli-hidden-content')

    // Create a div element with class "create-lli-button-container"
    const editLLIButtonContainer = document.createElement('div');
    editLLIButtonContainer.classList.add('create-lli-button-container');

    // Create a button element with id "create-lli-button" and text "Create LLI"
    const editLLIButton = document.createElement('button');
    editLLIButton.id = 'create-lli-button';
    editLLIButton.textContent = 'Edit LLI';

    // Append the button to the button container
    editLLIButtonContainer.appendChild(editLLIButton);

    // Add event listener to edit button
    editLLIButton.addEventListener('click', function () {
        let title = document.getElementById('update-title-input' + id).textContent
        let deadline = document.getElementById('update-date-input' + id).value
        let categories = document.getElementById('update-category-input' + id)

        const MAX_NUM_OF_CATEGORIES = 3;
        var selectedCategories = [];
        if (categories)
        {
            for (var i = 0; i < categories.options.length; i++) {
                var option = categories.options[i];
                if (option.selected && selectedCategories.length < MAX_NUM_OF_CATEGORIES) {
                    selectedCategories.push(option.value);
                }
            }
        }
        else {
            selectedCategories = null;
        }
        

        let description = document.getElementById('update-paragraph-input' + id).textContent
        let status = document.getElementById('update-status-input' + id).value
        let visibility = document.getElementById('update-visibility-input' + id).value
        let cost = document.getElementById('update-cost-input' + id).textContent
        let recurrence = document.getElementById('update-recurrence-input' + id).value

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
            lliid: id,
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

        updateLLI(options)

    })

    lliHiddenContentContainer.appendChild(editLLIButtonContainer)


    return lliDiv.outerHTML;
}

export function filterLLI(filterOption, filterContainer) {
    const categoriesList = new Set([
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

    const statusList = new Set([
        "Active",
        "Completed",
        "Postponed"
    ])

    const visibilityList = new Set([
        "Public",
        "Private"
    ])

    const timeOutConstantSecond = 5;

    // Validate filter options
    if (filterOption.label == "Filter" && filterOption.values != "None") {
        for (const val of filterOption.values) {
            if (!categoriesList.has(val) && !statusList.has(val) && !visibilityList.has(val)) {
                window.alert("The filter selections are invalid, please try again.")
                return
            }
        }
    }

    // Validate search query
    if (filterOption.label == "Search") {
        if (filterOption.values == null || filterOption.values == "" || !filterOption.values.toLowerCase().match(/^[0-9a-z]+$/) || filterOption.values.length > 50) {
            window.alert('The search query is invalid, please try again.')
            return
        }
    }

    let containerDiv
    let lliDivList

    let numberOfHiddenLLI = 0
    
    let numberOfDisplayedLLI

    let context

    if (filterContainer.id.includes('current')) {
        context = 'current'
        containerDiv = document.getElementsByClassName('current-lli-content-container')[0]
        lliDivList = containerDiv.getElementsByClassName('lli')
        numberOfDisplayedLLI = lliDivList.length - 1
    } else {
        context = 'finished'
        containerDiv = document.getElementsByClassName('finished-lli-content-container')[0]
        lliDivList = containerDiv.getElementsByClassName('lli')
        numberOfDisplayedLLI = lliDivList.length
    }

    const timeAtStart = performance.now()

    for (let lliDiv of lliDivList) {
        // Check for timeout
        let timeNow = performance.now()
        if ((timeNow - timeAtStart) / 1000 > timeOutConstantSecond) {
            if (filterOption.label == "Filter") alert('The operation took too long. Please try again later.')
            else if(filterOption.label == "Search") alert('The LLI search operation took too long.')
            return
        }

        if (lliDiv.id.includes('create')) continue // Skip the create template lli

        let isValid = true // Keep track of if the lli is valid

        if (filterOption.label == 'Search') {
            let lliDivTitle = document.getElementById('title' + lliDiv.id).textContent
            let lliDivDescription = document.getElementById('description' + lliDiv.id).textContent

            if (!lliDivTitle.includes(filterOption.values) && !lliDivDescription.includes(filterOption.values)) {
                lliDiv.classList.add('hidden')
                numberOfHiddenLLI += 1
                numberOfDisplayedLLI -= 1
            }
            else {
                if (lliDiv.classList.contains('hidden')) {
                    lliDiv.classList.remove('hidden')
                    numberOfHiddenLLI -= 1
                    numberOfDisplayedLLI += 1
                }
            }
            continue
        }

        if (filterOption.values == 'None') {
            if (lliDiv.classList.contains('hidden')) {
                lliDiv.classList.remove('hidden')
                numberOfHiddenLLI -= 1
                numberOfDisplayedLLI += 1
            }
            continue
        }

        if (isValid && filterOption.values.some(val => categoriesList.has(val))) {
            let lliDivCategories = document.getElementById('category' + lliDiv.id).textContent

            for (const val of filterOption.values) {
                if (!categoriesList.has(val)) continue
                if (!lliDivCategories.includes(val)) {
                    isValid = false
                }
            }
        } if (isValid && filterOption.values.some(val => statusList.has(val))) {
            let lliDivStatus = document.getElementById('status' + lliDiv.id).textContent

            for (const val of filterOption.values) {
                if (!statusList.has(val)) continue
                if (!lliDivStatus.includes(val)) {
                    isValid = false
                }
            }

        } if (isValid && filterOption.values.some(val => visibilityList.has(val))) {
            let lliDivVisibility = document.getElementById('visibility' + lliDiv.id).textContent

            for (const val of filterOption.values) {
                if (!visibilityList.has(val)) continue
                if (!lliDivVisibility.includes(val)) {
                    isValid = false
                }
            }
        }

        if (!isValid) {
            lliDiv.classList.add('hidden')
            numberOfHiddenLLI += 1
            numberOfDisplayedLLI -= 1
        }
        else {
            if (lliDiv.classList.contains('hidden')) {
                lliDiv.classList.remove('hidden')
                numberOfHiddenLLI -= 1
                numberOfDisplayedLLI += 1
            }
        }

    }


    if (context == 'current' && numberOfHiddenLLI == lliDivList.length - 1/*Not accounting for the create template*/) {
        alert('No LLI found. Please try again.')
    } else if(context == 'finished' && numberOfHiddenLLI == lliDivList.length) {
        alert('No LLI found. Please try again.')
    }

    if (context == 'current' && numberOfDisplayedLLI + numberOfHiddenLLI != lliDivList.length - 1) {
        if (filterOption.label == "Filter") alert('Failed to get all LLIs from the filter. Please try again.')
        else if (filterOption.label == "Search") alert('Failed to get all LLIs from the search. Please try again.')
    } else if (context == 'hidden' && numberOfDisplayedLLI + numberOfHiddenLLI != lliDivList.length) {
        if (filterOption.label == "Filter") alert('Failed to get all LLIs from the filter. Please try again.')
        else if (filterOption.label == "Search") alert('Failed to get all LLIs from the search. Please try again.')
    }

}

