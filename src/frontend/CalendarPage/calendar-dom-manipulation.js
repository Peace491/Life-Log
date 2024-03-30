const daysContainer = document.querySelector(".days"),
  nextBtn = document.querySelector(".next-btn"),
  prevBtn = document.querySelector(".prev-btn"),
  month = document.querySelector(".month"),
  todayBtn = document.querySelector(".today-btn")


const months = [
    "Janurary",
    "Feburary",
    "March",
    "April",
    "May", 
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December"
]

const days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"]

// get current date
const date = new Date()

// get current month
let currentMonth = date.getMonth()

// get current year
let currentYear = date.getFullYear()



function renderCalendar() {
    
    date.setDate(1);
    const firstDay = new Date(currentYear, currentMonth, 1)
    const lastDay = new Date(currentYear, currentMonth + 1, 0)
    const lastDayIndex = lastDay.getDay()
    const lastDayDate = lastDay.getDate()
    const prevLastDay = new Date(currentYear, currentMonth, 0)
    const prevLastDayDate = prevLastDay.getDate()
    const nextDays = 7 - lastDayIndex - 1

    month.innerHTML = `${months[currentMonth]} ${currentYear}`

    let days = ""

    //prev days 
    for (let x = firstDay.getDay(); x > 0; x--) {
        days += `<div class="day prev-month-days">${prevLastDayDate - x + 1}</div>`
    }

    // curr month
    for (let i = 1; i <= lastDayDate; i++) {
        // check if its today then add today class
        if (
          i === new Date().getDate() &&
          currentMonth === new Date().getMonth() &&
          currentYear === new Date().getFullYear()
        ) {
          // if date month year matches add today
          days += `<div class="day current-day">${i}</div>`
        } else {
          //else dont add today
          days += `<div class="day">${i} <div class="lli-events event"><button class="lli-btn event">LLI 1</button></div><button class="pn-btn event">PN</button></div>`
        }
    }
    
    //next month days 
    for (let j = 1; j <= nextDays; j++) {
        days += `<div class="day next-month-days">${j}</div>`
    }

    daysContainer.innerHTML = days;



}

renderCalendar()

// get the next month
nextBtn.addEventListener("click", () => {
    currentMonth++;

    if (currentMonth > 11) {
      currentMonth = 0;
      currentYear++;
    }
   
    renderCalendar();
});


// get the prev month 
prevBtn.addEventListener("click", () => {
    currentMonth--;
    
    if (currentMonth < 0) {
      currentMonth = 11;
      currentYear--;
    }

    renderCalendar();
});



