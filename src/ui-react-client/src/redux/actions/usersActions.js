import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}users/`;

export const loadUsersAllData = userSummaries => {
  return {
      type: types.LOAD_USERS_ALL,
      userSummaries: userSummaries };
};

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

export const loadSpecificUser = (userID) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            let user = _initialState.userProfiles.filter(userProfile => {
                return userProfile.userSummary.userID == userID;
            });
            dispatch(loadUserProfileData(user[0]));
        } else {
            return axios
                .get(`${baseURL + userID}`, { headers })
                .then(response => {
                    dispatch(loadUserProfileData(response.data.payload))
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
                    console.log(response);
                    dispatch(updateUserProfileData(user));
                })
                .catch(error => {
                    throw error;
                });
        }
    }
};


