

export default async function otherRequest(url, headers, urlArgs, body, method){
    try{
        const response = await fetch(url + urlArgs, {
            method: method,
            headers: headers,
            body: JSON.stringify(body)
        });
        const result = await response.json();
        
        const returnData = {
            response: response,
            result: result
        }

        return returnData
    }catch(error){
        return {
            response : {
                status: 503
            },
            result: {
                message: error.message

            }
        }
    }
}