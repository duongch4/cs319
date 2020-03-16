import * as types from './actionTypes';
import {CLIENT_DEV_ENV, SVC_ROOT} from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState_client from '../reducers/_initialState_client';
import {updateUserSummary} from "./usersActions";

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
        return String(userProfile.userSummary.userID) === String(userID); // redux thinks one is a string and the other is a number
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

export const updateSpecificUser = (user, history) => {
  return dispatch => {
    if (CLIENT_DEV_ENV) {
      dispatch(updateUserProfileData(user));
    } else {
      return axios
          .put(baseURL + user.userSummary.userID, user, { headers })
          .then(response => {
            dispatch(updateUserProfileData(user));
            dispatch(updateUserSummary(user.userSummary));
            history.push('/users/' + user.userSummary.userID);
          })
          .catch(error => {
            throw error;
          });
    }
  }
};

