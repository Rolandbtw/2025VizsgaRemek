

export default function FilterOptions({changeSortBy, changeSortDir, search, mode}){
    return (
        
            <div className="searchContainer">
                <form className="searchForm" onSubmit={search}>
                    <input type="text" name="searchBar" id="searchBar" />
                    <button className="searchButton" type="submit">Keresés</button>
                </form>
        
                <div className="sortContainer">
                    <select name="sortBy" id="sortBy" onChange={changeSortBy} defaultValue={"id"}>
                        <option value="id">Rendezés szerint</option>
                        <option value="created_at">Dátum</option>
                        {mode == "post" && <option value="likes">Like</option>}
                    </select>
                    <select name="sortDir" id="sortDir" onChange={changeSortDir} defaultValue={"asc"}>
                        <option value="asc">Növekvő</option>
                        <option value="desc">Csökkenő</option>
                    </select>
                </div>
            </div>
            
        
    )
}