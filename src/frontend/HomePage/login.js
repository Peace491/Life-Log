'use strict';

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    const webServiceUrl = 'http://localhost:8082/authentication';

    // NOT exposed to the global object ("Private" functions)
    function getOTPEmail(email) {
        const getUrl = webServiceUrl + `/getOTPEmail?UserId=${email}`

        let request = ajaxClient.get(getUrl)

        return new Promise((resolve, reject) => {
            request.then(function (response) {
                console.log(response)
                return response.json();
            }).then(function (data) {
                console.log(data)
                resolve(data);
            }).catch(function (error) {
                reject(error);
            });
        });


    }

    let otpStatus = false;

    function onSubmitRegistrationCredentials() {
        // Get html elements
        let loginContainer = document.getElementById('login-container')

        let loginPrompt = document.getElementById('login-prompt')
        let usernameInput = document.getElementById('username-input')
        let submitCredentialButton = document.getElementById('submit-credential-button')

        //Change form format 
        loginContainer.style = 'grid-template-rows: 1fr 1fr 2fr 2fr;'

        // // Increase form height without animation
        // loginContainer.style = 'height: 85%'

        // Increase form height with animation:
        if (otpStatus == false) {
            loginContainer.style = 'animation: height-change 0.5s forwards;'
            otpStatus = true
        }

        // Change registration prompt
        loginPrompt.innerText = 'OTP has been sent to your email for account verification!'

        // Add new input
        usernameInput.insertAdjacentHTML('afterend', '<input class="otp-input" id="otp-input" placeholder="Enter OTP">')
        let otpInput = document.getElementById('otp-input')

        // // Make input appear without animation
        // otpInput.style = 'opacity: 100;'

        // Make input appear with animation
        otpInput.style = 'animation: otp-input-appear 0.5s forwards'

        // // Set background color to muted without animation
        // const mutedBackgroundColor = 'background-color: #F5C992;'
        // usernameInput.style = mutedBackgroundColor

        // Set background color to muted with animation
        const mutedBackgroundColorAnimation = 'animation: input-color-change 0.7s forwards'
        usernameInput.style = mutedBackgroundColorAnimation

        // Make credentials fields uneditable
        usernameInput.setAttribute("readonly", "")

        submitCredentialButton.innerText = "Log In!"
        submitCredentialButton.disabled = true;

        // Make API queries
        var email = usernameInput.value
        getOTPEmail(email)
    }

    root.myApp = root.myApp || {};

    // Initialize the current view by setting up data and attaching event handlers 
    function init() {
        const submitButton = document.getElementById('submit-credential-button')
        submitButton.addEventListener('click', onSubmitRegistrationCredentials)
    }

    init();

})(window, window.ajaxClient);





