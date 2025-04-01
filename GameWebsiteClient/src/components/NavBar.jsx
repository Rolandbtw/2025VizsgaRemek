import { useState } from "react"
import { Link } from "react-router-dom"
import UserMenu from "./UserMenu"
import userPic from '../assets/user.png'

function NavBar({ loggedIn, setLoggedIn, url, headers }) {
    const [showBar, setShowBar] = useState(false)

    const disableMenu = () => {
        setShowBar(false)
    }

    if (localStorage.getItem("token")) {
        return (
            <>
                <nav>
                    <ul>
                        <li><Link onClick={() => setShowBar(false)} className="navElement" to={"/"}>Főoldal</Link></li>
                        <li><Link onClick={() => setShowBar(false)} className="navElement" to={"/post"}>Posztok</Link></li>
                        <li><Link onClick={() => setShowBar(false)} className="navElement" to={"/user"}>Felhasználók</Link></li>
                        <li><Link onClick={() => setShowBar(false)} className="navElement" to={"/achievement"}>Achievementek</Link></li>
                        <li><img className="navElement userImage" onClick={() => setShowBar(!showBar)} src={userPic} alt="user" /> <p className="navElement profilP" onClick={() => setShowBar(!showBar)}>Profil</p> </li>

                    </ul>
                </nav>
                {showBar === true && <UserMenu loggedIn={loggedIn} setLoggedIn={setLoggedIn} url={url} headers={headers} hidePanels={disableMenu} />}
                <hr />
            </>
        )
    }
    else {
        return (
            <>
                <nav>
                    <ul>
                        <li><Link onClick={() => setShowBar(false)} className="navElement" to={"/"}>Főoldal</Link></li>
                        <li><Link onClick={() => setShowBar(false)} className="navElement" to={"/post"}>Posztok</Link></li>
                        <li><Link onClick={() => setShowBar(false)} className="navElement" to={"/user"}>Felhasználók</Link></li>
                        <li><Link onClick={() => setShowBar(false)} className="navElement" to={"/achievement"}>Achievementek</Link></li>
                        <li><Link onClick={() => setShowBar(false)} className="navElement" to={"/login"}>Bejelentkezés</Link></li>
                        <li><Link onClick={() => setShowBar(false)} className="navElement" to={"/register"}>Regisztráció</Link></li>
                    </ul>
                </nav>
                <hr />
            </>
        )
    }

}

export default NavBar