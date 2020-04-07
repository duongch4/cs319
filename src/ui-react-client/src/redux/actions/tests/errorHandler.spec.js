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
            message: 'this should be returned'
        }, 
        statusText: 'this should not be returned'}};
    let received = errorHandler(error);

    expect(alert).toHaveBeenCalled();
    expect(received).toEqual(error.response.data.message);
});
