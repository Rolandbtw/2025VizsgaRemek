import { useState } from "react"
import FeedBack from "../components/FeedBack"
import { useNavigate } from "react-router-dom"
import RegisterBox from "../components/RegisterBox"
import SetLocalSorage from "../components/localStorageHandle"
import otherRequest from "../components/otherRequest"
import CircleLoader from "../components/CircleLoader"

export default function Register({ url, headers, setLoggedIn }) {

    const navigation = useNavigate()
    const [error, setError] = useState("")
    const [success, setSuccess] = useState("")
    const [values, setValues] = useState({
        name: "",
        email: "",
        password: "",
        password2: ""
    })
    const [loading, setLoading] = useState(false)

    const stepBack = () => {
        navigation(-1)
    }

    async function addUser(url, name, email, password) {
        setLoading(true)
        const userData = {
            name: name,
            email: email,
            password: password
        }

        const responseData = await otherRequest(url, headers, "user/register", userData, "POST")

        if (responseData.response.status == 200) {
            SetLocalSorage(responseData.result.user.token, responseData.result.user.id, responseData.result.user.privilege)
            setLoggedIn(true)
            setError("")
            setSuccess(responseData.result.message)
            setTimeout(() => {
                navigation("/")
            }, 1000);
        }
        else {
            setError(responseData.result.message)
        }
        setLoading(false)
    }

    const register = async (event) => {
        event.preventDefault();

        await addUser(url, event.target.name.value, event.target.email.value, event.target.password.value)

    }

    const onChange = (e) => {
        setValues({ ...values, [e.target.name]: e.target.value })
    }

    return (
        <div>
            {(error || success) && <FeedBack message={error ? error : success} status={error ? "failure" : "success"} setError={setError} setSuccess={setSuccess} />}
            <RegisterBox register={register} password={values.password} onChange={onChange} stepBack={stepBack} />
            {loading && <CircleLoader />}

        </div>
    )

}