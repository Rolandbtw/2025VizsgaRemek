import { useEffect, useState } from "react";
import getRequest from "../components/getRequest";
import CircleLoader from "../components/CircleLoader";
import { Link } from "react-router-dom"
import AchievementItem from "../components/AchievementItem";
import FeedBack from "../components/FeedBack";
import NoData from "../components/NoData";
import plusLogo from '../assets/plusWhite.png'

function AchievementList({ url, headers }) {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false)
    const [success, setSuccess] = useState("")
    const [error, setError] = useState("")
    const privilege = localStorage.getItem("privilege")

    const token = localStorage.getItem("token")
    if (token) {
        headers.Authorization = "Bearer " + token
    }

    useEffect(() => {
        setLoading(true)
        const fetchAchievements = async () => {
            const data = await getRequest(url, headers, "achievement")
            if (data.response.status == 200) {
                setData(data.result.achievements)
            }
            else {
                setError(data.result.message)
            }
            setLoading(false)
        }

        fetchAchievements();
    }, [])


    return (
        <div>
            <h1 className="achievementMargin">Achievements</h1>
            {loading && <CircleLoader />}
            {privilege == 10 && <Link className="linkButton" to={"/achievement/create"} ><img className="smallPicture" src={plusLogo} alt="newButton" />Új achievement</Link>}
            {(error || success) && <FeedBack message={error ? error : success} status={error ? "failure" : "success"} setError={setError} setSuccess={setSuccess} />}
            {(!data && !loading) && <NoData />}
            <div>
                {data.map((item) => (
                    <AchievementItem key={item.id} achievement={item} setError={setError} setSuccess={setSuccess} url={url} headers={headers} />
                ))}
            </div>

            {/* <ul>
                {data.map((item) => (
                    <li key={item.id}>{item.name}</li>
                ))}
            </ul> */}
        </div>
    )


}

export default AchievementList;