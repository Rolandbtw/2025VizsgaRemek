import { useState } from "react"


export default function FormInput({ type, inputId, label, onChange, errorMessage, pattern, min, defaultValue }) {
    const [focused, setFocused] = useState(false)

    return (
        <>
            <label htmlFor={inputId}>{label}</label>

            {pattern ? <input type={type} placeholder={label} id={inputId} name={inputId} onBlur={() => setFocused(true)} focused={focused.toString()} onChange={onChange ? onChange : () => { }} required pattern={pattern} min={min ? min : ""} defaultValue={defaultValue ? defaultValue : ""} /> :
                <input type={type} placeholder={label} id={inputId} name={inputId} onBlur={() => setFocused(true)} focused={focused.toString()} onChange={onChange ? onChange : () => { }} required min={min ? min : ""} defaultValue={defaultValue ? defaultValue : ""} />}

            <span className="formError">{errorMessage}</span>
        </>
    )
}