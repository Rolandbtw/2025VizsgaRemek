import FormInput from "./FormInput";


export default function UserUpdateBox({ submitUser, name, email }) {
    return (
        <div className="coolBox loginSize centerHorizontal">
            <form className="loginForm" onSubmit={submitUser}>
                <FormInput type={"text"} inputId={"name"} label={"Új felhasználónév"} errorMessage={"Felhasználnév megadása kötelező, legalább 3 karakter hosszúnak kell lennie"} pattern={".{3,}"} defaultValue={name} />
                <FormInput type={"email"} inputId={"email"} label={"Email cím"} errorMessage={"Email cím megadása kötelező"} defaultValue={email} />
                <button type="submit">Frissítés</button>
            </form>
        </div>
    )
}