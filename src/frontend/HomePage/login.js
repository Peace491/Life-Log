let otpStatus = false;

onSubmitRegistrationCredentials = () => {
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

    // TODO: Make API queries
}