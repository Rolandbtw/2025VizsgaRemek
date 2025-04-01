import { useEffect, useState } from "react"
import getRequest from "../components/getRequest"
import UserDataShow from "../components/UserDataShow"
import { Link, useParams, useNavigate, useLocation } from "react-router-dom"
import deleteRequest from "../components/deleteRequest"
import otherRequest from "../components/otherRequest"
import CircleLoader from "../components/CircleLoader"
import FeedBack from "../components/FeedBack"
import ConfirmWindow from "../components/ConfirmWindow"
import deleteLogo from '../assets/delete.png'
import editLogo from '../assets/edit.png'


export default function GetUserData({ url, headers, setLoggedIn }) {
    headers.Authorization = "Bearer " + localStorage.getItem("token")
    const id = useParams().id
    const [error, setError] = useState("")
    const [success, setSuccess] = useState("")
    const [user, setUser] = useState({})
    const [achievements, setAchievements] = useState([])
    const [loading, setLoading] = useState(false)
    const privilege = localStorage.getItem("privilege")
    const navigation = useNavigate()
    const [showConfirm, setShowConfirm] = useState(false)
    const [confirmData, setConfirmData] = useState({})
    const location = useLocation()

    useEffect(() => {
        setLoading(true)
        const getUserData = async () => {
            const responseData = await getRequest(url, headers, "user/" + id)
            if (responseData.response.status == 200) {
                setUser(responseData.result.user)
                setAchievements(responseData.result.achievements)
                console.log(responseData)
            }
            else {
                setError(responseData.result.message)
            }
            setLoading(false)
        }

        getUserData()
    }, [location])

    const deleteUser = async (userId) => {
        setLoading(true)
        const responseData = await deleteRequest(url, headers, "user/" + userId)
        if (responseData.response.status == 200) {
            setError("")
            setSuccess(responseData.result.message)
            setUser(responseData.result.user)
        }
        else {
            setError(responseData.result.message)
        }
        setShowConfirm(false)
        setLoading(false)

    }

    const restoreUser = async (userId) => {
        setLoading(true)
        const responseData = await deleteRequest(url, headers, "user/restore/" + userId)
        if (responseData.response.status == 200) {
            setError("")
            setSuccess(responseData.result.message)
            setUser(responseData.result.user)
        }
        else {
            setError(responseData.result.message)
        }
        setShowConfirm(false)
        setLoading(false)
    }

    const makeUserAdmin = async (userId) => {
        setLoading(true)
        const requestBody = {
            privilege: 10
        }
        const responseData = await otherRequest(url, headers, "user/update/privilege/" + userId, requestBody, "PATCH")
        if (responseData.response.status == 200) {
            setError("")
            setSuccess(responseData.result.message)
            setUser(responseData.result.user)
        }
        else {
            setError(responseData.result.message)
        }
        setShowConfirm(false)
        setLoading(false)
    }

    const confirmSetup = (type, userId) => {
        if (type == "delete") {
            setShowConfirm(true)
            setConfirmData({
                text: "Biztosan törölni szeretné a felhasználót",
                functionToCall: () => deleteUser(userId),
                setShow: setShowConfirm
            })
        }
        else if (type == "restore") {
            setShowConfirm(true)
            setConfirmData({
                text: "Biztosan vissza szeretné állítani a felhasználót",
                functionToCall: () => restoreUser(userId),
                setShow: setShowConfirm
            })
        }
        else if (type == "makeAdmin") {
            setShowConfirm(true)
            setConfirmData({
                text: "Biztosan adminná szeretné tenni a felhasználót",
                functionToCall: () => makeUserAdmin(userId),
                setShow: setShowConfirm
            })
        }
    }

    return (
        <>
            {loading && <CircleLoader />}
            {(error || success) && <FeedBack message={error ? error : success} status={error ? "failure" : "success"} setError={setError} setSuccess={setSuccess} />}
            <UserDataShow user={user} achievements={achievements} admin={privilege == 10 ? true : false} />
            {((id === localStorage.getItem("userId") || privilege == 10) && !error && !loading) && <Link className="link" to={"/user/update/" + id}> <img src={editLogo} alt="Edit" /> </Link>}
            {/* {(privilege == 10 && !error && !loading) && <button onClick={user.deleted_at ? () => restoreUser(id) : () => deleteUser(id)}>{user.deleted_at ? "Vissaállítás" : "Fiók törlése"}</button>} */}
            {(privilege == 10 && !error && !loading) && <button className="dataButton" onClick={user.deleted_at ? () => confirmSetup("restore", id) : () => confirmSetup("delete", id)}>{user.deleted_at ? "Vissaállítás" : <img src={deleteLogo} alt="Delete" />}</button>}
            {(privilege == 10 && user.privilege != 10 && !error && !loading) && <button className="dataButtonAdmin" onClick={() => confirmSetup("makeAdmin", id)}>Adminná tevés</button>}
            {(showConfirm) && <ConfirmWindow text={confirmData.text} functionToCall={confirmData.functionToCall} setShow={confirmData.setShow} />}
        </>

    )

}