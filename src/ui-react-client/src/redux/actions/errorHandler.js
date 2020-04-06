export default function errorHandler(error) {
    let errorParsed = "";
    if(error.response && error.response.data && error.response.data.message){
        errorParsed = error.response.data.message;
    } else {
        errorParsed = error;
    }
    alert(errorParsed);
    return errorParsed;
}