'use strict';

import * as routeManager from '../routeManager.js';
import * as userFormService from '../UserFormPage/userFormServices.js'

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
export function loadHomePage(root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    let jwtToken;

    const webServiceUrl = 'http://localhost:8082/authentication';
    const motivationalQuoteServiceUrl = 'http://localhost:8084';
    let userFormCompletionStatusUrl = ""


    function getMotivationalQuote() {
        const quoteUrl = motivationalQuoteServiceUrl + '/quotes/getQuote';

        let request = ajaxClient.get(quoteUrl);

        return new Promise((resolve, reject) => {
            request.then(function (response) {
                return response.json();
            }).then(function (data) {
                // Check if data has the Output array and has at least two elements (quote and author)
                //if (data.Output && data.Output.length >= 2) {
                const quote = data.Output[0]; // Assuming the first element is the quote
                const author = data.Output[1]; // Assuming the second element is the author
                resolve({ quote, author });
                //} else {
                //    reject('Quote data is not in the expected format.');
                //}
            }).catch(function (error) {
                reject(error);
            });
        });
    }


    // NOT exposed to the global object ("Private" functions)
    function getOTPEmail(email) {
        const getUrl = webServiceUrl + `/getOTPEmail?UserId=${email}`

        let request = ajaxClient.get(getUrl)

        return new Promise((resolve, reject) => {
            request.then(function (response) {
                return response.json();
            }).then(function (data) {
                resolve(data);
            }).catch(function (error) {
                reject(error);
            });
        });
    }

    function authenticateOTP(userHash, otp) {
        const postUrl = webServiceUrl + `/authenticateOTP`

        let data = {
            userHash: userHash,
            otp: otp
        }

        let request = ajaxClient.post(postUrl, data)


        return new Promise((resolve, reject) => {
            request.then(function (response) {
                return response.json();
            }).then(function (jwtToken) {
                localStorage.setItem("token-local", JSON.stringify(jwtToken));
                resolve(JSON.stringify(jwtToken));
            }).catch(function (error) {
                reject(error);
            });
        });
    }


    let otpStatus = false;

    async function onSubmitRegistrationCredentials() {
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
        // submitCredentialButton.disabled = true;

        const submitButton = document.getElementById('submit-credential-button')

        submitButton.removeEventListener('click', onSubmitRegistrationCredentials)

        // Make API queries
        try {
            var email = usernameInput.value;
            var userHash = await getOTPEmail(email);
    
            // Change event listener of button
            submitButton.addEventListener('click', async () => {
                jwtToken = await authenticateOTP(userHash, otpInput.value);
                
                var userFormIsCompleted = await userFormService.getUserFormCompletionStatus(userFormCompletionStatusUrl, userHash, jwtToken);

                if (userFormIsCompleted == 'true') {
                    routeManager.loadPage(routeManager.PAGES.lliManagementPage)
                } else {
                    routeManager.loadPage(routeManager.PAGES.userFormPage)
                }
            });
        } catch (error) {
            // Handle any errors that might occur
            console.error(error);
        }
    }

    root.myApp = root.myApp || {};

    async function fetchConfig() {
        const response = await fetch('./lifelog-config.url.json');
        const data = await response.json();
        let webServiceUrl = data.LifelogUrlConfig.UserManagement.UserForm.UserFormWebService;
        userFormCompletionStatusUrl = webServiceUrl + data.LifelogUrlConfig.UserManagement.UserForm.UserFormCompletionStatus;
    }

    // Initialize the current view by setting up data and attaching event handlers 
    async function init() {
        if (localStorage.length != 0) {
            jwtToken = localStorage["token-local"]
        }

        await fetchConfig()

        if (jwtToken) {
            routeManager.loadPage(routeManager.PAGES.lliManagementPage)
        } else {
            const submitButton = document.getElementById('submit-credential-button');
            submitButton.addEventListener('click', onSubmitRegistrationCredentials);

            const registerUserButton = document.getElementById('sign-up-text');
            registerUserButton.addEventListener('click', function () {
                routeManager.loadPage(routeManager.PAGES.registrationPage)
            });

        }
        // Fetch and display the motivational quote
        getMotivationalQuote().then(function (quoteData) {
            const quoteElement = document.querySelector('.quote h2');
            //console.log("a");
            const authorElement = document.querySelector('.quote-author h3');

            if (quoteData.quote) {
                quoteElement.textContent = quoteData.quote[0];
                //console.log("b");
                authorElement.textContent = ` - ${quoteData.quote[1]}`;
            }
        }).catch(function (error) {
            console.error(error);
            // Handle any errors, e.g., display a default quote or show a message
        });
    }


    init();

}
