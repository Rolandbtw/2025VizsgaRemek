import handleAllDates from "./handleAllDates"
import { useState } from "react"
import deleteRequest from "./deleteRequest"
import CircleLoader from "./CircleLoader"
import ConfirmWindow from "./ConfirmWindow"
import likeLogo from '../assets/heartWhite.png'
import editLogo from '../assets/edit.png'
import deleteLogo from '../assets/delete.png'
import likeON from '../assets/heartON.png'
import userPic from '../assets/user.png'

export default function PostList({ post, url, headers, viewPost, editPost, likePost, likedPostsArr, admin, setError, setSuccess }) {
    const privilege = localStorage.getItem("privilege")
    const [postState, setPostState] = useState(post)
    const processedDates = handleAllDates(postState)
    const [showConfirm, setShowConfirm] = useState(false)
    const [loading, setLoading] = useState(false)
    const postTitleCharacterLimit = 50

    const deletePost = async (postId) => {
        setLoading(true)
        const responseData = await deleteRequest(url, headers, "post/" + postId)
        if (responseData.response.status == 200) {
            setError("")
            setSuccess(responseData.result.message)
            setPostState(responseData.result.post)
        }
        else {
            setError(responseData.result.message)
        }
        setShowConfirm(false)
        setLoading(false)

    }

    const restorePost = async (postId) => {
        setLoading(true)

        const responseData = await deleteRequest(url, headers, "post/restore/" + postId)
        if (responseData.response.status == 200) {
            setError("")
            setSuccess(responseData.result.message)
            setPostState(responseData.result.post)

        }
        else {
            setError(responseData.result.message)
        }
        setShowConfirm(false)
        setLoading(false)

    }

    const startLikeProcess = async (postId, likes) => {
        setLoading(true)

        const responseData = await likePost(postId, likes)

        if (responseData.response.status == 200) {
            setPostState(responseData.result.post)
        }
        else {
            setError(responseData.result.message)
        }
        setLoading(false)

    }

    let longPost = false
    if (postState.post.length > postTitleCharacterLimit) {
        longPost = true
    }

    if (admin) {
        if (privilege == 10 || !postState.deleted_at) {
            return (<>
                <div className={postState.deleted_at ? "listBox deleteBox" : "listBox"}>

                    <h2 className="postTitle" onClick={() => viewPost(post.id)}>{postState.post.substring(0, postTitleCharacterLimit)}{longPost && "..."}</h2>
                    {(loading) && <CircleLoader position={5} />}
                    <div><img className="profileImage" src={userPic} alt="user" /><p className="userName">{postState.name}</p></div>

                    <div className="postBottom">



                        <div className="likeNumber">
                            <button className="circleButton edit" onClick={() => editPost(post.id)}><img src={editLogo} alt="szerkesztés" /></button>
                            <button className="circleButton delete" onClick={() => setShowConfirm(true)}>{postState.deleted_at ? "Visszaállítás" : <img src={deleteLogo} alt="törlés" />}</button>
                            <button className="circleButton like" onClick={() => startLikeProcess(post.id, { likes: (!likedPostsArr.includes(post.id) ? true : false) })}><img src={(!likedPostsArr.includes(post.id) ? likeLogo : likeON)} alt="likeButton" /></button>
                            <p>{postState.likes}</p>

                        </div>
                        <div className="dates">
                            {postState.deleted_at && <p className="topDate"><i>Törölve:</i> {processedDates.deleted_at.year} {processedDates.deleted_at.time}</p>}
                            <p><i>Közzététel:</i> {processedDates.created_at.year} {processedDates.created_at.time}</p>
                            <p className="bottomDate"><i>Módosítva:</i> {processedDates.updated_at.year} {processedDates.updated_at.time}</p>
                        </div>
                    </div>
                </div>
                {(showConfirm) && <ConfirmWindow text={postState.deleted_at ? "Biztosan vissza szeretné állítani a posztot?" : "Biztosan törölni szeretné a posztot?"} functionToCall={postState.deleted_at ? () => restorePost(postState.id) : () => deletePost(postState.id)} setShow={setShowConfirm} />}
            </>
            )
        }

    }
    else {
        return (
            <div className="listBox">

                <h2 className="postTitle" onClick={() => viewPost(post.id)}>{postState.post}</h2>
                {(loading) && <CircleLoader />}
                <div><img className="profileImage" src={userPic} alt="user" /><p className="userName">{postState.name}</p></div>

                <div className="postBottom">

                    <div className="likeNumber">
                        <button className="circleButton like" onClick={() => startLikeProcess(post.id, { likes: (!likedPostsArr.includes(post.id) ? true : false) })}><img src={(!likedPostsArr.includes(post.id) ? likeLogo : likeON)} alt="likeButton" /></button>
                        <p>{postState.likes}</p>
                    </div>
                    <div className="dates">
                        <p> <i>Közzététel:</i>{processedDates.created_at.year} {processedDates.created_at.time}</p>
                        <p className="bottomDate"> <i>Módosítva:</i>{processedDates.updated_at.year} {processedDates.updated_at.time}</p>
                    </div>
                </div>

            </div>
        )
    }


}