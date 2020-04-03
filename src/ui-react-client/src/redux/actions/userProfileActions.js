import * as types from './actionTypes';
import { CLIENT_DEV_ENV, SVC_ROOT } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
import axios from 'axios';
import _initialState from '../reducers/_initialState';
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

export const loadSpecificUser = (userID, userRoles) => {
  return dispatch => {
    if (CLIENT_DEV_ENV) {
      let user = _initialState.userProfiles.filter(userProfile => {
        return String(userProfile.userSummary.userID) === String(userID); // redux thinks one is a string and the other is a number
      });
      dispatch(loadUserProfileData(user[0]));
    } else {
      return getHeaders(userRoles).then(headers => {
        return axios.get(`${baseURL + userID}`, { headers });
      }).then(response => {
        dispatch(loadUserProfileData(response.data.payload));
      }).catch(error => {
            let errorParsed = ""
            console.log(error.response)
            if(error.response.status === 500){
                let err = error.response.data.message
                errorParsed = err.substr(err.indexOf('Message') + 8, err.indexOf('StackTrace') - err.indexOf('Message') - 8);
                console.log(err)                 
            } else {
                errorParsed = error.response.statusText
            }
            alert(errorParsed)
        })
    }
  };
};

export const updateSpecificUser = (user, history, userRoles) => {
  return dispatch => {
    if (CLIENT_DEV_ENV) {
      dispatch(updateUserProfileData(user));
    } else {
      return getHeaders(userRoles).then(headers => {
        return axios.put(baseURL + user.userSummary.userID, user, { headers });
      }).then(_ => {
        dispatch(updateUserProfileData(user));
        dispatch(updateUserSummary(user.userSummary));
        history.push('/users/' + user.userSummary.userID);
      }).catch(error => {
            let errorParsed = ""
            console.log(error.response)
            if(error.response.status === 500){
                let err = error.response.data.message
                errorParsed = err.substr(err.indexOf('Message') + 8, err.indexOf('StackTrace') - err.indexOf('Message') - 8);
                console.log(err)                 
            } else {
                errorParsed = error.response.statusText
            }
            alert(errorParsed)
        })
    }
  }
};

