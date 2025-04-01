import FormInput from "./FormInput"
import backWhite from "../assets/backWhite.png"


export default function RegisterBox({ register, password, onChange, stepBack }) {

    return (
        <div className="centerScreen loginSize coolBox">
            <h1>Regisztráció</h1>
            <button onClick={stepBack} className="circleButton backButton"><img src={backWhite} alt="backButton" /></button>
            <form className="loginForm" onSubmit={register}>
                <FormInput type={"email"} inputId={"email"} label={"Email cím"} errorMessage={"Email cím megadása kötelező"} />
                <FormInput type={"text"} inputId={"name"} label={"Felhasználónév"} errorMessage={"Felhasználnév megadása kötelező, legalább 3 karakter hosszúnak kell lennie"} pattern={".{3,}"} />
                <FormInput type={"password"} inputId={"password"} label={"Jelszó"} onChange={onChange} errorMessage={"Jelszó megadása kötelező, legalább 8 karakter, tartalmazzon kis- és nagy betűt, és számot"} pattern={"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$"} />
                <FormInput type={"password"} inputId={"password2"} label={"Jelszó újra"} errorMessage={"Két jelszó nem egyezik"} pattern={password} />
                <button type="submit">Regisztráció</button>
            </form>
        </div>
    )
}