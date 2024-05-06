export function initInactivityTracker() {
    let inactivityTimer;

    let inactivityTime = 0;
    const inactivityThreshold = 20 * 60 * 1000; // 20 minutes in milliseconds

    // Function to reset inactivity timer
    function resetInactivityTimer() {
        clearTimeout(inactivityTimer);
        inactivityTime = 0;
        startInactivityTimer();
    }

    // Function to start inactivity timer
    function startInactivityTimer() {
        // Event listener for user activity
        window.addEventListener('mousemove', resetInactivityTimer, true);
        window.addEventListener('mousedown', resetInactivityTimer, true);
        window.addEventListener('touchstart', resetInactivityTimer, true);
        window.addEventListener('touchmove', resetInactivityTimer, true);
        window.addEventListener('click', resetInactivityTimer, true);
        window.addEventListener('keydown', resetInactivityTimer, true);
        window.addEventListener('scroll', resetInactivityTimer, true);


        inactivityTimer = setTimeout(function () {
            // Perform logout action when inactive for 20 minutes
            window.localStorage.clear(); // Clearing localStorage
            alert("You have been logged out due to inactivity.");
            location.reload()

            // Redirect to login page or perform any other necessary action
        }, inactivityThreshold);
    }

    startInactivityTimer()
}


