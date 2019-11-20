import { CustomElement, TimePeriodWrapper } from "./dom-elements.js";
import { MONTH_NAMES, formatTime, getDateFromQueryString, preventDefault } from "./utilities.js";

export class Calendar {

    // Strings in format "HH:MM"
    constructor(dayStartTime, dayEndTime, minutesPerColumn) {

        this.twentyFourHourMode = true;
        this.minutesPerColumn = minutesPerColumn;

        this.dayStartTime = new Date();
        this.dayEndTime = new Date();
        this.setDayStartTime(dayStartTime);
        this.setDayEndTime(dayEndTime);

        // See getters below
        // this.columnsPerDay
        // this.dayStartColumn
        // this.dayEndColumn

        this.element = document.createElement("div");
        this.date = getDateFromQueryString();
        this.dayElements = null;
        this.dayList = null;

        // NOTE: This is required to allow making new time period templates
        this.timePeriodTemplate = null;

        const currentDate = new Date();

        // Objects that represent an instance of resizing or movement
        this.timePeriodResizal = null;
        this.timePeriodMovement = null;

        this.element.classList.add("calendar");
        this.element.innerHTML += `
        <div class="calendar-header">
            <a href="?m=${this.date.getMonth()}&d=${this.date.getDate()}&y=${this.date.getFullYear()}" class="calendar-month-previous"><i class="fas fa-chevron-left"></i></a>
            <a href="?m=${this.date.getMonth() + 2}&d=${this.date.getDate()}&y=${this.date.getFullYear()}" class="calendar-month-next"><i class="fas fa-chevron-right"></i></a>
            <span class="month-title h2">${MONTH_NAMES[this.date.getMonth()]} ${this.date.getFullYear()}</span>
            <a href="?m=${currentDate.getMonth() + 1}&d=${currentDate.getDate()}&y=${currentDate.getFullYear()}" class="btn btn-primary">Today</a>
        </div>
        `;

        this.element.innerHTML += `
        <div class="day-list">
            <div class="weekday-headers">
                <div>S</div>
                <div>M</div>
                <div>T</div>
                <div>W</div>
                <div>T</div>
                <div>F</div>
                <div>S</div>
            </div>
            <div class="month-day-list">
            </div>
        </div>
        `;

        this.dayListElement = this.element.getElementsByClassName("month-day-list")[0];

        // Count weekdays from the last Sunday before this month, to the 1st of this month
        let dateBuffer = new Date(this.date);
        dateBuffer.setDate(1);
        let daysBeforeMonth = dateBuffer.getDay();

        // Count days of this month
        dateBuffer = new Date(this.date);
        dateBuffer.setMonth(dateBuffer.getMonth() + 1);
        dateBuffer.setDate(0);
        let daysInMonth = dateBuffer.getDate();

        // Count weekdays from the last day of this month, to the next Saturday
        dateBuffer = new Date(this.date);
        dateBuffer.setMonth(dateBuffer.getMonth() + 1);
        dateBuffer.setDate(0);
        let daysAfterMonth = 6 - dateBuffer.getDay();

        this.addMonthDays(daysBeforeMonth, "month-day-filler");
        this.addMonthDays(daysInMonth, "month-day");
        this.addMonthDays(daysAfterMonth, "month-day-filler");

        let today = daysBeforeMonth + currentDate.getDate();

        if (this.date.getMonth() == currentDate.getMonth() && this.date.getFullYear() == currentDate.getFullYear()) {
            let todayElement = this.dayListElement.querySelector(".month-day:nth-child(" + today + ") .day-number");
            todayElement.classList.add("bg-primary");
            todayElement.classList.add("text-light");
        }
    }

    get dayStartColumn() {
        return (this.dayStartTime.getHours() * 60 + this.dayStartTime.getMinutes()) / this.minutesPerColumn;
    }

    get dayEndColumn() {
        return (this.dayEndTime.getHours() * 60 + this.dayEndTime.getMinutes()) / this.minutesPerColumn;
    }

    get columnsPerDay() {
        let startMinutes = this.dayStartTime.getHours() * 60 + Math.round(this.dayStartTime.getMinutes() / 60) * 60;
        let endMinutes = this.dayEndTime.getHours() * 60 + Math.round(this.dayEndTime.getMinutes() / 60) * 60;
        return (endMinutes - startMinutes) / this.minutesPerColumn;
    }

    columnToTime(column) {
        let timeStart = new Date(this.dayStartTime.getTime());
        timeStart.setMinutes(timeStart.getMinutes() + (column * this.minutesPerColumn));
        let time = formatTime(timeStart);
        if (time == "0:00" && column > 1) {
            time = "24:00";
        }
        return time;
    }

    // Expected format: HH:MM
    setDayStartTime(time) {
        let hours = time.split(":")[0];
        let minutes = time.split(":")[1];
        this.dayStartTime.setHours(hours, minutes, 0, 0);
    }

    // Expected format: HH:MM
    setDayEndTime(time) {
        let hours = time.split(":")[0];
        let minutes = time.split(":")[1];

        if (hours == 24)
            this.dayEndTime.setHours(23, 59, 59, 999);
        else
            this.dayEndTime.setHours(hours, minutes, 0, 0);
    }

    addMonthDays(count, classes) {
        let html = "";

        for (let i = 1; i <= count; i++) {
            html += `
            <div class="${classes}">
                <div class="day-number-wrapper"><span class="day-number">${i}</span></div>
                <div class="time-period-section"></div>
            </div>
            `;
        }

        this.dayListElement.innerHTML += html;
    }

    appendTo(parent) {
        parent.appendChild(this.element);
    }
}

export class AvailabilityCalendar extends Calendar {
    constructor(dayStartTime = "9:00", dayEndTime = "17:00", minutesPerColumn = 15) {
        super(dayStartTime, dayEndTime, minutesPerColumn);


        let card = new CustomElement(`<div class="card"><div class="card-header h2">New Availability Period</div><div class="card-body"></div></div>`);
        let cardBody = card.getElementsByClassName("card-body")[0];

        this.timePeriodTemplate = new TimePeriodWrapper(this);
        this.timePeriodTemplate.classList.add("time-period-template");

        // Insert right after the calendar header
        cardBody.appendChild(this.timePeriodTemplate);
        this.element.prepend(card);
        //this.element.prepend(new CustomElement(`<div class="h1">Time Period</div>`));

        let monthDayElements = this.element.getElementsByClassName("month-day");
        for (let element of monthDayElements) {

            element.onclick = (event) => {

                if (this.timePeriodResizal == null && this.timePeriodMovement == null) {

                    let dayNumberElement = element.getElementsByClassName("day-number")[0];

                    let timePeriodWrapper = new TimePeriodWrapper(this);

                    element.querySelector(".time-period-section").prepend(timePeriodWrapper);
                }
            }
        }

        // Add this to the body in case the user's mouse leaves the calendar
        document.body.addEventListener("click", (event) => {

            if (this.timePeriodResizal != null) {
                this.timePeriodResizal.stop(event);
                this.timePeriodResizal = null;
            }
            if (this.timePeriodMovement != null) {
                this.timePeriodMovement.stop(event);
                this.timePeriodMovement = null;
            }

        });

        this.element.onmousemove = (event) => {

            if (this.timePeriodResizal != null) {
                this.timePeriodResizal.resize(event);
            }
            else if (this.timePeriodMovement != null) {
                this.timePeriodMovement.move(event);
            }
        };
    }
}

// ---------------------------------------------------------------------------------------------------------
// Static properties
// ---------------------------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------------------------
// Globals
// ---------------------------------------------------------------------------------------------------------
