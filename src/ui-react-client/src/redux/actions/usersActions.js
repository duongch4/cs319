import * as types from './actionTypes';
import { SVC_ROOT } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';

const baseURL = `${SVC_ROOT}users/`;

export const loadUsersAllData = users => {
  return { type: types.LOAD_USERS_ALL, users: users };
};

export const loadUsers = () => {
  return dispatch => {
    return axios
      .get(baseURL, { headers })
      .then(response => {
        dispatch(loadUsersAllData(response.data));
      })
      .catch(error => {
        throw error;
      });
  };
};
