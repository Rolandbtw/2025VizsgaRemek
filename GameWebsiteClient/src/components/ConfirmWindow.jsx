

export default function ConfirmWindow({ text, functionToCall, setShow }) {
    return (
        <div className="centerScreen confirmWindow">
            <h1>{text}</h1>
            <hr/>
            <button onClick={functionToCall}>Igen</button>
            <button onClick={() => setShow(false)}>Nem</button>
        </div>
    )
}