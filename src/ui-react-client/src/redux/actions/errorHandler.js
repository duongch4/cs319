export default function errorHandler(error) {
    let errorParsed = undefined;
    if(error.response && error.response.data && error.response.data.message){
        errorParsed = error.response.data.message;
    } else {
        errorParsed = (error.stack) ? error.stack : error;
    }
    alert(errorParsed);
    return errorParsed;
}