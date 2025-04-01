import closeWhite from "../assets/closeWhite.png"

export default function FeedBack({ message, status, setError, setSuccess }) {
    function hideFeedBack() {
        setError("");
        setSuccess("");
    }

    return <div className={`feedBack ${status}`}>
        <p>{message}</p>
        <button className="xButton" onClick={hideFeedBack}><img src={closeWhite} alt="CloseButton" /></button>

    </div>
}