import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
import axios from 'axios';
import _initialState from '../reducers/_initialState_client';

const baseURL = `${SVC_ROOT}api/users/`;

export const loadUsersAllData = userSummaries => {
    return {
        type: types.LOAD_USERS_ALL,
        users: userSummaries
    };
};

export const updateUserSummary = userSummary => {
    return {
        type: types.UPDATE_USER_SUMMARIES,
        userSummary: userSummary
    }
};

export const loadUsers = () => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(loadUsersAllData(_initialState.userSummaries));
        } else {
            return getHeaders().then(headers => {
                return axios.get(baseURL, { headers });
            }).then(response => {
                dispatch(loadUsersAllData(response.data.payload));
            }).catch(error => {
                throw error;
            });
        }
    };
};


