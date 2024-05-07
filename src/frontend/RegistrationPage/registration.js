'use strict';

import * as routeManager from '../routeManager.js';
import * as log from '../Log/log.js'

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
export function loadRegistrationPage(root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if(!isValid){
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    let jwtToken

    const registrationServiceUrl = 'http://localhost:8081';
    const authenticationServiceUrl = 'http://localhost:8082/authentication';

    // NOT exposed to the global object ("Private" functions)
    function registerUser(userId, dob, zipCode) {
        let postDataUrl = registrationServiceUrl + "/registration/registerNormalUser"

        let data = {
            userId: userId,
            dob: dob,
            zipCode: zipCode
        }

        let request = ajaxClient.post(postDataUrl, data)

        return new Promise((resolve, reject) => {
            request.then(function (response) {
                return response.json();
            }).then(function (data) {
                if (data.HasError == true) throw new Error("User Registration Failed")
                resolve(data);
            }).catch(function (error) {
                reject(error)
            })
        })
        
    }

    function authenticateOTP(userHash, otp) {
        const postUrl = authenticationServiceUrl + `/authenticateOTP`

        let data = {
            userHash: userHash,
            otp: otp
        }

        let request = ajaxClient.post(postUrl, data)

        return new Promise((resolve, reject) => {
            request.then(function (response) {
                return response.json();
            }).then(function (response) {
                if (response.status == 500) throw new Error("Failed to authenticate OTP")
                let jwtToken = response
                log.logLogin(userHash, "Success")
                localStorage.setItem("token-local", JSON.stringify(jwtToken));
                routeManager.loadPage(routeManager.PAGES.userFormPage, "Create")
                resolve(JSON.stringify(response));
            }).catch(function (error) {
                log.logLogin(userHash, "Failure")
                reject(error);
            });
        });
    }

    function onSubmitRegistrationCredentials(){
        console.log("in reg credentials")
        // Get html elements
        let registrationFormContainer = document.getElementById('registration-form-container')
    
        let registrationPrompt = document.getElementById('registration-prompt')
        let zipcodeInput = document.getElementById('zipcode-input')
        let usernameInput = document.getElementById('username-input')
        let dobInput = document.getElementById('dob-input')
        let submitCredentialButton = document.getElementById('submit-credential-button')
    
        // Change form format 
        registrationFormContainer.style = 'grid-template-rows: 1fr 1fr 4fr 2fr;'
    
        // // Increase form height without animation
        // registrationFormContainer.style = 'height: 85%'
    
        // Increase form height with animation:
        registrationFormContainer.style = 'animation: height-change 0.5s forwards;'
    
        // Change registration prompt
        registrationPrompt.innerText = 'OTP has been sent to your email for account verification!'
    
        // Add new input
        zipcodeInput.insertAdjacentHTML('afterend', '<input class="otp-input" id="otp-input" placeholder="Enter OTP">')
        let otpInput = document.getElementById('otp-input')
    
        // // Make input appear without animation
        // otpInput.style = 'opacity: 100;'
    
        // Make input appear with animation
        otpInput.style = 'animation: otp-input-appear 0.5s forwards'
    
        // // Set background color to muted without animation
        // const mutedBackgroundColor = 'background-color: #F5C992;'
        // usernameInput.style = mutedBackgroundColor
        // dobInput.style = mutedBackgroundColor
        // zipcodeInput.style = mutedBackgroundColor
    
        // Set background color to muted with animation
        const mutedBackgroundColorAnimation = 'animation: input-color-change 0.5s forwards'
        usernameInput.style = mutedBackgroundColorAnimation
        dobInput.style = mutedBackgroundColorAnimation
        zipcodeInput.style = mutedBackgroundColorAnimation
    
        // Make credentials fields uneditable
        usernameInput.setAttribute("readonly", "")
        dobInput.setAttribute("readonly", "")
        zipcodeInput.setAttribute("readonly", "")
    
        submitCredentialButton.innerText = "Sign Up!"
    
        let userId = usernameInput.value
        let dob = dobInput.value
        let zipCode = zipcodeInput.value

        submitCredentialButton.removeEventListener('click', onSubmitRegistrationCredentials)
        

        try {
            registerUser(userId, dob, zipCode).then(function(data) {
                let userHash = data.Output[0]
    
                submitCredentialButton.removeEventListener('click', onSubmitRegistrationCredentials)
    
                submitCredentialButton.addEventListener('click', function() {
                    authenticateOTP(userHash, otpInput.value)
                });
            })
        } catch (error) {
            console.error(error)
            alert(error)
        }
        
    }

    // Fetch URL config
    async function fetchConfig() {
        // fetch all Url's
        const response = await fetch('../lifelog-config.url.json');
        const data = await response.json();
        webServiceUrl = data.LifelogUrlConfig.Map.MapWebService;
        webServiceUrlLLI = data.LifelogUrlConfig.LLI.LLIWebService;
        createUrl = data.LifelogUrlConfig.Map.MapPinCreate;
        getAllPinUrl = data.LifelogUrlConfig.Map.MapGetAllPin;
        getAllLLIUrl = data.LifelogUrlConfig.LLI.GetAllLLI;
        updateUrl = data.LifelogUrlConfig.Map.MapPinUpdate;
        deleteUrl = data.LifelogUrlConfig.Map.MapPinDelete;
        viewUrl = data.LifelogUrlConfig.Map.MapPinView;
        getPinStatusUrl = data.LifelogUrlConfig.Map.MapPinStatusGet;
        editPinLLIUrl = data.LifelogUrlConfig.Map.MapPinLLIEdit;
        viewChangeUrl = data.LifelogUrlConfig.Map.MapViewUpdate;

        webServiceUrlLocRec = data.LifelogUrlConfig.LocationRecommendation.LocRecWebService;
        getRecommendationUrl = data.LifelogUrlConfig.LocationRecommendation.GetLocationRecommendation;
        viewRecommendationUrl = data.LifelogUrlConfig.LocationRecommendation.ViewLocationRecommendation;
        updateLogUrl = data.LifelogUrlConfig.LocationRecommendation.UpdateLog;
    }

    root.myApp = root.myApp || {};

    // Show or Hide private functions
    //root.myApp.getData = getDataHandler;
    //root.myApp.sendData = sendDataHandler;

    // Initialize the current view by attaching event handlers 
    function init() {
        if (localStorage.length != 0) {
            jwtToken = localStorage["token-local"]
        }

        if (jwtToken) {
            routeManager.loadPage(routeManager.PAGES.lliManagementPage)
        } else {
            let submitButton = document.getElementById("submit-credential-button")
            submitButton.addEventListener("click", onSubmitRegistrationCredentials)
        }
    }

    init();

}



