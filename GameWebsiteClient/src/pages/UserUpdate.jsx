import { useEffect, useState } from "react"
import { useParams, useNavigate } from "react-router-dom"
import FeedBack from "../components/FeedBack"
import UserUpdateBox from "../components/UserUpdateBox"
import otherRequest from "../components/otherRequest"
import getRequest from "../components/getRequest"
import CircleLoader from "../components/CircleLoader"


export default function UserUpdate({ url, headers }) {
    headers.Authorization = "Bearer " + localStorage.getItem("token")
    const navigation = useNavigate()
    const [error, setError] = useState("")
    const [success, setSuccess] = useState("")
    const [user, setUser] = useState({})
    const [loading, setLoading] = useState(false)
    const id = useParams().id

    useEffect(() => {
        const getUser = async () => {
            setLoading(true)
            const returnData = await getRequest(url, headers, "user/" + id)

            if (returnData.response.status == 200) {
                setUser(returnData.result.user)
            }
            else {
                setError(returnData.result.message)
            }
            setLoading(false)
        }
        getUser()

    }, [])

    const updateUser = async (e) => {
        e.preventDefault()
        setLoading(true)
        const newUser = {
            name: e.target.name.value,
            email: e.target.email.value
        }

        const returnData = await otherRequest(url, headers, "user/update/" + user.id, newUser, "PUT")

        setLoading(false)
        if (returnData.response.status == 200) {
            setError("")
            setSuccess(returnData.result.message)
            setTimeout(() => {
                navigation(-1)
            }, 1000);
        }
        else {
            setError(returnData.result.message)
        }
    }

    return (
        <div>
            {loading && <CircleLoader />}
            {(error || success) && <FeedBack message={error ? error : success} status={error ? "failure" : "success"} setError={setError} setSuccess={setSuccess} />}
            <UserUpdateBox name={user.name} email={user.email} submitUser={updateUser} />
        </div>
    )

}