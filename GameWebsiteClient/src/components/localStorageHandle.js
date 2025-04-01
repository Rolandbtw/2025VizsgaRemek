

export default function SetLocalSorage(token, userId, privilege){
    localStorage.setItem("token", token)
    localStorage.setItem("userId", userId)
    localStorage.setItem("privilege", privilege)
}