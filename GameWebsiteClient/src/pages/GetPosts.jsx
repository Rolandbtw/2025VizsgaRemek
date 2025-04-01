import { useEffect, useState } from "react"
import getRequest from "../components/getRequest"
import PostList from "../components/PostList"
import { Link, useNavigate } from "react-router-dom"
import NoData from "../components/NoData"
import FilterOptions from "../components/FilterOptions"
import CircleLoader from "../components/CircleLoader"
import FeedBack from "../components/FeedBack"
import plusLogo from '../assets/plusWhite.png'


export default function GetPosts({ url, headers, likedPosts, likePost, setLikedPosts, scrollThreshold }) {
    headers.Authorization = "Bearer " + localStorage.getItem("token")
    const [page, setPage] = useState(1)
    const [searchPage, setSearchPage] = useState(1)
    const [sortBy, setSortBy] = useState("id")
    const [sortDir, setSortDir] = useState("asc")
    const [data, setData] = useState([])
    const [loading, setLoading] = useState(false)
    const navigation = useNavigate()
    const privilege = localStorage.getItem("privilege")
    const [search, setSearch] = useState("")
    const [dataMaxNum, setDataMaxNum] = useState(0)
    const [error, setError] = useState("")
    const [success, setSuccess] = useState("")
    // const [likedPosts, setLikedPosts] = useState([])

    const fetchPosts = async () => {
        setLoading(true)
        let responseData
        if (search) {
            responseData = await getRequest(url, headers, "post/search/" + sortBy + "/" + sortDir + "/" + search + "/?page=" + searchPage)
        }
        else {
            responseData = await getRequest(url, headers, "post/" + sortBy + "/" + sortDir + "/?page=" + page)
        }
        if (responseData.response.status == 200) {
            setLikedPosts(responseData.result.likedPosts)
            setDataMaxNum(responseData.result.posts.total)
            // setData(responseData.result.posts.data)
            setData((prevData) => [...prevData, ...responseData.result.posts.data])
        }
        else {
            setError(responseData.result.message)
        }

        setLoading(false)
    }

    useEffect(() => {
        if (data.length < dataMaxNum || data.length == 0) {
            fetchPosts()
        }

    }, [page, searchPage, sortBy, sortDir, search])

    const handleScroll = () => {
        // if (document.body.scrollHeight - 200 < window.scrollY + window.innerHeight) {
        //     setLoading(true)
        // }
        setTimeout(() => {
            if (document.body.scrollHeight - scrollThreshold < window.scrollY + window.innerHeight) {
                setLoading(true)
            }
        }, 500);

    }

    window.addEventListener("scroll", handleScroll)


    useEffect(() => {
        if (loading == true && data.length > 0) {
            if (search) {
                setSearchPage((prevPage) => prevPage + 1)
            }
            else {
                setPage((prevPage) => prevPage + 1)
            }
        }
    }, [loading])

    const viewPost = (postId) => {
        navigation("/post/show/" + postId)
    }

    const changeSortBy = (e) => {
        setData([])
        setSortBy(e.target.value)
    }

    const changeSortDir = (e) => {
        setData([])
        setSortDir(e.target.value)
    }

    const editPost = (postId) => {
        navigation("/post/update/" + postId)
    }


    const searchPost = async (event) => {
        event.preventDefault();
        if (event.target.searchBar.value != "") {
            setData([])
            setPage(1)
            setSearchPage(1)
            setSearch(event.target.searchBar.value)
        }
        else {
            setData([])
            setSearchPage(1)
            setSearch("")
        }
    }

    return (
        <div>
            {(error || success) && <FeedBack message={error ? error : success} status={error ? "failure" : "success"} setError={setError} setSuccess={setSuccess} />}
            <FilterOptions changeSortBy={changeSortBy} changeSortDir={changeSortDir} search={searchPost} mode={"post"} />
            <Link className="linkButton" to={"/post/create"} ><img className="smallPicture" src={plusLogo} alt="newButton" />Új poszt</Link>
            {(data.length == 0 && !loading) && <NoData />}
            {privilege == 10 && data.map((item) => (
                <PostList key={item.id} post={item} url={url} headers={headers} viewPost={viewPost} likePost={likePost} likedPostsArr={likedPosts} editPost={editPost} admin={true} setError={setError} setSuccess={setSuccess} />
            ))}
            {privilege == 1 && data.map((item) => (
                <PostList key={item.id} post={item} url={url} headers={headers} viewPost={viewPost} likePost={likePost} likedPostsArr={likedPosts} admin={false} />
            ))}
            {(loading && (data.length < dataMaxNum || data.length == 0)) && <CircleLoader />}

        </div>

    )

}