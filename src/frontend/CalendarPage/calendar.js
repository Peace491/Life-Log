'use strict';

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if(!isValid){
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    let jwtToken

    const calendarServiceUrl = 'http://localhost:8087/calendarService';
    

    // NOT exposed to the global object ("Private" functions)
    function getNextMonthData() {
        let getDataUrl = calendarServiceUrl + "/postNextMonth"
        let data = {}
        let request = ajaxClient.post(getDataUrl, data, jwtToken)

        return new Promise((resolve, reject) => {
            request.then(function (response) {
                return response.json();
            }).then(function (data) {
                resolve(data);
            }).catch(function (error) {
                reject(error)
            })
        })
        
    }

    
    function onClickGetNextMonth(){
        
        getNextMonthData().then(function(data) {
            let MonthData = data.output[0]

            console.log(MonthData.month)
            console.log(MonthData.year)
            console.log(MonthData.currDay)
            console.log(MonthData.numOfDayInMonth)

        })
    }

    root.myApp = root.myApp || {};

    // Show or Hide private functions
    //root.myApp.getData = getDataHandler;
    //root.myApp.sendData = sendDataHandler;

    // Initialize the current view by attaching event handlers 
    function init() {
        if (localStorage.length != 0) {
            jwtToken = localStorage["token-local"]
        }

        if (jwtToken) {
            window.location = "../CalendarPage/index.html"
        } else {
            let nextMonth = document.getElementById("get-next-month")
            nextMonth.addEventListener("click", onClickGetNextMonth)
        }
    }

    init();

})(window, window.ajaxClient);



