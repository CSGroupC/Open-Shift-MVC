import { SchedulingCalendar } from "../Scripts/calendar.js";

const data = {
    associate: JSON.parse(document.getElementById("availability-data").dataset.associate),
    availabilities: JSON.parse(document.getElementById("availability-data").dataset.availabilities),
    shifts: JSON.parse(document.getElementById("availability-data").dataset.shifts),
};

let workingHoursStart = "17:00";
let workingHoursEnd = "24:00";
let closedWeekdays = ["Saturday", "Sunday"];

let associateMinimum = 2;
let managerMinimum = 1;

let container = document.getElementById("scheduling-calendar");
let calendar = new SchedulingCalendar(associateMinimum, managerMinimum, shifts, availabilities, closedWeekdays, workingHoursStart, workingHoursEnd, 15);
calendar.appendTo(container);