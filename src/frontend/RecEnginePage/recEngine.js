"use strict";

import * as validator from "./recEngineInputValidation.js";
import * as routeManager from "../routeManager.js";

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
export function loadRecEnginePage (root, ajaxClient) {
  // Dependency check
  const isValid = root && ajaxClient;

  if (!isValid) {
    // Handle missing dependencies
    alert("Missing dependencies");
  }

  let jwtToken = "";
  let principal = { };


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
      .post(url, { 
        appPrincipal: principal,
        numRecs: numRecs 
      }, 
      jwtToken
      )
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
          console.log(recommendationContainer);
          recommendationContainer.innerHTML = "";
          for (let recommendation of recommendationList) {
            console.log(recommendation);
            let result = createLLIDiv(recommendation);
            console.log(result[0]);
            recommendationContainer.appendChild(result[0]);
            setupPostLLI(result[1]);
          }
        })
        .catch(function (error) {
          console.log(error);
        });
    });
  }

  function postLLI(options) {
    // Validate the input
    // if (!validator.validateLLI(options)) {
    //   alert("Invalid input");
    //   return Promise.reject(new Error("Invalid input"));
    // }
    let createLLIUrl = `${lliWebServiceUrl}/postLLI`;

    let request = ajaxClient.post(createLLIUrl, options, jwtToken)

        return new Promise(function (resolve, reject) {
            request.then(function (response) {
                if (response.status != 200) {
                    throw new Error(response.statusText)
                }

                return response.json()
            }).then(function (response) {
                alert('The LLI is successfully created.')
                // location.reload()
                resolve(response)
            }).catch(function (error) {
                alert(error)
                reject(error)
            })
        })
  }

  function setupPostLLI(idPrefix) {
    console.log(`${idPrefix}-create-recommendation-button`)
      let createRecommendationButton = document.getElementById(
        `${idPrefix}-create-recommendation-button`
      );

      createRecommendationButton.addEventListener("click", function () {
        // pass in the user hash
        let title = document.getElementById(`${idPrefix}-title`).textContent;
        let deadline = document.getElementById(`${idPrefix}-date-input`).value;
        // TODO categories
        let selectedCategories = document.getElementById(`${idPrefix}-categories`).textContent.split(", ");
        console.log(selectedCategories);
        let description = document.getElementById(
          `${idPrefix}-description-input`
        ).textContent;
        let status = document.getElementById(`${idPrefix}-status-input`).value;
        let visibility = document.getElementById(
          `${idPrefix}-visibility-input`
        ).value;
        let cost = document.getElementById(`${idPrefix}-cost-input`).textContent;
        
        let recurrence = document.getElementById(
          `${idPrefix}-recurrence-input`
        ).value;

        let recurrenceStatus;
        let recurrenceFrequency;

        if (recurrence === "Off") {
          recurrenceStatus = "Off"
          recurrenceFrequency = "None"
        }
        else {
            recurrenceStatus = "On"
            const match = recurrence.match(/\(([^)]+)\)/);
            
            if (match) {
              console.log(match[1]); 
            } else {
              console.log("No match found");
            }

            recurrenceFrequency = match[1];
        }

        let options = {
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

      console.log(options)
      let response = postLLI(options, idPrefix);

        response
          .then(() => {
            console.log("Recommendation has been created");
            alert("Recommendation has been created");
          })
          .catch(function (error) {
            console.log(error);
          });
      });
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
    let container = document.getElementsByClassName("admin-seeding-container")[0];
    container.classList.remove("hidden");
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
    return [recommendationDiv, idPrefix];
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

    // Create visibility container
    const visibilityContainer = createVisibilityContainer(idPrefix);

    // Create deadline container
    const deadlineContainer = createDeadlineContainer(idPrefix);

    // Create recurrence container
    const recurrenceContainer = createRecurrenceContainer(idPrefix);


    // Create cost container
    const costContainer = createCostContainer(idPrefix, recommendation.Cost);

    // Create recommendation button container
    const recommendationContainer = createRecommendationButtonContainer(idPrefix);


    // Append all containers to formContainer
    formContainer.appendChild(inputArea);
    formContainer.appendChild(statusContainer);
    formContainer.appendChild(visibilityContainer);
    formContainer.appendChild(deadlineContainer);
    formContainer.appendChild(recurrenceContainer);
    formContainer.appendChild(costContainer);
    formContainer.appendChild(recommendationContainer);


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

    const costValueSpan = document.createElement("span");
    costValueSpan.id = `${idPrefix}-cost-input`;
    costValueSpan.contentEditable = "true";
    costValueSpan.innerText = "100";
    costHeading.appendChild(document.createTextNode(" $"));
    costHeading.appendChild(costValueSpan);
    // const costHeading = document.createElement("h2");
    // const costHeadingSpan = document.createElement("span");
    // costHeadingSpan.style.fontWeight = "600";
    // costHeadingSpan.innerText = "Cost:";
    // costHeading.appendChild(costHeadingSpan);

    // // Create the input element for cost
    // const costInput = document.createElement("input");
    // costInput.type = "number";
    // costInput.id = `${idPrefix}-cost-input`; // Use unique ID for the input
    // costInput.className = "cost-input";
    // costInput.placeholder = cost;
    // costInput.setAttribute("autocomplete", "off");

    // Append the heading and input to the cost container
    costContainer.appendChild(costHeading);
    costContainer.appendChild(costValueSpan);

    return costContainer;
  }

  function createRecommendationButtonContainer(idPrefix) {
    const createRecommendationContainer = document.createElement("div");
    createRecommendationContainer.className = "create-recommendation-container";
    createRecommendationContainer.innerHTML = `
          <button id="${idPrefix}-create-recommendation-button">Create Item</button>
        `;
    return createRecommendationContainer;
  }
  


  // Initialize the current view by setting up data and attaching event handlers
  function init() {
    jwtToken = localStorage["token-local"];

    if (jwtToken == null) {
      routeManager.loadPage(routeManager.PAGES.homePage)
    } else {
      let jwtTokenObject = JSON.parse(jwtToken); 
      console.log(jwtTokenObject);
      principal = {
        userId: jwtTokenObject.Payload.UserHash,
        claims: jwtTokenObject.Payload.Claims,
      };
      console.log(principal);
      window.name = routeManager.PAGES.recEnginePage;
      setupGetNumRecomendations();
      setupRepopulateUserDatamart();
      console.log(principal.claims.Role);
      if (principal.claims.Role ==  "Admin") {
        console.log("Admin")
        setupRepopulateAllUserSummary();
      }
      routeManager.setupHeaderLinks();
    } 
    
  }

  init();
}
