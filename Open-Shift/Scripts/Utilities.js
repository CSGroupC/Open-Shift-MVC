
export const MONTH_NAMES = ["January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
];

// Returns format HH:MM
export function formatTime(time) {
    let coefficient = 1000 * 60 * 1;
    let timeBuffer = new Date(Math.ceil(time.getTime() / coefficient) * coefficient);
    let hours = timeBuffer.getHours();

    if (time.getHours() == 23 && hours == 0)
        hours = 24;

    return hours + ":" + timeBuffer.getMinutes().toString().padStart(2, "0");
}

export function getDateFromQueryString() {
    let requestData = new URLSearchParams(location.search);
    let date = null;

    if (requestData.has("m") && requestData.has("d") && requestData.has("y")) {
        date = new Date(requestData.get("y"), requestData.get("m") - 1, requestData.get("d"));
    } else {
        date = new Date();
        window.history.replaceState(null, null, `?m=${date.getMonth() + 1}&d=${date.getDate()}&y=${date.getFullYear()}`);
        requestData = new URLSearchParams(location.search);
    }

    return date;
}

export function preventDefault(event) {
    event.preventDefault();
    event.stopPropagation();
}