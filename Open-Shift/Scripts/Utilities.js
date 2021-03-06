﻿
export const MONTH_NAMES = ["January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
];

// Source: https://stackoverflow.com/questions/3426404/create-a-hexadecimal-colour-based-on-a-string-with-javascript
// This converts a given string to a color (hexidecimal format) that's unique to that string
export function nameToColor(id, name) {

    let str = name.slice(0, name.length / 2) + id + name.slice(name.length / 2);
    var hash = 0;
    for (var i = 0; i < str.length; i++) {
        hash = str.charCodeAt(i) + ((hash << 5) - hash);
    }
    var color = "#";
    for (var i = 0; i < 3; i++) {
        var value = (hash >> (i * 8)) & 0xFF;
        color += ("00" + value.toString(16)).substr(-2);
    }
    return color;
}

// Expects Date object
// Returns format "HH:MM"
export function formatTime(datetime) {
    let coefficient = 1000 * 60 * 1;
    let timeBuffer = new Date(Math.ceil(datetime.getTime() / coefficient) * coefficient);
    let hours = timeBuffer.getHours();

    if (datetime.getHours() == 23 && hours == 0)
        hours = 24;

    return hours + ":" + timeBuffer.getMinutes().toString().padStart(2, "0");
}

// Expected format: "YYYY-MM-DDTHH:MM:SS"
export function stringToDate(string) {
    let buffer = string.split("T");
    let dateParts = buffer[0].split("-");
    let timeParts = buffer[1].split(":");

    return new Date(dateParts[0], dateParts[1], dateParts[2], timeParts[0], timeParts[1], timeParts[2]);
}

export function getDateFromQueryString() {
    let requestData = new URLSearchParams(location.search);
    let date = null;

    if (requestData.has("m") && requestData.has("d") && requestData.has("y")) {
        date = new Date(requestData.get("y"), requestData.get("m") - 1, requestData.get("d"));
    } else {
        date = new Date();
        // NOTE: The controller depends on reloading the page with the query string
        //window.history.replaceState(null, null, `?m=${date.getMonth() + 1}&d=${date.getDate()}&y=${date.getFullYear()}`);
        location.replace(`?m=${date.getMonth() + 1}&d=${date.getDate()}&y=${date.getFullYear()}`);
        requestData = new URLSearchParams(location.search);
    }

    return date;
}

export function preventDefault(event) {
    event.preventDefault();
    event.stopPropagation();
}

export const Event = {

    areFalseClicksPrevented: false,
    pointerDownElement: null,

    // NOTE: Calling this function changes the way click events are fired. 
    // It will prevent any click events from firing unless the mousedown and mouseup
    // both directly targetted the same element.
    // This is useful for apps with "click and drag" behavior
    preventFalseClicks: function () {

        if (this.areFalseClicksPrevented == false) {
            this.areFalseClicksPrevented = true;

            document.body.addEventListener("mousedown", (event) => {
                this.pointerDownElement = event.target;
            }, true);

            document.body.addEventListener("click", (event) => {
                if (event.target != this.pointerDownElement) {
                    event.preventDefault();
                    event.stopPropagation();
                }

                this.pointerDownElement = null;
            }, true);
        }
    },

    // Wrapper function for mouse/touch events
    // NOTE: This function does not account for multi-touching
    PointerHandler: function (callback, doDefault = false) {

        return (event) => {

            // Reassign the event object to the appropriate event (either the touch or the original mouse event)
            if ("changedTouches" in event && event.changedTouches.length > 0) {
                event.clientX = event.changedTouches[0].clientX;
                event.clientY = event.changedTouches[0].clientY;
                if (!("pageX" in event)) event.pageX = event.changedTouches[0].pageX;
                if (!("pageY" in event)) event.pageY = event.changedTouches[0].pageY;
                event.screenX = event.changedTouches[0].screenX;
                event.screenY = event.changedTouches[0].screenY;

                if (doDefault == false) {
                    event.stopPropagation();
                    // Don't prevent default on mobile, to allow scrolling
                    //event.preventDefault();
                }
            } else {

                if (doDefault == false) {
                    event.stopPropagation();
                    event.preventDefault();
                }
            }

            callback(event);
        }
    }
};