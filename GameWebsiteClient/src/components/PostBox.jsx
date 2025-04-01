import backWhite from "../assets/backWhite.png"
import { useNavigate } from "react-router-dom"

export default function PostBox({ submitPost, post, isCreate }) {
    const navigation = useNavigate()

    const stepBack = () => {
        navigation(-1)
    }

    if (isCreate) {
        return (
            <div className="coolBox centerHorizontal">
                <button onClick={stepBack} className="circleButton backButton"><img src={backWhite} alt="backButton" /></button>
                
                <h1>Poszt létrehozása</h1>
                <form className="postForm" onSubmit={submitPost}>
                    <textarea name="post" id="post" placeholder="Poszt szövege" rows="20" cols="70" required></textarea>
                    <input type="file" id="image" name="image" />
                    <button type="submit">Közzététel</button>
                </form>
            </div>
        )
    }
    else {

        return (
            <div className="coolBox centerHorizontal">
                <button onClick={stepBack} className="circleButton backButton"><img src={backWhite} alt="backButton" /></button>
                <h1>Poszt szerkesztése</h1>
                <form className="postForm" onSubmit={submitPost}>
                    <textarea name="post" id="post" placeholder="Poszt szövege" rows="20" cols="70" required defaultValue={post}></textarea>
                    <input type="file" id="image" name="image" />
                    <button type="submit">Frissítés</button>
                </form>
            </div>
        )
    }

}