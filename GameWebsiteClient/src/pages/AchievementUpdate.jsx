import { useEffect, useState } from "react"
import { useParams, useNavigate } from "react-router-dom"
import FeedBack from "../components/FeedBack"
import AchievementBox from "../components/AchievementBox"
import getRequest from "../components/getRequest"
import otherRequest from "../components/otherRequest"
import CircleLoader from "../components/CircleLoader"

export default function AchievementUpdate({ url, headers }) {
    headers.Authorization = "Bearer " + localStorage.getItem("token")
    const navigation = useNavigate()
    const [error, setError] = useState("")
    const [success, setSuccess] = useState("")
    const [achievement, setAchievement] = useState({})
    const id = useParams().id
    const [loading, setLoading] = useState(false)

    useEffect(() => {
        setLoading(true)
        const getAchievement = async () => {

            const responseData = await getRequest(url, headers, "achievement/" + id)
            if (responseData.response.status == 200) {
                setAchievement(responseData.result.achievement)
            }
            else {
                setError(responseData.result.message)
            }
            setLoading(false)
        }
        getAchievement()

    }, [])

    const updateAchievement = async (e) => {
        setLoading(true)
        e.preventDefault()

        const newAchievement = {
            name: e.target.name.value,
            description: e.target.description.value,
            field: e.target.field.value,
            threshold: e.target.threshold.value
        }

        const responseData = await otherRequest(url, headers, "achievement/" + achievement.id, newAchievement, "PATCH")

        setLoading(false)
        if (responseData.response.status == 200) {
            setError("")
            setSuccess(responseData.result.message)
            setTimeout(() => {
                navigation("/achievement")
            }, 1000);
        }
        else {
            setError(responseData.result.message)
        }
    }

    return (
        <div>
            {loading && <CircleLoader />}
            {(error || success) && <FeedBack message={error ? error : success} status={error ? "failure" : "success"} setError={setError} setSuccess={setSuccess} />}
            <AchievementBox name={achievement.name} selected={achievement.field} threshold={achievement.threshold} description={achievement.description} submitAchievement={updateAchievement} isCreate={false} />
        </div>
    )


}