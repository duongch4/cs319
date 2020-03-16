import * as types from './actionTypes';
import {CLIENT_DEV_ENV, SVC_ROOT} from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}api/users/search`;

export const getUsers = users => {
    return {
      type: types.PERFORM_USER_SEARCH,
      users: users
    };
  };

export const performUserSearch = (filterParams) => {
return dispatch => {
    if (CLIENT_DEV_ENV) {
        dispatch(getUsers(filterParams));
    } else {
    return axios
        .post(`${baseURL}`, filterParams, {headers})
        .then(response => {
            dispatch(getUsers(response.data.payload));
        })
        .catch(error => {
            throw error;
        });
    }
};
};