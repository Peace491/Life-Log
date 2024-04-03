export function setUp() {
    // Set today's date for note on bootup
    var date = new Date();
    date.setDate(date.getDate());
    document.getElementById('create-date-input').valueAsDate = date;

    //#region Done Button
    // Get references to the paragraph and the "Done" button
    const paragraphInput = document.getElementById('create-paragraph-input');
    const doneButton = document.getElementById('submit-note-button');

    // Function to toggle the visibility of the "Done" button based on the paragraph content
    function toggleDoneButtonVisibility() {
        if (paragraphInput.textContent.trim().length > 0) {
            doneButton.style.display = 'block'; // Show the button if there is text
        } else {
            doneButton.style.display = 'none'; // Hide the button if there is no text
        }
    }

    // Listen for input events on the paragraph
    paragraphInput.addEventListener('input', toggleDoneButtonVisibility);
    // Listen for when the user presses done 
    doneButton.addEventListener('click', function(){
        doneButton.style.display = 'none';
    })
    // Initially, check the visibility of the "Done" button
    toggleDoneButtonVisibility();
}



