import trophy from "../assets/award.png";

export default function ShowAchievement({ name, description}){
    return (
        <div className="achievement">
            <img src={trophy} alt="award" />
            <div>
                <p>{name}</p>
                <p>{description}</p>
            </div>
        </div>
    )
}