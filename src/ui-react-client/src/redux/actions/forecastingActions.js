import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
import axios from 'axios';
import errorHandler from './errorHandler'
import {unassignUpdateUserData} from './userProfileActions'

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

export const unassignOpening = (openingId, userId, confirmedUtilization, userSummaryDisciplineName, opening, projectName) => {
    return {
        type: types.UNASSIGN_OPENING,
        openingId: openingId,
        userId: userId,
        confirmedUtilization: confirmedUtilization,
        userSummaryDisciplineName: userSummaryDisciplineName,
        opening: opening
    }
};

export const unassignOpenings = (openingId, userID, confirmedUtilization, userRoles, userSummaryDisciplineName, projectName) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(unassignOpening(openingId, userID, confirmedUtilization, userSummaryDisciplineName));
        } else {
            return getHeaders(userRoles).then(headers => {
            return axios
                .put(`${baseURL}positions/${openingId}/unassign`, {}, { headers })
                .then(response => {
                    dispatch(unassignUpdateUserData(openingId, response.data.userId, response.data.confirmedUtilization, response.data.opening, projectName))
                    dispatch(unassignOpening(openingId, response.data.userId, response.data.confirmedUtilization, userSummaryDisciplineName, response.data.opening))
                })
                .catch(error => {
                    errorHandler(error);
                })
            })
        }
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
                    errorHandler(error);
                })
        })
      }
    }
};


export const createAssignOpenings = (openingId, userId, confirmedUtilization, user, userRoles, projectNumber, history) => {
    return dispatch => {
      if (CLIENT_DEV_ENV) {
          dispatch(createAssignOpening(openingId, userId, confirmedUtilization, user))
      } else {
        return getHeaders(userRoles).then(headers => {
            return axios
            .put(`${baseURL}positions/${openingId}/assign/${userId}`, {}, { headers })
                .then(response => {
                  dispatch(createAssignOpening(response.data.openingId, response.data.userID, response.data.confirmedUtilization, user))
                  history.push('/projects/' + projectNumber);
                })
                .catch(error => {
                    errorHandler(error);
                })
        })
      }
    }
};
