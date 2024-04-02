"use strict";

import * as validator from "./reInputValidation.js";

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root, ajaxClient) {
  // Dependency check
  const isValid = root && ajaxClient;

  if (!isValid) {
    // Handle missing dependencies
    alert("Missing dependencies");
  }

  let jwtToken = "";

  const webServiceUrl = "http://localhost:8086/re"; 
  const lliWebServiceUrl = "http://localhost:8080/lli"; 
  const summaryWebServiceUrl = "http://localhost:8085/summary"; 

  root.myApp = root.myApp || {};

  function getRecommendations(numRecs) {
    if (!validator.validateNumRecs(numRecs)) {
      alert("Invalid number of recommendations");
      return Promise.reject(new Error("Invalid number of recommendations"));
    }

    const url = `${webServiceUrl}/NumRecs`;
    return ajaxClient
      .post(url, { numRecs: numRecs })
      .then((response) => response.json())
      .catch((error) => Promise.reject(error));
  }

  function setupGetNumRecomendations() {
    let recomendationButton = document.getElementById("getRecommendation");

    recomendationButton.addEventListener("click", function () {
      let numRecs = document.getElementById("numberRecs").value;
      let response = getRecommendations(numRecs);

      response
        .then(function (recommendationList) {
          let recommendationContainer = document.getElementsByClassName(
            "recommendation-container"
          )[0];
          recommendationContainer.innerHTML = "";
          for (let recommendation of recommendationList) {
            let lliDiv = createLLIDiv(recommendation);
            console.log(lliDiv);
            recommendationContainer.appendChild(lliDiv);
          }
        })
        .catch(function (error) {
          console.log(error);
        });
    });
  }

  function postRecommendation(idPrefix) {


  }

  function repopulateUserDatamart() {
    // populate url with the accurate url for the service
    // TODO : Fix with correct url
    const url = `${summaryWebServiceUrl}/UserRecSummary`;
    return ajaxClient
      .get(url)
      .then((response) => response.json())
      .catch((error) => Promise.reject(error));
  }

  function setupRepopulateUserDatamart() {
    let repopulateUserDatamartButton = document.getElementById("seedRecommendation");

    repopulateUserDatamartButton.addEventListener("click", function () {
      // pass in the user hash
      let response = repopulateUserDatamart();

      response
        .then(() => {
          console.log("Recommendations have been repopulated");
          alert("Recommendations have been repopulated");
        })
        .catch(function (error) {
          console.log(error);
        });
    });
  }

  function repopulateAllUserSummary() {
    // Preform validation to check if admin.
    // If admin, populate the entire summary tableconst 
    const url = `${summaryWebServiceUrl}/AllUserRecSummary`;
    if (true){ // TODO validate user role
      return ajaxClient
      .get(url)
      .then((response) => response.json())
      .catch((error) => Promise.reject(error));
    }
    

    // If not admin, return an error message
  }


  function setupRepopulateAllUserSummary() {
    // Function to setup the repopulate datamart button
    let repopulateAllUserSummaryButton = document.getElementById("seedDatabase");

    repopulateAllUserSummaryButton.addEventListener("click", function () {
      // pass in the user hash
      let response = repopulateAllUserSummary();

      response
        .then(() => {
          console.log("Recommendations db have been repopulated");
          alert("Recommendations db have been repopulated");
        })
        .catch(function (error) {
          console.log(error);
        });
    });
  }


  let formCounter = 0;

  function createLLIDiv(recommendation) {
    formCounter++; // Increment at the beginning to ensure unique IDs across the board

    // Prefix for unique ID
    const idPrefix = `recommendation-${formCounter}`;

    // Create the main recommendation container with unique ID
    const recommendationDiv = document.createElement("div");
    recommendationDiv.className = "lli-recommendation";
    recommendationDiv.id = `${idPrefix}-container`;

    // Title div with unique ID
    const titleDiv = document.createElement("div");
    titleDiv.className = "lli-recommendation-title";
    titleDiv.id = `${idPrefix}-title-div`;
    const titleH2 = document.createElement("h2");
    titleH2.innerText = recommendation.Title;
    titleH2.contentEditable = "true";
    titleH2.id = `${idPrefix}-title`;

    titleDiv.appendChild(titleH2);

    // Categories div with unique ID
    const categoriesDiv = document.createElement("div");
    categoriesDiv.className = "lli-recommendation-categories";
    categoriesDiv.id = `${idPrefix}-categories-div`;
    const categoriesH2 = document.createElement("h2");
    categoriesH2.id = `${idPrefix}-categories`;

    // Building categories text
    let categoriesText = recommendation.Category1;
    if (recommendation.Category2 != null && recommendation.Category2 != "") {
      categoriesText += ", " + recommendation.Category2;
    }
    if (recommendation.Category3 != null && recommendation.Category3 != "") {
      categoriesText += ", " + recommendation.Category3;
    }
    categoriesH2.innerText = categoriesText;

    categoriesDiv.appendChild(categoriesH2);

    // Append the title and categories divs to the recommendation container
    recommendationDiv.appendChild(titleDiv);
    recommendationDiv.appendChild(categoriesDiv);

    // Append form created by createForm function
    recommendationDiv.appendChild(createForm(idPrefix, recommendation)); // Pass idPrefix to maintain consistency
    return recommendationDiv;
  }

  function createForm(idPrefix, recommendation) {
    // Create container div
    const formContainer = document.createElement("div");
    formContainer.className = "form-container";
    formContainer.id = `${idPrefix}-form-container`;

    // Create input area
    const inputArea = createInputArea(idPrefix, recommendation.Description);

    // Create status container
    const statusContainer = createStatusContainer(idPrefix);
    // const statusContainer = document.createElement("div");
    // statusContainer.className = "lli-status-container";
    // statusContainer.innerHTML = `
    //       <h2><span style="font-weight: 600;">Status:</span></h2>
    //       <select name="" id="create-status-input" class="status-select" autocomplete="off">
    //         <option value="Active" selected="selected">Active</option>
    //         <option value="Postponed">Postponed</option>
    //         <option value="Completed">Completed</option>
    //       </select>
    //     `;

    // Create visibility container
    const visibilityContainer = createVisibilityContainer(idPrefix);

    // Create deadline container
    const deadlineContainer = createDeadlineContainer(idPrefix);

    // Create recurrence container
    const recurrenceContainer = createRecurrenceContainer(idPrefix);
    // const recurrenceContainer = document.createElement("div");
    // recurrenceContainer.className = "lli-recurrence-container";
    // recurrenceContainer.innerHTML = `
    //       <h2><span style="font-weight: 600;">Recurrence:</span></h2>
    //       <select name="Recurrence" id="create-recurrence-input" class="recurrence-select" autocomplete="off">
    //         <option value="Off" selected="selected">Off</option>
    //         <option value="On(Weekly)">On(Weekly)</option>
    //         <option value="On(Monthly)">On(Monthly)</option>
    //         <option value="On(Yearly)">On(Yearly)</option>
    //       </select>
    //     `;

    // Create cost container
    const costContainer = createCostContainer(idPrefix, recommendation.Cost);
    // const costContainer = document.createElement("div");
    // costContainer.className = "lli-cost-container";
    // costContainer.innerHTML = `
    //       <h2><span style="font-weight: 600;">Cost:</span></h2>
    //       <input type="number" id="create-cost-input" class="cost-input" placeholder="Enter cost" autocomplete="off">
    //     `;

    // Create recommendation button container
    
    // const RecommendationContainer = createRecommendationContainer(idPrefix)
    const createRecommendationContainer = document.createElement("div");
    createRecommendationContainer.className = "create-recommendation-container";
    createRecommendationContainer.innerHTML = `
          <button id="create-recommendation-button">Create Item</button>
        `;

    // Append all containers to formContainer
    formContainer.appendChild(inputArea);
    formContainer.appendChild(statusContainer);
    formContainer.appendChild(visibilityContainer);
    formContainer.appendChild(deadlineContainer);
    formContainer.appendChild(recurrenceContainer);
    formContainer.appendChild(costContainer);
    formContainer.appendChild(createRecommendationContainer);

    return formContainer;
  }

  function createInputArea(idPrefix, description) {
    const inputArea = document.createElement("div");
    inputArea.className = "input-area";

    // Create the heading for the Description
    const descriptionHeading = document.createElement("h2");
    const descriptionHeadingSpan = document.createElement("span");
    descriptionHeadingSpan.style.fontWeight = "600";
    descriptionHeadingSpan.innerText = "Description:";
    descriptionHeading.appendChild(descriptionHeadingSpan);

    // Create an editable div instead of a textarea, without inline styles
    const descriptionDiv = document.createElement("div");
    descriptionDiv.id = `${idPrefix}-description-input`; // Use unique ID
    descriptionDiv.contentEditable = "true";
    descriptionDiv.innerText = description; // Placeholder text

    // Append the heading and the editable div to the inputArea
    inputArea.appendChild(descriptionHeading);
    inputArea.appendChild(descriptionDiv);

    return inputArea;
  }

  function createStatusContainer(idPrefix) {
    const statusContainer = document.createElement("div");
    statusContainer.className = "lli-status-container";

    // Create the heading for Status
    const statusHeading = document.createElement("h2");
    const statusHeadingSpan = document.createElement("span");
    statusHeadingSpan.style.fontWeight = "600";
    statusHeadingSpan.innerText = "Status:";
    statusHeading.appendChild(statusHeadingSpan);

    // Create the select element for status
    const statusSelect = document.createElement("select");
    statusSelect.id = `${idPrefix}-status-input`; // Use unique ID for the select
    statusSelect.className = "status-select";
    statusSelect.setAttribute("autocomplete", "off");

    // Create and append options to the select element
    const options = [
      { value: "Active", text: "Active", selected: true },
      { value: "Postponed", text: "Postponed" },
      { value: "Completed", text: "Completed" },
    ];

    options.forEach((opt) => {
      const option = document.createElement("option");
      option.value = opt.value;
      option.innerText = opt.text;
      if (opt.selected) option.selected = true;
      statusSelect.appendChild(option);
    });

    // Append the heading and select to the status container
    statusContainer.appendChild(statusHeading);
    statusContainer.appendChild(statusSelect);

    return statusContainer;
  }

  function createVisibilityContainer(idPrefix) {
    const visibilityContainer = document.createElement("div");
    visibilityContainer.className = "lli-visibility-container";

    // Create the heading for Visibility
    const visibilityHeading = document.createElement("h2");
    const visibilityHeadingSpan = document.createElement("span");
    visibilityHeadingSpan.style.fontWeight = "600";
    visibilityHeadingSpan.innerText = "Visibility:";
    visibilityHeading.appendChild(visibilityHeadingSpan);

    // Create the select element for visibility
    const visibilitySelect = document.createElement("select");
    visibilitySelect.name = "Visibility";
    visibilitySelect.id = `${idPrefix}-visibility-input`; // Use unique ID for the select
    visibilitySelect.className = "visibility-select";
    visibilitySelect.setAttribute("autocomplete", "off");

    // Create and append options to the select element
    const options = [
      { value: "Public", text: "Public", selected: true },
      { value: "Private", text: "Private" },
    ];

    options.forEach((opt) => {
      const option = document.createElement("option");
      option.value = opt.value;
      option.innerText = opt.text;
      if (opt.selected) option.setAttribute("selected", "selected");
      visibilitySelect.appendChild(option);
    });

    // Append the heading and select to the visibility container
    visibilityContainer.appendChild(visibilityHeading);
    visibilityContainer.appendChild(visibilitySelect);

    return visibilityContainer;
  }

  function createDeadlineContainer(idPrefix) {
    const deadlineContainer = document.createElement("div");
    deadlineContainer.className = "lli-deadline-container";

    // Create the heading for Deadline
    const deadlineHeading = document.createElement("h2");
    const deadlineHeadingSpan = document.createElement("span");
    deadlineHeadingSpan.style.fontWeight = "600";
    deadlineHeadingSpan.innerText = "Deadline:";
    deadlineHeading.appendChild(deadlineHeadingSpan);

    // Create the date input for deadline
    const deadlineInput = document.createElement("input");
    deadlineInput.type = "date";
    deadlineInput.id = `${idPrefix}-date-input`; // Use unique ID for the date input
    deadlineInput.className = "date-input";
    deadlineInput.setAttribute("autocomplete", "off");

    // Set the min attribute to today's date to prevent selecting earlier dates
    const today = new Date().toISOString().split("T")[0];
    deadlineInput.setAttribute("min", today);

    // Optionally set the value to today or leave it blank
    deadlineInput.value = today;

    // Append the heading and date input to the deadline container
    deadlineContainer.appendChild(deadlineHeading);
    deadlineContainer.appendChild(deadlineInput);

    return deadlineContainer;
  }

  function createRecurrenceContainer(idPrefix, cost) {
    const recurrenceContainer = document.createElement("div");
    recurrenceContainer.className = "lli-recurrence-container";

    // Create the heading for Recurrence
    const recurrenceHeading = document.createElement("h2");
    const recurrenceHeadingSpan = document.createElement("span");
    recurrenceHeadingSpan.style.fontWeight = "600";
    recurrenceHeadingSpan.innerText = "Recurrence:";
    recurrenceHeading.appendChild(recurrenceHeadingSpan);

    // Create the select element for recurrence
    const recurrenceSelect = document.createElement("select");
    recurrenceSelect.name = "Recurrence";
    recurrenceSelect.id = `${idPrefix}-recurrence-input`; // Use unique ID for the select
    recurrenceSelect.className = "recurrence-select";
    recurrenceSelect.setAttribute("autocomplete", "off");

    // Define options for the recurrence select
    const options = [
      { value: "Off", text: "Off", selected: true },
      { value: "On(Weekly)", text: "On(Weekly)" },
      { value: "On(Monthly)", text: "On(Monthly)" },
      { value: "On(Yearly)", text: "On(Yearly)" },
    ];

    // Loop through options to create and append each one to the select element
    options.forEach((opt) => {
      const option = document.createElement("option");
      option.value = opt.value;
      option.innerText = opt.text;
      if (opt.selected) option.selected = true;
      recurrenceSelect.appendChild(option);
    });

    // Append the heading and select to the recurrence container
    recurrenceContainer.appendChild(recurrenceHeading);
    recurrenceContainer.appendChild(recurrenceSelect);

    return recurrenceContainer;
  }

  function createCostContainer(idPrefix, cost) {
    const costContainer = document.createElement("div");
    costContainer.className = "lli-cost-container";

    // Create the heading for Cost
    const costHeading = document.createElement("h2");
    const costHeadingSpan = document.createElement("span");
    costHeadingSpan.style.fontWeight = "600";
    costHeadingSpan.innerText = "Cost:";
    costHeading.appendChild(costHeadingSpan);

    // Create the input element for cost
    const costInput = document.createElement("input");
    costInput.type = "number";
    costInput.id = `${idPrefix}-cost-input`; // Use unique ID for the input
    costInput.className = "cost-input";
    costInput.placeholder = cost;
    costInput.setAttribute("autocomplete", "off");

    // Append the heading and input to the cost container
    costContainer.appendChild(costHeading);
    costContainer.appendChild(costInput);

    return costContainer;
  }
  


  // Initialize the current view by setting up data and attaching event handlers
  function init() {
    jwtToken = localStorage["token-local"];
    setupGetNumRecomendations();
    setupRepopulateUserDatamart();
    setupRepopulateAllUserSummary();
  }

  init();
})(window, window.ajaxClient);
