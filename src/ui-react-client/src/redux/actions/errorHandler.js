export default function errorHandler(error) {
    let errorParsed = "";
    if(error.response &&  error.response.status === 500){
        let err = error.response.data.message;
        errorParsed = err.substr(err.indexOf('Message') + 8, err.indexOf('StackTrace') - err.indexOf('Message') - 8);
        console.log(err);
    } else if(error.response){
        errorParsed = error.response.data.message;
    } else {
        errorParsed = error;
    }
    alert(errorParsed);
    return errorParsed;
}