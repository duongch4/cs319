import * as types from './actionTypes';
import { SVC_ROOT } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}users/`;

export const loadUserProfileData = userID => {
  return { type: types.LOAD_USERS_SPECIFIC, userID: userID };
};

export const loadSpecificUser = (userID) => {
  return dispatch => {
    dispatch(loadUserProfileData(userID));
    // XXX TODO: Uncomment this for full-stack integration
  };
};
