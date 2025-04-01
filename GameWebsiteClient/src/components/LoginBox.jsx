import { Link } from "react-router-dom"
import FormInput from "./FormInput"
import backWhite from "../assets/backWhite.png"

export default function LoginBox({ login, stepBack }) {

    return (
        <div className="centerScreen loginSize coolBox">
            <h1>Bejelentkezés</h1>
            <button onClick={stepBack} className="circleButton backButton"><img src={backWhite} alt="backButton" /></button>
            <form className="loginForm" onSubmit={login}>
                <FormInput type={"email"} inputId={"email"} label={"Email cím"} errorMessage={"Email cím megadása kötelező"} />
                <FormInput type={"password"} inputId={"password"} label={"Jelszó"} errorMessage={"Jelszó megadása kötelező"} />
                <button type="submit">Bejelentkezés</button>
            </form>
            <p className="registerText">Még nincsen fiókod? <Link to={"../register"} >Regisztráció</Link></p>
        </div>
    )
}