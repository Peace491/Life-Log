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

  const webServiceUrl = "http://localhost:8086/re"; // into config
  const lliWebServiceUrl = "http://localhost:8080/lli"; // into config

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

  function repopulateDatamart() { 
    // populate url with the accurate url for the service
    // TODO : Fix with correct url
    const url = `${webServiceUrl}/NumRecs`;
    return ajaxClient
      .get(url)
      .then((response) => response.json())
      .catch((error) => Promise.reject(error));
  }

  function setupRepopulateUserSummary() {
    // Function to setup the repopulate datamart button
  }

  function repopulateEntireSummaryTable() {
    // Preform validation to check if admin. 
    // If admin, populate the entire summary table

    // If not admin, return an error message
    // TODO: Fix with correct url
    const url = `${webServiceUrl}/NumRecs`;
    return ajaxClient
      .get(url)
      .then((response) => response.json())
      .catch((error) => Promise.reject(error));
  }

  function setupRepopulateEntireSummaryTable() {

  }

  function createLLIDiv(recommendation) {
    // Create the main recommendation container
    const recommendationDiv = document.createElement("div");
    recommendationDiv.className = "lli-recommendation";

    const titleDiv = document.createElement("div");
    titleDiv.className = "lli-recommendation-title";
    const titleH2 = document.createElement("h2");
    titleH2.innerText = recommendation.Title;
    titleH2.contentEditable = "true"; // Make the h2 element editable

    titleDiv.appendChild(titleH2);

    const categoriesDiv = document.createElement("div");
    categoriesDiv.className = "lli-recommendation-categories";
    const categoriesH2 = document.createElement("h2");

    categoriesH2.innerText = recommendation.Category1;
    if (recommendation.Category2 != null && recommendation.Category2 != "") {
      categoriesH2.innerText += ", " + recommendation.Category2;
    }
    if (recommendation.Category3 != null && recommendation.Category3 != "") {
      categoriesH2.innerText += ", " + recommendation.Category3;
    }

    categoriesDiv.appendChild(categoriesH2);

    // deadlineDiv.appendChild(createDeadlineInputDiv());

    // Append the title and categories divs to the LLI recomendations container
    recommendationDiv.appendChild(titleDiv);
    recommendationDiv.appendChild(categoriesDiv);
    recommendationDiv.appendChild(createForm());
    return recommendationDiv;
  }

  function createForm() {
    // Create container div
    const formContainer = document.createElement("div");
    formContainer.className = "form-container";

    // Create input area
    const inputArea = document.createElement("div");
    inputArea.className = "input-area";
    inputArea.innerHTML = `
          <h2><span style="font-weight: 600;">Description:</span></h2>
          <textarea id="description-input" required>Enter Description</textarea>
        `;

    // Create status container
    const statusContainer = document.createElement("div");
    statusContainer.className = "lli-status-container";
    statusContainer.innerHTML = `
          <h2><span style="font-weight: 600;">Status:</span></h2>
          <select name="" id="create-status-input" class="status-select" autocomplete="off">
            <option value="Active" selected="selected">Active</option>
            <option value="Postponed">Postponed</option>
            <option value="Completed">Completed</option>
          </select>
        `;

    // Create visibility container
    const visibilityContainer = document.createElement("div");
    visibilityContainer.className = "lli-visibility-container";
    visibilityContainer.innerHTML = `
          <h2><span style="font-weight: 600;">Visibility:</span></h2>
          <select name="Visibility" id="create-visibility-input" class="visibility-select" autocomplete="off">
            <option value="Public" selected="selected">Public</option>
            <option value="Private">Private</option>
          </select>
        `;

    // Create deadline container
    const deadlineContainer = document.createElement("div");
    deadlineContainer.className = "lli-deadline-container";
    deadlineContainer.innerHTML = `
          <h2><span style="font-weight: 600;">Deadline:</span></h2>
          <input type="date" id="create-date-input" class="date-input" value="2024-11-11" autocomplete="off">
        `;

    // Create recurrence container
    const recurrenceContainer = document.createElement("div");
    recurrenceContainer.className = "lli-recurrence-container";
    recurrenceContainer.innerHTML = `
          <h2><span style="font-weight: 600;">Recurrence:</span></h2>
          <select name="Recurrence" id="create-recurrence-input" class="recurrence-select" autocomplete="off">
            <option value="Off" selected="selected">Off</option>
            <option value="On(Weekly)">On(Weekly)</option>
            <option value="On(Monthly)">On(Monthly)</option>
            <option value="On(Yearly)">On(Yearly)</option>
          </select>
        `;

    // Create cost container
    const costContainer = document.createElement("div");
    costContainer.className = "lli-cost-container";
    costContainer.innerHTML = `
          <h2><span style="font-weight: 600;">Cost:</span></h2>
          <input type="number" id="create-cost-input" class="cost-input" placeholder="Enter cost" autocomplete="off">
        `;

    // Create recommendation button container
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

  // Initialize the current view by setting up data and attaching event handlers
  function init() {
    jwtToken = localStorage["token-local"];
    setupGetNumRecomendations();
  }

  init();
})(window, window.ajaxClient);
