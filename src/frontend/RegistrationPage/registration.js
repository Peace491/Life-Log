onSubmitRegistrationCredentials = () => {
    // Get html elements
    let registrationFormContainer = document.getElementById('registration-form-container')

    registrationFormContainer.style = 'grid-template-rows: 1fr 1fr 4fr 2fr;'

    let zipcodeInput = document.getElementById('zipcode-input')
    zipcodeInput.insertAdjacentHTML('afterend', '<input class="otp-input" id="otp-input" placeholder="Enter OTP">')

    let usernameInput = document.getElementById('username-input')
    let dobInput = document.getElementById('dob-input')

    // Set background color to muted
    const mutedBackgroundColor = 'background-color: #F5C992;'
    usernameInput.style = mutedBackgroundColor
    dobInput.style = mutedBackgroundColor
    zipcodeInput.style = mutedBackgroundColor

    // Make credentials fields uneditable
    usernameInput.setAttribute("readonly", "")
    dobInput.setAttribute("readonly", "")
    zipcodeInput.setAttribute("readonly", "")

    // TODO: Make API queries
}