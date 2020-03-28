import * as types from './actionTypes';
import { CLIENT_DEV_ENV, SVC_ROOT } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
import axios from 'axios';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}api/users/search`;

export const getUsers = users => {
    return {
        type: types.PERFORM_USER_SEARCH,
        users: users
    };
};

export const performUserSearch = (filterParams, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(getUsers(_initialState.users));
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios
                .post(`${baseURL}`, filterParams, { headers })
                .then(response => {
                    dispatch(getUsers(response.data.payload));
                })
                .catch(error => {
                    let errorParsed = ""
                    console.log(error.response)
                    if(error.response.status === 500){
                        let err = error.response.data.message
                        errorParsed = err.substr(err.indexOf('Message') + 8, err.indexOf('StackTrace') - err.indexOf('Message') - 8);
                        console.log(err)                 
                    } else {
                        errorParsed = error.response.statusText
                    }
                    throw(error);
                })
            })
        }
    }
};
