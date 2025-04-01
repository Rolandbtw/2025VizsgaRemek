import { Link, useRouteError } from "react-router-dom";


function ErrorPage() {

    return (
        <>
            <h1 className="centerScreen">404 A keresett oldal nem található</h1>
            <Link className=" linkButton" to={"/"} >Vissza a főoldalra</Link>
        </>
    )
}

export default ErrorPage