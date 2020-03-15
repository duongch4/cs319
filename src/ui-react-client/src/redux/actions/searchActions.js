import * as types from './actionTypes';
import {CLIENT_DEV_ENV, SVC_ROOT} from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState_client from '../reducers/_initialState_client';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}api/users/search`;

export const getUsers = userProfiles => {
    return {
      type: types.PERFORM_USER_SEARCH,
      userProfiles: userProfiles
    };
  };

export const performUserSearch = (filterParams) => {
return dispatch => {
    if (CLIENT_DEV_ENV) {
        dispatch(getUsers(filterParams));
    } else {
    return axios
        .get(`${baseURL}`, {headers})
        .then(response => {
            dispatch(getUsers(filterParams));
        })
        .catch(error => {
            throw error;
        });
    }
};
};