'use strict';

import * as homePage from './HomePage/login.js'
import * as lliPage from './LLIManagementPage/lli.js'
import * as registrationPage from './RegistrationPage/registration.js'
import * as calendarPage from './CalendarPage/calendar.js'
import * as userFormPage from './UserFormPage/userForm.js'
import * as recEnginePage from './RecEnginePage/recEngine.js'


export const PAGES = {
    homePage: 'HomePage',
    lliManagementPage: 'LLIManagementPage',
    registrationPage: 'RegistrationPage',
    calendarPage: 'CalendarPage',
    userFormPage: 'UserFormPage',
    recEnginePage: 'RecEnginePage'
}

const SCRIPTS = {
    'HomePage': 'login.js',
    'LLIManagementPage': 'lli.js',
    'RegistrationPage': 'registration.js',
    'CalendarPage': 'calendar.js',
    'UserFormPage': 'userForm.js',
    'RecEnginePage': 'recEngine.js'
}

const LOAD_FUNCTION = {
    'HomePage': homePage.loadHomePage,
    'LLIManagementPage': lliPage.loadLLIPage,
    'RegistrationPage': registrationPage.loadRegistrationPage,
    'CalendarPage': calendarPage.loadCalendarPage,
    'UserFormPage': userFormPage.loadUserFormPage,
    'RecEnginePage': recEnginePage.loadRecEnginePage
}

export async function loadPage(page) {
    let fetchHtmlResponse = await fetchHtml(window.location.origin + "/" + page + "/index.html")

    document.body.innerHTML = fetchHtmlResponse.innerHTML

    let scriptElement = document.createElement('script');
    scriptElement.src = './' + page + '/' + SCRIPTS[page];
    scriptElement.type = 'module'
    scriptElement.id = 'script'

    // Append the script element to the document body
    document.body.appendChild(scriptElement);

    // Add an onload event listener to ensure the script is loaded before calling init()
    scriptElement.onload = function() {
        // Call init() function after the script is loaded
        LOAD_FUNCTION[page](window, window.ajaxClient)
    };

}

async function fetchHtml(pageRoute) {
    try {
        const response = await fetch(pageRoute);
        const text = await response.text();
        const dom = new DOMParser().parseFromString(text, "text/html");
        return dom.documentElement;
    } catch (error) {
        console.error("Error fetching index.html:", error);
        return null;
    }
}









