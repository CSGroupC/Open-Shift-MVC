import { SchedulingCalendar } from "../Scripts/calendar.js";

const data = {
    associate: JSON.parse(document.getElementById("schedule-data").dataset.associate),
    availabilities: JSON.parse(document.getElementById("schedule-data").dataset.availabilities),
    shifts: JSON.parse(document.getElementById("schedule-data").dataset.shifts),
};

let workingHoursStart = "17:00";
let workingHoursEnd = "24:00";
let closedWeekdays = ["Saturday", "Sunday"];

let associateMinimum = 2;
let managerMinimum = 1;

let container = document.getElementById("schedule-calendar");
let calendar = new SchedulingCalendar(associateMinimum, managerMinimum, data.shifts, data.availabilities, closedWeekdays, workingHoursStart, workingHoursEnd, 15);
calendar.appendTo(container);