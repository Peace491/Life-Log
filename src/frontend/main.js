'use strict';

import * as routeManager from './routeManager.js';

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root) {
    // Dependency check
    const isValid = root;

    let jwtToken;

    if (!isValid) {
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    root.myApp = root.myApp || {};


    // Initialize the current view by setting up data and attaching event handlers 
    async function init() {
        if (localStorage.length != 0) {
            jwtToken = localStorage["token-local"]
        }

        if (!jwtToken) {
            routeManager.loadPage(routeManager.PAGES.homePage)
        } else {
            routeManager.loadPage(routeManager.PAGES.lliManagementPage)
        }
    }

    init();

})(window, window.ajaxClient);




