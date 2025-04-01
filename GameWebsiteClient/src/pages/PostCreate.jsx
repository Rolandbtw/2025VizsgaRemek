import FeedBack from "../components/FeedBack"
import PostBox from "../components/PostBox"
import { useNavigate } from "react-router-dom"
import { useState } from "react"
import otherRequest from "../components/otherRequest"
import CircleLoader from "../components/CircleLoader"
import backWhite from "../assets/backWhite.png"

export default function PostCreate({ url, headers }) {
    headers.Authorization = 'Bearer ' + localStorage.getItem("token")

    const navigation = useNavigate()
    const [error, setError] = useState("")
    const [success, setSuccess] = useState("")
    const [loading, setLoading] = useState(false)

    async function submitPost(post, image) {
        setLoading(true)
        const data = { post: post, image: image }

        const responseData = await otherRequest(url, headers, "post", data, "POST")

        setLoading(false)
        if (responseData.response.status == 200) {
            setError("")
            setSuccess(responseData.result.message)
            setTimeout(() => {
                navigation("/post/show/" + responseData.result.post.id)
            }, 1000);
        }
        else {
            setError(responseData.result.message)
        }
    }


    const createPost = async (e) => {
        setLoading(true)
        e.preventDefault()

        let reader = new FileReader()

        if (e.target.image["files"][0]) {
            reader.readAsDataURL(e.target.image["files"][0])

            reader.onload = async function () {
                await submitPost(e.target.post.value, reader.result)
            }
        }
        else {
            await submitPost(e.target.post.value)
        }
        setLoading(false)

    }


    return (
        <div>            
            {loading && <CircleLoader />}
            {(error || success) && <FeedBack message={error ? error : success} status={error ? "failure" : "success"} setError={setError} setSuccess={setSuccess} />}
            <PostBox submitPost={createPost} isCreate={true} />

        </div>
    )

}