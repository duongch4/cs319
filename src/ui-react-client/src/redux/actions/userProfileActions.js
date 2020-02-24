import * as types from './actionTypes';
import {CLIENT_DEV_ENV, SVC_ROOT} from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState_client from '../reducers/_initialState_client';

const baseURL = `${SVC_ROOT}api/users/`;

export const loadUserProfileData = userProfile => {
  return {
    type: types.LOAD_USERS_SPECIFIC,
    userProfile: userProfile
  };
};

export const updateUserProfileData = userProfile => {
  return {
    type: types.UPDATE_USERS_SPECIFIC,
    userProfile: userProfile
  }
};

export const loadSpecificUser = (userID) => {
  return dispatch => {
    if (CLIENT_DEV_ENV) {
      let user = _initialState_client.userProfiles.filter(userProfile => {
        return userProfile.userSummary.userID === userID;
      });
      dispatch(loadUserProfileData(user[0]));
    } else {
      return axios
          .get(`${baseURL + userID}`, { headers })
          .then(response => {
            dispatch(loadUserProfileData(response.data.payload));
          })
          .catch(error => {
            throw error;
          });
    }
  };
};

export const updateSpecificUser = (user) => {
  return dispatch => {
    if (CLIENT_DEV_ENV) {
      dispatch(updateUserProfileData(user));
    } else {
      return axios
          .put(baseURL, user, { headers })
          .then(response => {
            dispatch(updateUserProfileData(user));
          })
          .catch(error => {
            throw error;
          });
    }
  }
};

