
export function createAvailability(associate, timePeriod, monthDay, calendar) {

    let dayNumberElement = monthDay.getElementsByClassName("day-number")[0];

    let startTime = timePeriod.getElementsByClassName("time-start")[0].innerHTML + ":00";
    startTime = `${calendar.date.getFullYear()}-${calendar.date.getMonth() + 1}-${dayNumberElement.innerHTML}T${startTime}Z`;
    let endTime = timePeriod.getElementsByClassName("time-end")[0].innerHTML + ":00";
    // NOTE: 24:00 is not a valid time
    if (endTime.split(":")[0] == 24) endTime = "23:59:59";
    endTime = `${calendar.date.getFullYear()}-${calendar.date.getMonth() + 1}-${dayNumberElement.innerHTML}T${endTime}Z`;

    let availability = {
        AssociateID: associate.AssociateID,
        AssociateName: associate.name,
        IsManager: associate.IsManager,
        StartTime: startTime,
        EndTime: endTime,
    };

    return fetch("Create", {
        method: "POST",
        body: JSON.stringify(availability),
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
            } else if (response.status == "INSERT_FAILED") {
                // TODO: Tell the user something went wrong
                console.error("/Profile/SignIn returned INSERT_FAILED");
            } else {
                availability.ID = response.id;
                timePeriod.dataset.id = response.id;
            }
        });
}

export function updateAvailability(associate, timePeriodBar, calendar) {

    let timePeriod = timePeriodBar.closest(".time-period");
    let monthDay = timePeriod.closest(".month-day");
    let dayNumberElement = monthDay.getElementsByClassName("day-number")[0];

    let startTime = timePeriod.getElementsByClassName("time-start")[0].innerHTML + ":00";
    startTime = `${calendar.date.getFullYear()}-${calendar.date.getMonth() + 1}-${dayNumberElement.innerHTML}T${startTime}Z`;
    let endTime = timePeriod.getElementsByClassName("time-end")[0].innerHTML + ":00";
    // NOTE: 24:00 is not a valid time
    if (endTime.split(":")[0] == 24) endTime = "23:59:59";
    endTime = `${calendar.date.getFullYear()}-${calendar.date.getMonth() + 1}-${dayNumberElement.innerHTML}T${endTime}Z`;

    return fetch("Update", {
        method: "PUT",
        body: JSON.stringify({
            ID: timePeriod.dataset.id,
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
                throw new Error('Availability/Update responded with ' + response.status);
            }
            return response.json();
        })
        .then(function (response) {
            if (response.status == "AUTHENTICATION_FAILED") {
                location.href = "/Profile/SignIn";
            }
        });
}

export function deleteTimePeriod(availabilityId) {

    return fetch("Delete", {
        method: "DELETE",
        body: JSON.stringify({
            ID: availabilityId
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