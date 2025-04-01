import { useEffect, useState } from "react"
import { Link, useNavigate } from "react-router-dom"
import ConfirmWindow from "./ConfirmWindow"
import otherRequest from "./otherRequest"
import FeedBack from "./FeedBack"
import CircleLoader from "./CircleLoader"
import closeWhite from "../assets/closeWhite.png"

export default function UserMenu({ loggedIn, setLoggedIn, url, headers, hidePanels }) {
    headers.Authorization = "Bearer " + localStorage.getItem("token")
    const navigation = useNavigate()
    const privilege = localStorage.getItem("privilege")
    const [showConfirm, setShowConfirm] = useState(false)
    const [loading, setLoading] = useState(false)


    async function logout() {
        setLoading(true)
        await otherRequest(url, headers, "user/logout", {}, "PATCH")
        localStorage.clear()
        setLoggedIn(false)
        setLoading(false)
        navigation("/")
    }

    const userId = localStorage.getItem("userId")
    return (
        <div className="menuBox">
            <button className="xButton" onClick={() => hidePanels(1)}><img src={closeWhite} alt="CloseButton" /></button>
            {loading && <CircleLoader />}
            <h1>Menü</h1>
            <hr />
            <Link onClick={() => hidePanels(0)} to={"/user/show/" + userId}>Adatok megjelenítése</Link>
            <Link onClick={() => hidePanels(0)} to={"/post/create"}>Új poszt létrehozása</Link>
            <Link onClick={() => hidePanels(0)} to={"/user/posts"}>Posztjaim</Link>
            {privilege == 10 && <Link onClick={() => hidePanels(0)} to={"/achievement/create"}>Új achievement</Link>}
            <button onClick={() => setShowConfirm(true)}>Kijelentkezés</button>
            {(showConfirm) && <ConfirmWindow text={"Biztosan ki szeretne jelentkezni?"} functionToCall={logout} setShow={setShowConfirm} />}
        </div>

    )
}