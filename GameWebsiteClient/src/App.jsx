import { BrowserRouter, Navigate, Route, Routes, useNavigate } from 'react-router-dom';
import { useState } from 'react';
import Layout from './pages/Layout';
import Register from './pages/Register';
import Login from "./pages/Login";
import GetUserData from './pages/GetUserData';
import UserUpdate from './pages/UserUpdate.jsx';
import PostCreate from './pages/PostCreate.jsx';
import PostUpdate from './pages/PostUpdate.jsx';
import AchievementCreate from './pages/AchievementCreate.jsx';
import AchievementUpdate from './pages/AchievementUpdate.jsx';
import AchievementList from './pages/AchievementList.jsx';
import ErrorPage from './pages/ErrorPage.jsx';
import GetPosts from './pages/GetPosts.jsx';
import GetPostData from './pages/GetPostData.jsx';
import GetUsers from './pages/GetUsers.jsx';
import otherRequest from './components/otherRequest.js';
import GetOwnPosts from './pages/GetOwnPosts.jsx';
import Home from './pages/Home.jsx';

function App() {
    const url = "http://localhost:8000/api/"
    const headers = {
        "Accept": "application/json",
        "Content-type": "application/json"
    }
    const scrollThreshold = 100

    const [loggedIn, setLoggedIn] = useState((localStorage.getItem("token") ? true : false))
    const [likedPosts, setLikedPosts] = useState([])


    const likePost = async (postId, newPost) => {
        const responseData = await otherRequest(url, headers, "post/like/" + postId, newPost, "PATCH")
        if (newPost.likes) {
            setLikedPosts([...likedPosts, postId])

        }
        else {
            let tempLikedPosts = [...likedPosts]
            tempLikedPosts.splice(tempLikedPosts.indexOf(postId), 1)
            setLikedPosts(tempLikedPosts)

        }
        return responseData
    }

    return (<>
        <BrowserRouter>
            <Routes>
                <Route path='/' element={<Layout loggedIn={loggedIn} setLoggedIn={setLoggedIn} url={url} headers={headers} />} >
                    <Route index element={<Home url={url} headers={headers} />} />
                    <Route path='achievement' element={<AchievementList url={url} headers={headers} />} />
                    <Route path='achievement/create' element={loggedIn ? <AchievementCreate url={url} headers={headers} /> : <Navigate replace to={"/login"} />} />
                    <Route path='achievement/update/:id' element={loggedIn ? <AchievementUpdate url={url} headers={headers} /> : <Navigate replace to={"/login"} />} />
                    <Route path='user/update/:id' element={loggedIn ? <UserUpdate url={url} headers={headers} /> : <Navigate replace to={"/login"} />} />
                    <Route path='user/show/:id' element={loggedIn ? <GetUserData url={url} headers={headers} setLoggedIn={setLoggedIn} /> : <Navigate replace to={"/login"} />} />
                    <Route path='user' element={loggedIn ? <GetUsers url={url} headers={headers} setLoggedIn={setLoggedIn} scrollThreshold={scrollThreshold} /> : <Navigate replace to={"/login"} />} />
                    <Route path='user/posts' element={loggedIn ? <GetOwnPosts url={url} headers={headers} scrollThreshold={scrollThreshold} likedPosts={likedPosts} likePost={likePost} setLikedPosts={setLikedPosts} /> : <Navigate replace to={"/login"} />} />
                    <Route path='post/create' element={loggedIn ? <PostCreate url={url} headers={headers} /> : <Navigate replace to={"/login"} />} />
                    <Route path='post/update/:id' element={loggedIn ? <PostUpdate url={url} headers={headers} /> : <Navigate replace to={"/login"} />} />
                    <Route path='post' element={loggedIn ? <GetPosts url={url} headers={headers} scrollThreshold={scrollThreshold} likedPosts={likedPosts} likePost={likePost} setLikedPosts={setLikedPosts} /> : <Navigate replace to={"/login"} />} />
                    <Route path='post/show/:id' element={loggedIn ? <GetPostData url={url} headers={headers} likedPosts={likedPosts} likePost={likePost} setLikedPosts={setLikedPosts} /> : <Navigate replace to={"/login"} />} />
                </Route>
                <Route path='login' element={<Login url={url} headers={headers} setLoggedIn={setLoggedIn} />} />
                <Route path='register' element={<Register url={url} headers={headers} setLoggedIn={setLoggedIn} />} />
                <Route path='*' element={<ErrorPage />} />
            </Routes>
        </BrowserRouter>
    </>
    )

}

export default App
