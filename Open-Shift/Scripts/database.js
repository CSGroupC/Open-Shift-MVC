
export function createAvailability(associate, timePeriod, monthDay, calendar) {

    let dayNumberElement = monthDay.getElementsByClassName("day-number")[0];

    let startTime = timePeriod.getElementsByClassName("time-start")[0].innerHTML + ":00";
    startTime = `${calendar.date.getFullYear()}-${calendar.date.getMonth() + 1}-${dayNumberElement.innerHTML}T${startTime}Z`;
    let endTime = timePeriod.getElementsByClassName("time-end")[0].innerHTML + ":00";
    // NOTE: 24:00 is not a valid time
    if (endTime.split(":")[0] == 24) endTime = "23:59:59";
    endTime = `${calendar.date.getFullYear()}-${calendar.date.getMonth() + 1}-${dayNumberElement.innerHTML}T${endTime}Z`;

    return fetch("Create", {
        method: "POST",
        body: JSON.stringify({
            AssociateID: associate.AssociateID,
            AssociateName: associate.name,
            IsManager: associate.IsManager,
            StartTime: startTime,
            EndTime: endTime,
        }),
        headers: {
            'Accept': 'application/json',
            'Content-type': 'application/json'
        },
    })
        .then(function (response) {
            if (!response.ok) {
                throw new Error('Availability/Create responded with ' + response.status);
            }
            return response.json();
        })
        .then(function (response) {
            if (response.status == "AUTHENTICATION_FAILED") {
                location.href = "/Profile/SignIn";
            }
        });
}

export function deleteAvailability(availabilityId) {

    fetch("Delete", {
        method: "DELETE",
        body: JSON.stringify({
            ID: availabilityId,
            //AssociateID: associate.AssociateID,
            // AssociateName: associate.name,
            //IsManager: associate.IsManager,
            //StartTime: startTime,
            //EndTime: endTime,
        }),
        headers: {
            'Accept': 'application/json',
            'Content-type': 'application/json'
        },
    })
        .then(function (response) {
            if (!response.ok) {
                throw new Error('Availability/Delete responded with ' + response.status);
            }
            return response.json();
        })
        .then(function (response) {
            if (response.status == "AUTHENTICATION_FAILED") {
                location.href = "/Profile/SignIn";
            }
        });
}