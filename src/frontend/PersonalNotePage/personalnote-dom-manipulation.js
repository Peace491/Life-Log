// Note Manager
document.getElementById('note-manager-button').addEventListener('click', function() 
{
    const noteExists = true; // Replace with your function to check if a note exists

    if (noteExists) {
        const noteOptions = document.getElementById('note-options-dropdown');
        event.stopPropagation();
        noteOptions.style.display = 'block'; // Show the dropdown menu
    } else {
        alert('No note exists');
    }
    });

    document.getElementById('delete-note').addEventListener('click', function() {
    deleteNote(); // Replace with your function to delete a note
    });

    document.getElementById('edit-note').addEventListener('click', function() {
    editNote(); // Replace with your function to edit a note
});

//when clicked outside anywhere on the document the dropdown disappears

// Get the dropdown menu
var dropdown = document.getElementById('note-options-dropdown');

// Hide the dropdown menu when the document is clicked
document.addEventListener('click', function() {
    dropdown.style.display = 'none';
  });

// Prevent the document click event from firing when the dropdown menu is clicked
dropdown.addEventListener('click', function(event) {
    event.stopPropagation();
  });

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

// Initially, check the visibility of the "Done" button
toggleDoneButtonVisibility();
//#endregion