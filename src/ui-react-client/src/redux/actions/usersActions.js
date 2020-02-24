import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState from '../reducers/_initialState_client';

const baseURL = `${SVC_ROOT}api/users/`;

export const loadUsersAllData = userSummaries => {
  return {
      type: types.LOAD_USERS_ALL,
      userSummaries: userSummaries };
};

export const loadUsers = () => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(loadUsersAllData(_initialState.userSummaries));
        } else {
            return axios
                .get(baseURL, { headers} )
                .then(response => {
                    dispatch(loadUsersAllData(response.data.payload));
                })
                .catch(error => {
                    throw error;
                });
        }
    };
};


