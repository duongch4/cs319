import * as types from './actionTypes';
import { CLIENT_DEV_ENV, SVC_ROOT } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
import axios from 'axios';
import _initialState from '../reducers/_initialState';
import errorHandler from './errorHandler'

const baseURL = `${SVC_ROOT}api/users/`;

export const unassignUpdateUserData = (openingID, userID, confirmedUtilization, opening, projectNumber) => {
  return {
    type: types.UNASSIGN_UPDATE_USER_DATA,
    openingID: openingID,
    userID: userID,
    confirmedUtilization: confirmedUtilization,
    opening: opening,
    projectNumber: projectNumber
  };
};

export const assignUpdateUserData = (userID, confirmedUtilization, opening, projectSummary) => {
  return {
    type: types.ASSIGN_UPDATE_USER_DATA,
    userID: userID,
    confirmedUtilization: confirmedUtilization,
    opening: opening,
    projectSummary: projectSummary
  };
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

export const updateUserSummary = userSummary => {
  return {
      type: types.UPDATE_USER_SUMMARIES,
      userSummary: userSummary
  }
};

export const updateUserProjectsDeletionData = projectNumber => {
  return {
    type: types.UPDATE_USER_PROJECTS_DELETION,
    projectNumber: projectNumber
  }
};

export const updateUserProjectsCreationData = (projectSummary, userID) => {
  return {
    type: types.UPDATE_USER_PROJECTS_CREATION,
    projectSummary: projectSummary,
    userID: userID
  }
};

export const updateUserUtilizationData = (userID, utilization) => {
  return {
    type: types.UPDATE_USER_UTILIZATION,
    userID: userID,
    utilization: utilization
  }
}

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
        errorHandler(error);
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
        errorHandler(error);
      })
    }
  }
};
