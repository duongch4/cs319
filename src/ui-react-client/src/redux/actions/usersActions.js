import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
import axios from 'axios';
import _initialState from '../reducers/_initialState_client';
import errorHandler from './errorHandler'

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

export const loadUsers = (filter, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(loadUsersAllData(_initialState.userSummaries));
        } else {
            var url = baseURL.concat(filter);
            return getHeaders(userRoles).then(headers => {
                return axios.get(url, { headers });
            }).then(response => {
                dispatch(loadUsersAllData(response.data.payload));
            }).catch(error => {
                errorHandler(error);
                throw(error);
            })
        }
    };
};


