import { AvailabilityCalendar } from "./calendar.js";

const data = {
    associate: JSON.parse(document.getElementById("availability-data").dataset.associate),
    availabilities: JSON.parse(document.getElementById("availability-data").dataset.availabilities),
};


const PLACEHOLDER = 0;
// DATA (BACK->FRONT):
// 1. The current user's ID
let userId = PLACEHOLDER;
// 2. The current user's access token
let accessToken = PLACEHOLDER;
// 3. An array/object/map of the current user's availabilities for this month (from the query string)
//    This should include each availability's start datetime, and end datetime
/*
let availabilities = [
    {
        id: 1,
        associateId: 1,
        associateName: "Rikako Kakinuma",
        isManager: true,
        startTime: "2019-11-26 17:00:00",
        endTime: "2019-11-26 23:59:59"
    },
    {
        id: 2,
        associateId: 1,
        associateName: "Rikako Kakinuma",
        isManager: true,
        startTime: "2019-11-27 19:00:00",
        endTime: "2019-11-27 23:59:59"
    },
    {
        id: 3,
        associateId: 1,
        associateName: "Rikako Kakinuma",
        isManager: true,
        startTime: "2019-11-25 17:00:00",
        endTime: "2019-11-25 23:59:59"
    }
];
*/


// 4. The start of the current user's business' working hours
let workingHoursStart = "17:00";
// 5. The end of the current user's business' working hours
let workingHoursEnd = "24:00";
// 6. The weekdays that the current user's store is CLOSED
let closedWeekdays = ["Saturday", "Sunday"];
/*
let associate = {
    id: @Model.user.AssociateID,
    name: "@{ Model.user.FirstName + " " + Model.user.LastName }"
};
*/

let container = document.getElementById("availability-calendar");
let calendar = new AvailabilityCalendar(data.availabilities, data.associate, closedWeekdays, workingHoursStart, workingHoursEnd, 15);
calendar.appendTo(container);