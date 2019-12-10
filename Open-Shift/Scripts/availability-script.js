import { AvailabilityCalendar } from "./calendar.js";

const data = {
    associate: JSON.parse(document.getElementById("availability-data").dataset.associate),
    availabilities: JSON.parse(document.getElementById("availability-data").dataset.availabilities),
};

let workingHoursStart = "17:00";
let workingHoursEnd = "24:00";
let closedWeekdays = ["Saturday", "Sunday"];

let container = document.getElementById("availability-calendar");
let calendar = new AvailabilityCalendar(data.availabilities, data.associate, closedWeekdays, workingHoursStart, workingHoursEnd, 15);
calendar.appendTo(container);