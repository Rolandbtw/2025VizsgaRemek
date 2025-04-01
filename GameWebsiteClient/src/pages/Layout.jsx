import { Outlet } from "react-router-dom";
import Footer from "../components/Footer";
import NavBar from "../components/NavBar";
import { useState, useEffect, useRef } from "react";
import NavBarMobile from "../components/NavBarMobile";


function Layout({ loggedIn, setLoggedIn, url, headers }) {
    const minWindowHeight = 750
    const [windowWidth, setWindowWidth] = useState(window.innerWidth);
    const mainRef = useRef(null);
    const [footerHeight, setFooterHeight] = useState(0);

    const handleFooterHeightChange = (height) => {
        setFooterHeight(height)
    }

    useEffect(() => {
        const handleResize = () => {
            setWindowWidth(window.innerWidth);
        };

        window.addEventListener("resize", handleResize);
        return () => {
            window.removeEventListener("resize", handleResize);
            
        };
    }, []);

    useEffect(() => {
        if(mainRef.current){
            mainRef.current.style.paddingBottom = `${footerHeight}px`
        }
    }, [footerHeight])

    return (<>
        {windowWidth < minWindowHeight ? <NavBarMobile loggedIn={loggedIn} setLoggedIn={setLoggedIn} url={url} headers={headers} /> : <NavBar loggedIn={loggedIn} setLoggedIn={setLoggedIn} url={url} headers={headers} />}
        
        <main ref={mainRef}>
            <Outlet />
        </main>
        <Footer onHeightChange={handleFooterHeightChange}/>
    </>
    )
}

export default Layout