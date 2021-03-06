﻿import { CustomElement, TimePeriod } from "./dom-elements.js";
import { MONTH_NAMES, formatTime, stringToDate, getDateFromQueryString, nameToColor, Event } from "./utilities.js";
import { createAvailability, updateAvailability, deleteTimePeriod, createShift } from "./database.js";

let WEEKDAY_INDEXES = { Sunday: 0, Monday: 1, Tuesday: 2, Wednesday: 3, Thursday: 4, Friday: 5, Saturday: 6 };

export class Calendar {

    // Strings in format "HH:MM"
    constructor(associate, timePeriods, closedWeekdays, dayStartTime, dayEndTime, minutesPerColumn) {

        // Require mouseup and mousedown to target the same element in order for click events to fire
        Event.preventFalseClicks();
        //Event.registerTouchEvent();

        this.associate = associate;
        this.twentyFourHourMode = true;
        this.minutesPerColumn = minutesPerColumn;

        this.dayStartTime = new Date();
        this.dayEndTime = new Date();
        this.hoursPerDay = 24;
        this.setDayStartTime(dayStartTime);
        this.setDayEndTime(dayEndTime);

        // See getters/setters below
        // this.columnsPerDay
        // this.dayStartColumn
        // this.dayEndColumn

        this.element = document.createElement("div");
        this.date = getDateFromQueryString();
        this.dayElements = null;
        this.dayList = null;

        // Used for viewing time period details
        this.focusedTimePeriod = null;

        this.currentDate = new Date();

        this.element.dataset.isManager = this.associate.IsManager;

        // Objects that represent an instance of resizing or movement
        this.timePeriodResizal = null;
        this.timePeriodMovement = null;

        let previousMonth = this.date.getMonth();
        let previousYear = this.date.getFullYear();
        if (previousMonth < 1) {
            previousMonth = 12;
            previousYear -= 1;
        }

        let nextMonth = this.date.getMonth() + 2;
        let nextYear = this.date.getFullYear();
        if (nextMonth > 12) {
            nextMonth = 1;
            nextYear += 1;
        }

        this.element.classList.add("calendar");
        this.element.innerHTML += `
        <div class="calendar-mobile-overlay hidden"></div>
        <div class="calendar-header">
            <a href="?m=${previousMonth}&d=${this.date.getDate()}&y=${previousYear}" class="calendar-month-previous"><i class="fas fa-chevron-left"></i></a>
            <a href="?m=${nextMonth}&d=${this.date.getDate()}&y=${nextYear}" class="calendar-month-next"><i class="fas fa-chevron-right"></i></a>
            <span class="month-title h4">${MONTH_NAMES[this.date.getMonth()]} ${this.date.getFullYear()}</span>
            <a href="?m=${this.currentDate.getMonth() + 1}&d=${this.currentDate.getDate()}&y=${this.currentDate.getFullYear()}" class="btn btn-primary">Today</a>
            <i class="announce-button fas fa-bullhorn text-primary"></i>
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
        this.mobileOverlay = this.element.getElementsByClassName("calendar-mobile-overlay")[0];

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

        let today = daysBeforeMonth + this.currentDate.getDate();

        if (this.date.getMonth() == this.currentDate.getMonth() && this.date.getFullYear() == this.currentDate.getFullYear()) {
            let todayElement = this.dayListElement.querySelector(".month-day:nth-child(" + today + ") .day-number");
            todayElement.classList.add("bg-primary");
            todayElement.classList.add("text-light");
        }


        // Mark the closed days as closed
        for (let weekday of closedWeekdays) {
            let offset = 1 + WEEKDAY_INDEXES[weekday];
            let days = this.element.querySelectorAll(`.month-day-list > :nth-child(7n+${offset})`);

            for (let day of days) {
                day.classList.add("closed-day");
            }
        }


        // Mark past days as "closed"
        this.monthDays = this.dayListElement.getElementsByClassName("month-day");

        if (this.date.getFullYear() < this.currentDate.getFullYear() ||
            (this.date.getFullYear() == this.currentDate.getFullYear() && this.date.getMonth() < this.currentDate.getMonth())) {
            this.dayListElement.classList.add("closed-month");
        } else if (this.date.getMonth() == this.currentDate.getMonth() && this.date.getFullYear() == this.currentDate.getFullYear()) {

            for (let i = 0; i < this.currentDate.getDate() - 1; i++) {
                this.monthDays[i].classList.add("closed-day");
            }
        }


        this.associates = {};
        this.timePeriods = [];

        if (timePeriods != [] && timePeriods != null) {
            let associateIndex = 0;
            // Convert the array into an object with the IDs as keys
            this.timePeriods = timePeriods.reduce((object, timePeriod) => {

                if ("AssociateID" in timePeriod && !(timePeriod.AssociateID in this.associates)) {

                    this.associates[timePeriod.AssociateID] = {
                        id: timePeriod.AssociateID,
                        index: associateIndex,
                        name: timePeriod.AssociateName,
                        isManager: timePeriod.IsManager,
                        color: nameToColor(timePeriod.AssociateID, timePeriod.AssociateName)
                    };

                    associateIndex++;
                }

                object[timePeriod.ID] = timePeriod;
                return object;

                // Start as empty object
            }, {});
        }

        this.element.dataset.associateCount = Object.keys(this.associates).length;

        // Load existing time periods onto the calendar
        for (let id in this.timePeriods) {
            let timePeriod = this.timePeriods[id];

            let time = {
                start: stringToDate(timePeriod.StartTime),
                end: stringToDate(timePeriod.EndTime)
            };

            let associate = null;

            if ("AssociateID" in timePeriod) {
                associate = this.associates[timePeriod.AssociateID];
            }

            let timePeriodElement = new TimePeriod(this, time, associate);
            timePeriodElement.classList.add("associate-" + associate.index);

            timePeriodElement.dataset.availabilityId = id;
            let monthDay = this.element.querySelector(`[data-month-day='${time.start.getDate()}']`);
            monthDay.dataset.availableAssociateCount = parseInt(monthDay.dataset.availableAssociateCount) + 1;
            monthDay.querySelector(`.time-period-section`).prepend(timePeriodElement);
        }


        let handler = new Event.PointerHandler((event) => {
            this.mobileOverlay.classList.add("hidden");
            this.focusedTimePeriod.classList.remove("focused");
            delete this.focusedTimePeriod.dataset.temporaryTooltip;
            this.focusedTimePeriod = null;
        });

        this.mobileOverlay.onclick = handler;
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

    pixelsToColumns(pixels, parentWidth) {
        return parseInt((pixels / parentWidth) * this.columnsPerDay);
    }

    hoursToColumns(hours) {
        return parseInt((hours / this.hoursPerDay) * this.columnsPerDay);
    }

    timeToColumns(datetime) {
        let hour = datetime.getHours() + datetime.getMinutes() / 60 + datetime.getSeconds() / 60 / 60 + datetime.getMilliseconds() / 1000 / 60 / 60;
        let startHour = this.dayStartTime.getHours() + this.dayStartTime.getMinutes() / 60 + this.dayStartTime.getSeconds() / 60 / 60 + this.dayStartTime.getMilliseconds() / 1000 / 60 / 60;

        return Math.round(((hour - startHour) / this.hoursPerDay) * this.columnsPerDay) + 1;
    }

    // Expected format: "HH:MM"
    setDayStartTime(time) {
        let hours = time.split(":")[0];
        let minutes = time.split(":")[1];
        this.dayStartTime.setHours(hours, minutes, 0, 0);

        this.hoursPerDay = (this.dayEndTime.getTime() - this.dayStartTime.getTime()) / 1000 / 60 / 60;
    }

    // Expected format: "HH:MM"
    setDayEndTime(time) {
        let hours = time.split(":")[0];
        let minutes = time.split(":")[1];

        if (hours == 24)
            this.dayEndTime.setHours(23, 59, 59, 999);
        else
            this.dayEndTime.setHours(hours, minutes, 0, 0);

        this.hoursPerDay = (this.dayEndTime.getTime() - this.dayStartTime.getTime()) / 1000 / 60 / 60;
    }

    addMonthDays(count, classes) {
        let html = "";

        for (let i = 1; i <= count; i++) {
            html += `
            <div class="${classes}" data-month-day="${i}" data-available-associate-count="0" data-associate-count="0" data-manager-count="0">
                <div class="day-number-wrapper"><span class="day-number">${i}</span></div>
                <span class="error-icons">
                    <i class="fas fa-exclamation-triangle warning-icon" data-tooltip="Not enough managers."></i>
                    <i class="fas fa-exclamation-circle error-icon" data-tooltip="Not enough associates."></i>
                </span>
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
    constructor(availabilities = [], associate = { AssociateID: 0 }, closedWeekdays = [], dayStartTime = "9:00", dayEndTime = "17:00", minutesPerColumn = 15) {
        super(associate, availabilities, closedWeekdays, dayStartTime, dayEndTime, minutesPerColumn);

        this.element.classList.add("availability-calendar");
        // NOTE: This is required to allow making new time period templates
        this.timePeriodTemplate = null;

        if (associate.AssociateID > 0) {
            associate.name = associate.FirstName + " " + associate.LastName;
            associate.color = nameToColor(associate.AssociateID, associate.name);
        }

        let card = new CustomElement(`<div class="card time-period-template-card"><div class="card-body"></div></div>`);
        let cardBody = card.getElementsByClassName("card-body")[0];

        this.timePeriodTemplate = new TimePeriod(this, { start: null, end: null }, associate);
        this.timePeriodTemplate.classList.add("time-period-template");

        cardBody.appendChild(this.timePeriodTemplate);
        this.element.append(card);

        let monthDayElements = this.element.getElementsByClassName("month-day");
        for (let element of monthDayElements) {

            let handler = new Event.PointerHandler((event) => {

                // If the click originated directly on this element
                if (this.timePeriodResizal == null && this.timePeriodMovement == null) {

                    let timePeriod = new TimePeriod(this, { start: null, end: null }, associate);

                    createAvailability(associate, timePeriod, element, this).then(() => {
                        element.querySelector(".time-period-section").prepend(timePeriod);
                    });
                }
            });

            // NOTE: onclick will be simulated on mobile browsers
            element.onclick = handler;
        }

        let handler = new Event.PointerHandler((event) => {

            if (this.timePeriodResizal != null) {
                if (this.timePeriodResizal.timePeriod != this.timePeriodTemplate) {
                    updateAvailability(associate, this.timePeriodResizal.timePeriod, this);
                }
                this.timePeriodResizal.stop(event);
                this.timePeriodResizal = null;
            }
            if (this.timePeriodMovement != null) {
                if (this.timePeriodMovement.timePeriod != this.timePeriodTemplate) {
                    updateAvailability(associate, this.timePeriodMovement.timePeriod, this);
                }
                this.timePeriodMovement.stop(event);
                this.timePeriodMovement = null;
            }
        }, true); // Allow default to allow the click event to fire on mobile

        // Add this to the body in case the user's mouse leaves the calendar
        document.body.addEventListener("touchend", handler);
        document.body.addEventListener("mouseup", handler);

        handler = new Event.PointerHandler((event) => {
            if (this.timePeriodResizal != null) {
                this.timePeriodResizal.resize(event);
            }
            else if (this.timePeriodMovement != null) {
                this.timePeriodMovement.move(event);
            }
        });

        this.element.ontouchmove = handler;
        this.element.onmousemove = handler;
    }
}

export class SchedulingCalendar extends Calendar {
    constructor(associate = null, storeId = 0, associateMinimum = 1, managerMinimum = 1, shifts = [], availabilities = [], closedWeekdays = [], dayStartTime = "9:00", dayEndTime = "17:00", minutesPerColumn = 15) {
        super(associate, availabilities, closedWeekdays, dayStartTime, dayEndTime, minutesPerColumn);

        this.element.classList.add("schedule-calendar");

        this.storeId = storeId;

        this.associateMinimum = associateMinimum;
        this.managerMinimum = managerMinimum;


        // Load existing shifts on the calendar
        for (let shift of shifts) {
            let schedulingBar = this.dayListElement.querySelector(`.time-period[data-availability-id='${shift.AvailabilityID}'] .scheduling-bar`);
            let associate = this.associates[shift.AssociateID];

            this.toggleScheduled(schedulingBar, associate, shift);
        }

        // Show list of available associates
        let card = new CustomElement(`<div class="card associate-list-card"><div class="card-body"></div></div>`);
        let cardBody = card.getElementsByClassName("card-body")[0];

        for (let id in this.associates) {
            let associate = this.associates[id];

            let associateElement = new CustomElement(`
                <span class="associate-list-item"><i class="fas fa-circle" style="color: ${associate.color}"></i><span>${associate.name}</span></span>
            `);
            cardBody.appendChild(associateElement);
        }

        this.element.append(card);

        for (let monthDay of this.monthDays) {
            this.checkSchedulingErrors(monthDay);
        }
    }

    toggleScheduled(timePeriodBar, associate, shift = null) {

        let monthDay = timePeriodBar.closest(".month-day");
        let timePeriod = timePeriodBar.closest(".time-period");

        if (timePeriodBar.classList.contains("scheduled") == false) {

            if (shift == null) {
                if (this.associate.IsManager == 1) {
                    createShift(associate, timePeriod, monthDay, this).then(() => {
                        timePeriodBar.classList.add("scheduled");
                        this.addAssociateToDay(monthDay, associate);
                    });
                }
            } else {
                timePeriodBar.classList.add("scheduled");
                this.addAssociateToDay(monthDay, associate);
                timePeriod.dataset.shiftId = shift.ID;
            }

        } else {
            if (this.associate.IsManager == 1) {
                deleteTimePeriod(timePeriod.dataset.shiftId).then(() => {
                    timePeriodBar.classList.remove("scheduled");
                    this.removeAssociateFromDay(monthDay, associate);

                    delete timePeriod.dataset.shiftId;
                });
            }
        }
    }


    addAssociateToDay(monthDay, associate) {
        let associateCount = parseInt(monthDay.dataset.associateCount) + 1;
        monthDay.dataset.associateCount = associateCount;

        if (associate.isManager == true) {
            let managerCount = parseInt(monthDay.dataset.managerCount) + 1;
            monthDay.dataset.managerCount = managerCount;
        }
        this.checkSchedulingErrors(monthDay);
    }

    removeAssociateFromDay(monthDay, associate) {
        let associateCount = parseInt(monthDay.dataset.associateCount) - 1;
        monthDay.dataset.associateCount = associateCount;

        if (associate.isManager == true) {
            let managerCount = parseInt(monthDay.dataset.managerCount) - 1;
            monthDay.dataset.managerCount = managerCount;
        }
        this.checkSchedulingErrors(monthDay);
    }

    checkSchedulingErrors(monthDay) {
        if (monthDay.dataset.associateCount < this.associateMinimum) {
            monthDay.classList.add("associate-minimum-error");
        } else {
            monthDay.classList.remove("associate-minimum-error");
        }

        if (monthDay.dataset.managerCount < this.managerMinimum) {
            monthDay.classList.add("manager-minimum-error");
        } else {
            monthDay.classList.remove("manager-minimum-error");
        }
    }
}

// ---------------------------------------------------------------------------------------------------------
// Static properties
// ---------------------------------------------------------------------------------------------------------


// ---------------------------------------------------------------------------------------------------------
// Globals
// ---------------------------------------------------------------------------------------------------------
