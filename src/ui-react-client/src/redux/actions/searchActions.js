import * as types from './actionTypes';
import { CLIENT_DEV_ENV, SVC_ROOT } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
import axios from 'axios';
import _initialState from '../reducers/_initialState';
import errorHandler from './errorHandler'

const baseURL = `${SVC_ROOT}api/users/search`;

export const getUsers = (users, isLastPage) => {
    return {
        type: types.PERFORM_USER_SEARCH,
        users: users,
        isLastPage: isLastPage,
    };
};

export const clearSearchResultsData = () => {
    return {
        type: types.CLEAR_SEARCH_RESULTS
    }
}

export const performUserSearch = (filterParams, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(getUsers(_initialState.users));
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios
                .post(`${baseURL}`, filterParams, { headers })
                .then(response => {
                    dispatch(getUsers(response.data.payload, response.data.extra.isLastPage));
                })
                .catch(error => {
                    errorHandler(error);
                    throw(error);
                })
            })
        }
    }
};

export const clearSearchResults = () => {
    return dispatch => {
        dispatch(clearSearchResultsData())
    }
}
