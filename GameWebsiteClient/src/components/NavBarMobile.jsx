import { Turn as Hamburger } from "hamburger-react"
import { useEffect, useRef, useState } from "react"
import { Link } from "react-router-dom"
import { motion } from "motion/react"
import UserMenu from "./UserMenu"

export default function NavBarMobile({ loggedIn, setLoggedIn, url, headers }) {
    const [isOpen, setIsOpen] = useState(false)
    const [showBar, setShowBar] = useState(false)

    const disableMenus = (options) => {
        if (options == 0) {
            setIsOpen(false)
            setShowBar(false)
        }
        else if (options == 1) {
            setShowBar(false)
        }

    }

    useEffect(() => {
        if (!isOpen) {
            setShowBar(false)
        }
    }, [isOpen])

    const loggedInElements = [
        {
            id: 1,
            path: "/",
            text: "Főoldal"
        },
        {
            id: 2,
            path: "/post",
            text: "Posztok"
        },
        {
            id: 3,
            path: "/user",
            text: "Felhasználók"
        },
        {
            id: 4,
            path: "/achievement",
            text: "Achievementek"
        }
    ]

    const notLoggedInElements = [
        {
            id: 1,
            path: "/",
            text: "Főoldal"
        },
        {
            id: 2,
            path: "/post",
            text: "Posztok"
        },
        {
            id: 3,
            path: "/user",
            text: "Felhasználók"
        },
        {
            id: 4,
            path: "/achievement",
            text: "Achievementek"
        },
        {
            id: 5,
            path: "/login",
            text: "Bejelentkezés"
        },
        {
            id: 6,
            path: "/register",
            text: "Regisztráció"
        },
    ]

    if (localStorage.getItem("token")) {
        return (
            <div>
                <Hamburger toggled={isOpen} size={20} toggle={setIsOpen} rounded label="Show menu" />

                {isOpen &&

                    <motion.nav initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} >
                        <ul>

                            {loggedInElements.map((item) => (
                                <motion.li key={item.id} initial={{ scale: 0, opacity: 0 }} animate={{ scale: 1, opacity: 1 }} transition={{ type: "spring", stiffness: 260, damping: 15, delay: item.id * 0.05 }} onClick={() => disableMenus(0)}><Link className="navElement" to={item.path} >{item.text}</Link></motion.li>
                            ))}
                            <motion.li initial={{ scale: 0, opacity: 0 }} animate={{ scale: 1, opacity: 1 }} transition={{ type: "spring", stiffness: 260, damping: 15, delay: 0.05 * loggedInElements[loggedInElements.length - 1].id }} ><a className="navElement" onClick={() => setShowBar(!showBar)}>Menü</a></motion.li>
                        </ul>
                    </motion.nav>}
                {(showBar === true && isOpen) && <UserMenu loggedIn={loggedIn} setLoggedIn={setLoggedIn} url={url} headers={headers} hidePanels={disableMenus} />}
                <hr />
            </div>
        )
    }
    else {
        return (
            <>
                <Hamburger toggled={isOpen} size={20} toggle={setIsOpen} rounded label="Show menu" />

                {isOpen &&
                    <motion.nav initial={{ scale: 0, opacity: 0 }} animate={{ scale: 1, opacity: 1 }} exit={{ opacity: 0 }} transition={{ type: "spring", stiffness: 260, damping: 20, duration: 0.2 }}>
                        <ul>
                            {notLoggedInElements.map((item) => (
                                <motion.li key={item.id} initial={{ scale: 0, opacity: 0 }} animate={{ scale: 1, opacity: 1 }} transition={{ type: "spring", stiffness: 260, damping: 15, delay: item.id * 0.05 }} onClick={disableMenus}><Link className="navElement" to={item.path} >{item.text}</Link></motion.li>
                            ))}
                        </ul>
                    </motion.nav>}
                <hr />
            </>
        )
    }
}