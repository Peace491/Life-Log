
export function renderCalendar(showCalendarLLI, date, currentMonth, currentYear) {
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
  
  let days = ""

  const daysContainer = document.querySelector(".days"),
  nextBtn = document.querySelector(".next-btn"),
  prevBtn = document.querySelector(".prev-btn"),
  month = document.querySelector(".month")

  date.setDate(1);
  const firstDay = new Date(currentYear, currentMonth, 1)
  const lastDay = new Date(currentYear, currentMonth + 1, 0)
  const lastDayIndex = lastDay.getDay()
  const lastDayDate = lastDay.getDate()
  const prevLastDay = new Date(currentYear, currentMonth, 0)
  const prevLastDayDate = prevLastDay.getDate()
  const nextDays = 7 - lastDayIndex - 1

  month.innerHTML = `${months[currentMonth]} ${currentYear}`


  //prev days 
  for (let x = firstDay.getDay(); x > 0; x--) {
    days += `<div class="day prev-month-days">${prevLastDayDate - x + 1}</div>`
  }

  // get all lli and pn 
  

  // curr month
  for (let i = 1; i <= lastDayDate; i++) {
    // check if its today then add today class
    if (
      i === new Date().getDate() &&
      currentMonth === new Date().getMonth() &&
      currentYear === new Date().getFullYear()
    ) {
      // current day 
      if (i < 10) {
        i = `0${i}`
      }
      let monthid = '00'
      if (currentMonth + 1 < 10) {
        monthid = `0${currentMonth + 1}`
      }

      // div for inserting a single lli event : <button data-modal-target="#lli-modal-edit" class="lli-btn event">LLI 1</button>
      days += `<div class="day current-day">${i} <div id="insert-llievent-${i}" class="lli-events event"></div><button data-modal-target="#lli-modal-create" class="add-lli-btn add-lli-btn-${i}">+</button><button data-modal-target="#pn-modal" class="pn-btn event" id="pn-btn-${monthid}/${i}/${currentYear}">PN</button></div></div>`
    } else {
      // all other days 
      if (i < 10) {
        i = `0${i}`
      }
      let monthid = '00'
      if (currentMonth + 1 < 10) {
        monthid = `0${currentMonth + 1}`
      }
      days += `<div class="day">${i} <div id="insert-llievent-${i}" class="lli-events event"></div><button data-modal-target="#lli-modal-create" class="add-lli-btn add-lli-btn-${i}">+</button><button data-modal-target="#pn-modal" class="pn-btn event" id="pn-btn-${monthid}/${i}/${currentYear}">PN</button></div>`
    }
  }

  //next month days 
  for (let j = 1; j <= nextDays; j++) {
    days += `<div class="day next-month-days">${j}</div>`
  }

  daysContainer.innerHTML = days;

  nextBtn.addEventListener("click", () => {
    currentMonth++;
  
    if (currentMonth > 11) {
      currentMonth = 0;
      currentYear++;
    }
  
    renderCalendar(showCalendarLLI, date, currentMonth, currentYear);
    renderModals()
    window.PNRetrieval()
  
  });
  
  
  // get the prev month 
  prevBtn.addEventListener("click", () => {
    currentMonth--;
  
    if (currentMonth < 0) {
      currentMonth = 11;
      currentYear--;
    }
  
    renderCalendar(showCalendarLLI, date, currentMonth, currentYear);
    renderModals()
    window.PNRetrieval()
  
  });

  showCalendarLLI(currentMonth + 1, currentYear)
}

// function useOutput() {

//     window.showCalendarLLI(currentMonth + 1, currentYear)
//         .then(function(output) {
//             console.log(output); // Use the output array here

//         })

// }

// Call the function when needed




// renderCalendar()
// renderModals()
// window.PNRetrieval()

// get the next month


// ------------------Modal-------------------------------------

export function renderModals() {

  const openModalButtons = document.querySelectorAll('[data-modal-target]')
  const overlay = document.getElementById('overlay')

  openModalButtons.forEach(button => {

    button.addEventListener('click', () => {
      const modal = document.querySelector(button.dataset.modalTarget)
      openModal(modal)
    })
  })

  overlay.addEventListener('click', () => {
    const modals = document.querySelectorAll('.modal.active')
    modals.forEach(modal => {
      closeModal(modal)
    })
  })


  function openModal(modal) {
    if (modal == null) {
      return
    }
    modal.classList.add('active')
    overlay.classList.add('active')
  }

  function closeModal(modal) {
    if (modal == null) return
    modal.classList.remove('active')
    overlay.classList.remove('active')
  }
}






