

export default function processDate(dateString){
    const dateSplit = dateString.split("T")
    const dateYear = dateSplit[0]
    const dateTime = dateSplit[1].split(".")[0]

    const newTime = {
        year: dateYear,
        time: dateTime
    }

    return newTime
}