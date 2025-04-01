import processDate from "./processDate"


export default function handleAllDates(obj){
    let dates = {
        created_at: "",
        updated_at: "",
        deleted_at: ""
    }

    if(obj.created_at){
        dates.created_at = processDate(obj.created_at)
    }
    if(obj.updated_at){
        dates.updated_at = processDate(obj.updated_at)
    }
    if(obj.deleted_at){
        dates.deleted_at = processDate(obj.deleted_at)
    }

    return dates
}