export default function errorHandler(error) {
    let errorParsed = ""
    console.log(error)
    if(error.response &&  error.response.status === 500){
        let err = error.response.data.message
        errorParsed = err.substr(err.indexOf('Message') + 8, err.indexOf('StackTrace') - err.indexOf('Message') - 8);
        console.log(err)                 
    } else if(error.response){
        errorParsed = error.response.text
    } else {
        errorParsed = error
    }
    alert(errorParsed)
    return errorParsed;
}