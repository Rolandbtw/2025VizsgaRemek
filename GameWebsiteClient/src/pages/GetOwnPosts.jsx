import { useState, useEffect } from "react";
import getRequest from "../components/getRequest";
import { useNavigate } from "react-router-dom"
import PostList from "../components/PostList";
import FilterOptions from "../components/FilterOptions";
import NoData from "../components/NoData";
import CircleLoader from "../components/CircleLoader";
import FeedBack from "../components/FeedBack";

export default function GetOwnPosts({ url, headers, likedPosts, likePost, setLikedPosts, scrollThreshold }) {
    headers.Authorization = "Bearer " + localStorage.getItem("token")

    const [page, setPage] = useState(1)
    const [searchPage, setSearchPage] = useState(1)
    const [sortBy, setSortBy] = useState("id")
    const [sortDir, setSortDir] = useState("asc")
    const [data, setData] = useState([])
    const [loading, setLoading] = useState(false)
    const navigation = useNavigate()
    const [search, setSearch] = useState("")
    const [dataMaxNum, setDataMaxNum] = useState(0)
    const [error, setError] = useState("")
    const [success, setSuccess] = useState("")

    const fetchPosts = async () => {
        setLoading(true)
        let responseData;

        if (search) {
            responseData = await getRequest(url, headers, "user/post/search/" + sortBy + "/" + sortDir + "/" + search + "/?page=" + searchPage)
        }
        else {
            responseData = await getRequest(url, headers, "user/" + sortBy + "/" + sortDir + "/?page=" + page)
        }

        if (responseData.response.status == 200) {
            setLikedPosts(responseData.result.likedPosts)
            setDataMaxNum(responseData.result.posts.total)
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
            <FilterOptions changeSortBy={changeSortBy} changeSortDir={changeSortDir} search={searchPost} mode={"post"} />
            {(error || success) && <FeedBack message={error ? error : success} status={error ? "failure" : "success"} setError={setError} setSuccess={setSuccess} />}
            {((success || data.length == 0) && !loading) && <NoData />}
            {data.map((item) => (
                <PostList key={item.id} post={item} viewPost={viewPost} likePost={likePost} likedPostsArr={likedPosts} editPost={editPost} url={url} headers={headers} setError={setError} setSuccess={setSuccess} admin={true} />
            ))}
            {(loading && (data.length < dataMaxNum || data.length == 0)) && <CircleLoader />}

        </div>

    )
}