export default async function getRequest(url, headers, urlArgs)
{
    try {
        const response = await fetch(url + urlArgs, {
            "method": "GET",
            "headers": headers
        });
        const result = await response.json();
        const returnData = {
            response: response,
            result: result
        }

        return returnData
    }
    catch (error) {
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