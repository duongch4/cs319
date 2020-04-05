import errorHandler from '../errorHandler'

let alert = jest.spyOn(window, 'alert').mockImplementation(() => {});

it('should return error message if error has no response field', () => {
    let error = "some error message";
    let received = errorHandler(error);

    expect(alert).toHaveBeenCalled();
    expect(received).toEqual(error);
});

it('should parse the response and return the message', () => {
    let error = { response: { 
        status: 400, 
        data: { 
            code: 400,
            status: "Bad Request",
            message: 'this should not be returned'
        }, 
        statusText: 'this should be returned'}};
    let received = errorHandler(error);

    expect(alert).toHaveBeenCalled();
    expect(received).toEqual(error.response.statusText);
});

it('should parse the 500 response message and return the message', () => {
    let errorMessage = 'This message should be returned';
    let error = {response: {
        status: 500, 
        data: {
            code: 500,
            status: 'Internal Server Error',
            message: `\"Source: Core .Net SqlClient Data Provider\n  Message:${errorMessage}StackTrace:    at System.Data.SqlClient.SqlConnection.OnError(SqlException exception)`}, 
            statusText: 'this should be returned'}};
    let received = errorHandler(error);

    expect(alert).toHaveBeenCalled();
    expect(received).toEqual(errorMessage);
});