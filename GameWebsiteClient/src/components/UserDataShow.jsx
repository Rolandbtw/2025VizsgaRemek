import { useState } from "react";
import handleAllDates from "./handleAllDates";
import ShowAchievement from "./ShowAchievement";


export default function UserDataShow({ user, achievements, admin }) {
    const processedDates = handleAllDates(user)
    if (admin) {
        return (
            <>
                <div className="userDataShow">
                    <p>Név: {user.name} <br /> {user.privilege == 10 && "Admin"}</p>
                    <p>Email: {user.email}</p>
                    <p>Halálok: </p><p className="right">{user.deaths}</p>
                    <p>Waves: </p><p className="right">{user.waves}</p>
                    <p>Kills: </p><p className="right">{user.kills}</p>
                    <p>1. Boss ölés: </p><p className="right">{user.boss1lvl}</p>
                    <p>2. Boss ölés: </p><p className="right">{user.boss2lvl}</p>
                    <p>3. Boss ölés: </p><p className="right">{user.boss3lvl}</p>
                    <div>
                        {user.deleted_at && <p><i>Törölve:</i> {processedDates.deleted_at.year} {processedDates.deleted_at.time}</p>}
                        <p><i>Módosítva:</i> {processedDates.updated_at.year} {processedDates.updated_at.time}</p>
                        <p><i>Létrehozás:</i> {processedDates.created_at.year} {processedDates.created_at.time}</p>
                    </div>

                </div>
                <div>
                    <h2 className="achievementTitle">Achievements:</h2>
                    <p className="centerText">{achievements.length} / {user.achievement_count}</p>
                    {
                        achievements.map((item) => (
                            <ShowAchievement key={item.id} name={item.name} description={item.description} />
                        ))
                    }
                </div>
            </>
        )
    }
    else {
        return (
            <>
                <div className="userDataShow">
                    <p>Név: {user.name} {user.privilege == 10 && <span>Admin</span>}</p>
                    <p>Email: {user.email}</p>
                    <p>Halálok: </p><p className="right">{user.deaths}</p>
                    <p>Waves: </p><p className="right">{user.waves}</p>
                    <p>Kills: </p><p className="right">{user.kills}</p>
                    <p>1. Boss ölés: </p><p className="right">{user.boss1lvl}</p>
                    <p>2. Boss ölés: </p><p className="right">{user.boss2lvl}</p>
                    <p>3. Boss ölés: </p><p className="right">{user.boss3lvl}</p>
                    <p><i>Létrehozás:</i> {processedDates.created_at.year} {processedDates.created_at.time}</p>

                </div>
                <div>
                    <h2 className="achievementTitle">Achievements:</h2>
                    {
                        achievements.map((item) => (
                            <ShowAchievement key={item.id} name={item.name} description={item.description} />
                        ))
                    }
                </div>
            </>
        )
    }

}