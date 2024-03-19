'use strict';

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

    function fromHTML(html, trim = true) {
        // Process the HTML string.
        html = trim ? html.trim() : html;
        if (!html) return null;
      
        // Then set up a new template element.
        const template = document.createElement('template');
        template.innerHTML = html;
        const result = template.content.children;
      
        // Then return either an HTMLElement or HTMLCollection,
        // based on whether the input HTML had one or more roots.
        if (result.length === 1) return result[0];
        return result;
      }

    root.myApp = root.myApp || {};

    // Initialize the current view by setting up data and attaching event handlers 
    function init() {
        if (localStorage.length != 0) {
            jwtToken = localStorage["token-local"]
        }

        let body = document.body
        let currentScript = document.createElement('script')

        if (!jwtToken) {
            currentScript.src = './HomePage/login.js'
            body.appendChild(currentScript)

        }
        else {
            let homePageDiv = document.getElementById('home-page')
            homePageDiv.classList.add('hidden')

            let lliDiv = document.getElementById('lli-management-page')
            lliDiv.classList.remove('hidden')

            let styling = fromHTML('<link rel="stylesheet" href="./LLIManagementPage/styles.css">');
            lliDiv.insertBefore(styling, lliDiv.firstChild)

            currentScript.src = './LLIManagementPage/lli.js'
            body.appendChild(currentScript)
        }
    }

    init();

})(window, window.ajaxClient);





