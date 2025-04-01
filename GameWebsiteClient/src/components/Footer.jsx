import { useRef, useEffect, useState } from "react";
import mailLogo from "../assets/mailWhite.png"
import phoneLogo from "../assets/phoneWhite.png"

function Footer({onHeightChange}) {
    const footerRef = useRef(null);

    useEffect(() =>{
        const updateFooterHeight = () => {
            if(footerRef.current){
                onHeightChange(footerRef.current.offsetHeight)
            }
        }

        updateFooterHeight()
        window.addEventListener("resize", updateFooterHeight)

        return () => {
            window.removeEventListener('resize', updateFooterHeight)
        }
        
    }, [])
   
    return (
        <footer ref={footerRef}>
            <div className="footerElement">
                <h2 className="centerText">R.O.L.A.N.D</h2>
                <p className="centerText">Egy brutális játék melyben a cél a hullámokban érkező ellenségek legyőzése, egy ellenséges, folyton változó kazamatákban.</p>
            </div>
            <div className="footerElement">
                <h2>Elérhetőségek</h2>
                <div className="contactElement">
                    <img className="icon" src={mailLogo} alt="Email logo" />
                    <p>roli.rozsalyi@gmail.com</p>
                </div>
                <div className="contactElement">
                    <img className="icon" src={phoneLogo} alt="Email logo" />
                    <p>+36 70 359 1818</p>
                </div>
            </div>
        </footer>
    )
}

export default Footer