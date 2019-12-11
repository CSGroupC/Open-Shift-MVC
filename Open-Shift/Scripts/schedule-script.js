import { SchedulingCalendar } from "../Scripts/calendar.js";

const dataElement = document.getElementById("schedule-data");
const data = {
    associate: JSON.parse(dataElement.dataset.associate),
    availabilities: JSON.parse(dataElement.dataset.availabilities),
    shifts: JSON.parse(dataElement.dataset.shifts),
    storeId: dataElement.dataset.storeId,
};

let workingHoursStart = "17:00";
let workingHoursEnd = "24:00";
let closedWeekdays = ["Saturday", "Sunday"];

let associateMinimum = 2;
let managerMinimum = 1;

let container = document.getElementById("schedule-calendar");
let calendar = new SchedulingCalendar(data.associate, data.storeId, associateMinimum, managerMinimum, data.shifts, data.availabilities, closedWeekdays, workingHoursStart, workingHoursEnd, 15);
calendar.appendTo(container);