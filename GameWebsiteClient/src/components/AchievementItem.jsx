import { Link } from "react-router-dom"
import { useState } from "react"
import deleteRequest from "./deleteRequest"
import ConfirmWindow from "./ConfirmWindow"
import CircleLoader from "./CircleLoader"
import editLogo from '../assets/edit.png'
import deleteLogo from '../assets/delete.png'

export default function AchievementItem({ achievement, setError, setSuccess, url, headers }) {
    const privilege = localStorage.getItem("privilege")
    const [achievementState, setAchievementState] = useState(achievement)
    const [showConfirm, setShowConfirm] = useState(false)
    const [loading, setLoading] = useState(false)

    const deleteAchievement = async (achievementId) => {
        setLoading(true)
        const responseData = await deleteRequest(url, headers, "achievement/" + achievementId)
        if (responseData.response.status == 200) {
            setAchievementState(responseData.result.achievement)
            setError("")
            setSuccess(responseData.result.message)
        }
        else {
            setError(responseData.result.message)
        }
        setShowConfirm(false)
        setLoading(false)
    }

    const restoreAchievement = async (achievementId) => {
        setLoading(true)
        const responseData = await deleteRequest(url, headers, "achievement/restore/" + achievementId)
        if (responseData.response.status == 200) {
            setAchievementState(responseData.result.achievement)
            setError("")
            setSuccess(responseData.result.message)
        }
        else {
            setError(responseData.result.message)
        }
        setShowConfirm(false)
        setLoading(false)
    }

    return (
        <div className={achievementState.deleted_at ? "listBox deleteBox" : "listBox"}>
            {loading && <CircleLoader />}
            <h2>{achievement.name}</h2>
            <p>{achievement.description}</p>
            {achievementState.deleted_at && <p>Törölve</p>}
            {privilege == 10 && <Link className="link inlineDisplay" to={"/achievement/update/" + achievement.id}><img src={editLogo} alt="editButton" /></Link>}
            {privilege == 10 && <button className="circleButton delete" onClick={() => setShowConfirm(true)}>{achievementState.deleted_at ? "Visszaállítás" : <img src={deleteLogo} alt="deleteButton" />}</button>}
            {(showConfirm) && <ConfirmWindow text={achievementState.deleted_at ? "Biztosan vissza szeretné állítani az achievementet?" : "Biztosan törölni szeretné az achievementet?"} functionToCall={achievementState.deleted_at ? () => restoreAchievement(achievementState.id) : () => deleteAchievement(achievementState.id)} setShow={setShowConfirm} />}
        </div>
    )
}