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

    const webServiceUrl = 'http://localhost:8081';

  
    

    // NOT exposed to the global object ("Private" functions)
    function registerUser(userId, dob, zipCode) {
        let postDataUrl = webServiceUrl + "/registration/registerNormalUser"

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
                resolve(data);
            }).catch(function (error) {
                reject(error)
            })
        })
        
    }

    function onSubmitRegistrationCredentials(){
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

        registerUser(userId, dob, zipCode)

        submitCredentialButton.removeEventListener('click', onSubmitRegistrationCredentials)


    }

    root.myApp = root.myApp || {};

    // Show or Hide private functions
    //root.myApp.getData = getDataHandler;
    //root.myApp.sendData = sendDataHandler;

    // Initialize the current view by attaching event handlers 
    function init() {
        let submitButton = document.getElementById("submit-credential-button")
        submitButton.addEventListener("click", onSubmitRegistrationCredentials)
        
    }

    init();

})(window, window.ajaxClient);


