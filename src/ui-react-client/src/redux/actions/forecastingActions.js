import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
import axios from 'axios';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}api/`;

export const createAssignOpening = (openingId, userId, confirmedUtilization, user) => {
    return {
        type: types.UPDATE_ASSIGN_OPENING,
        openingId: openingId,
        userId: userId,
        confirmedUtilization: confirmedUtilization,
        user: user
    }
};

export const confirmAssignOpening = (openingId, userId, confirmedUtilization, userSummaryDisciplineName) => {
    return {
        type: types.CONFIRM_ASSIGN_OPENING,
        openingId: openingId,
        userId: userId,
        confirmedUtilization: confirmedUtilization,
        userSummaryDisciplineName: userSummaryDisciplineName
    }
};

export const unassignOpening = (openingId, userId, confirmedUtilization, userSummaryDisciplineName) => {
    return {
        type: types.UNASSIGN_OPENING,
        openingId: openingId,
        userId: userId,
        confirmedUtilization: confirmedUtilization,
        userSummaryDisciplineName: userSummaryDisciplineName
    }
};

export const unassignOpenings = (openingId, userID, confirmedUtilization, userRoles, userSummaryDisciplineName) => {
    return dispatch => {
      dispatch(unassignOpening(openingId, userID, confirmedUtilization, userSummaryDisciplineName))
      // TODO once backend unassign API done add proper code below
    }
};

export const confirmAssignOpenings = (openingId, userID, confirmedUtilization, userRoles, userSummaryDisciplineName) => {
    return dispatch => {
      if (CLIENT_DEV_ENV) {
          dispatch(confirmAssignOpening(openingId, userID, confirmedUtilization, userSummaryDisciplineName))
      } else {
        return getHeaders(userRoles).then(headers => {
            return axios
            .put(`${baseURL}positions/${openingId}/confirm`, {}, { headers })
                .then(response => {
                    dispatch(confirmAssignOpening(response.data.openingId, response.data.userID, response.data.confirmedUtilization, userSummaryDisciplineName))
                })
                .catch(error => {
                    let errorParsed = "";
                    console.log(error.response)
                    if(error.response.status === 500){
                        let err = error.response.data.message;
                        errorParsed = err.substr(err.indexOf('Message') + 8, err.indexOf('StackTrace') - err.indexOf('Message') - 8);
                        console.log(err);
                    } else {
                        errorParsed = error.response.statusText;
                    }
                    alert(errorParsed);
                })
        })
      }
    }
};


export const createAssignOpenings = (openingId, userId, confirmedUtilization, user, userRoles) => {
    return dispatch => {
      if (CLIENT_DEV_ENV) {
          dispatch(createAssignOpening(openingId, userId, confirmedUtilization, user))
      } else {
        return getHeaders(userRoles).then(headers => {
            return axios
            .put(`${baseURL}positions/${openingId}/assign/${userId}`, {}, { headers })
                .then(response => {
                  dispatch(createAssignOpening(response.data.openingId, response.data.userID, response.data.confirmedUtilization, user))
                })
                .catch(error => {
                    let errorParsed = "";
                    console.log(error.response)
                    if(error.response.status === 500){
                        let err = error.response.data.message;
                        errorParsed = err.substr(err.indexOf('Message') + 8, err.indexOf('StackTrace') - err.indexOf('Message') - 8);
                        console.log(err);
                    } else {
                        errorParsed = error.response.statusText;
                    }
                    alert(errorParsed);
                })
        })
      }
    }
};
