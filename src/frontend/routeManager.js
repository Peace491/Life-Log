'use strict';

import * as homePage from './HomePage/login.js'
import * as lliPage from './LLIManagementPage/lli.js'
import * as registrationPage from './RegistrationPage/registration.js'
import * as calendarPage from './CalendarPage/calendar.js'
import * as userFormPage from './UserFormPage/userForm.js'
import * as recEnginePage from './RecEnginePage/recEngine.js'
import * as personalNotePage from './PersonalNotePage/personalnote.js'
import * as mapPage from './MapPage/map.js'
import * as uadPage from './UsageAnalysisDashboardPage/uad.js'

import * as log from './Log/log.js'

export const PAGES = {
    homePage: 'HomePage',
    lliManagementPage: 'LLIManagementPage',
    registrationPage: 'RegistrationPage',
    calendarPage: 'CalendarPage',
    userFormPage: 'UserFormPage',
    recEnginePage: 'RecEnginePage',
    personalNotePage: 'PersonalNotePage',
    mapPage: 'MapPage',
    uadPage: 'UsageAnalysisDashboardPage'
}

const SCRIPTS = {
    'HomePage': 'login.js',
    'LLIManagementPage': 'lli.js',
    'RegistrationPage': 'registration.js',
    'CalendarPage': 'calendar.js',
    'UserFormPage': 'userForm.js',
    'RecEnginePage': 'recEngine.js',
    'PersonalNotePage': 'personalnote.js',
    'MapPage': 'map.js',
    'UsageAnalysisDashboardPage': 'uad.js'
}

const LOAD_FUNCTION = {
    'HomePage': homePage.loadHomePage,
    'LLIManagementPage': lliPage.loadLLIPage,
    'RegistrationPage': registrationPage.loadRegistrationPage,
    'CalendarPage': calendarPage.loadCalendarPage,
    'UserFormPage': userFormPage.loadUserFormPage,
    'RecEnginePage': recEnginePage.loadRecEnginePage,
    'PersonalNotePage': personalNotePage.LoadPersonalNotePage,
    'MapPage': mapPage.LoadMapPage,
    'UsageAnalysisDashboardPage': uadPage.loadUADPage
}

export async function loadPage(page, state = null, currPage = null, timeVisited = null, jwtToken = null) {
    if (currPage != null && timeVisited != null && jwtToken != null) {
        let userHash = JSON.parse(jwtToken).Payload.UserHash;
        logPageVisitTime(currPage, timeVisited, userHash, jwtToken)
    }

    window.name = ''
    let fetchHtmlResponse = await fetchHtml(window.location.origin + "/" + page + "/index.html")

    document.body.innerHTML = fetchHtmlResponse.innerHTML

    let scriptElement = document.createElement('script');
    scriptElement.src = './' + page + '/' + SCRIPTS[page];
    scriptElement.type = 'module'
    scriptElement.id = 'script'

    // Append the script element to the document body
    document.body.appendChild(scriptElement);

    // Add an onload event listener to ensure the script is loaded before calling init()
    scriptElement.onload = function () {
        // Call init() function after the script is loaded
        if (state == null) {
            LOAD_FUNCTION[page](window, window.ajaxClient)
        } else {
            LOAD_FUNCTION[page](window, window.ajaxClient, state)
        }

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

function logPageVisitTime(page, timeVisited, userHash, jwtToken) {
    timeVisited = Math.round((performance.now() - timeVisited) / 1000) // Round from miliseconds to seconds

    log.log(userHash, "Info", "Business", `${userHash} was on ${page} for ${timeVisited} seconds`, jwtToken)
}


export function setupHeaderLinks(currPage = null, timeVisited = null, jwtToken = null) {
    let calendarLink = document.getElementById("calendar-link")
    let personalNotesLink = document.getElementById('note-link')
    let lliLink = document.getElementById('lli-view')
    let recEngineLink = document.getElementById('rec-engine-link')
    let userFormLink = document.getElementById('user-form-link')
    let logoutInput = document.getElementById('logout')
    let mapLink = document.getElementById('map-link')

    calendarLink.addEventListener('click', function () {
        loadPage(PAGES.calendarPage, null, currPage, timeVisited, jwtToken)
    })

    personalNotesLink.addEventListener('click', function () {
        loadPage(PAGES.personalNotePage, null, currPage, timeVisited, jwtToken)
    })

    mapLink.addEventListener('click', function () {
        loadPage(PAGES.mapPage, null, currPage, timeVisited, jwtToken)
    })

    lliLink.addEventListener('click', function () {
        loadPage(PAGES.lliManagementPage, null, currPage, timeVisited, jwtToken)
    })

    recEngineLink.addEventListener('click', function () {
        loadPage(PAGES.recEnginePage, null, currPage, timeVisited, jwtToken)
    })

    userFormLink.addEventListener('click', function () {
        loadPage(PAGES.userFormPage, "Update", currPage, timeVisited, jwtToken)
    })

    logoutInput.addEventListener('click', function () {
        window.localStorage.clear()
        loadPage(PAGES.homePage, null, currPage, timeVisited, jwtToken)
    })

    if (jwtToken != null) {
        let jwtTokenJson = JSON.parse(jwtToken)

        let role = jwtTokenJson.Payload.Claims.Role
        if (role == "Admin" || role == "Root") {
            var divContainer = document.createElement("div");
            divContainer.className = "uad-link-container";
            divContainer.id = "uad-link";

            // Create an h2 element
            var h2Element = document.createElement("h2");
            h2Element.textContent = "Usage Analysis Dashboard";

            // Append the h2 element to the div container
            divContainer.appendChild(h2Element);

            let pageLinkContainer = document.getElementsByClassName("page-link-container")[0]
            pageLinkContainer.appendChild(divContainer)

            let uadLink = document.getElementById('uad-link')

            uadLink.addEventListener('click', function () {
                loadPage(PAGES.uadPage, null, currPage, timeVisited, jwtToken)
            })
        }

    }

}










