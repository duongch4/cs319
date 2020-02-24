function formatDate(dateString) {
    let monthNames = [
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    ];
    let date = new Date(dateString);
    let monthIndex = date.getMonth();
    let year = date.getFullYear();
    let day = date.getDate();

    return "" + monthNames[monthIndex] + " " + day + " " + year;
}

export { formatDate }
