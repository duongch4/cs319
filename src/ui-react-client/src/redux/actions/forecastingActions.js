import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
import axios from 'axios';
import errorHandler from './errorHandler'
import {unassignUpdateUserData} from './userProfileActions'
import {assignUpdateUserData} from './userProfileActions'

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

export const unassignOpeningData = (openingId, userId, confirmedUtilization, userSummaryDisciplineName, opening) => {
    return {
        type: types.UNASSIGN_OPENING,
        openingId: openingId,
        userId: userId,
        confirmedUtilization: confirmedUtilization,
        userSummaryDisciplineName: userSummaryDisciplineName,
        opening: opening
    }
};

export const unassignOpenings = (openingId, userID, confirmedUtilization, userRoles, userSummaryDisciplineName, projectNumber) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(unassignOpeningData(openingId, userID, confirmedUtilization, userSummaryDisciplineName));
        } else {
            return getHeaders(userRoles).then(headers => {
            return axios
                .put(`${baseURL}positions/${openingId}/unassign`, {}, { headers })
                .then(response => {
                    dispatch(unassignUpdateUserData(openingId, response.data.userId, response.data.confirmedUtilization, response.data.opening, projectNumber));
                    dispatch(unassignOpeningData(openingId, response.data.userId, response.data.confirmedUtilization, userSummaryDisciplineName, response.data.opening));
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


export const createAssignOpenings = (opening, userId, confirmedUtilization, user, userRoles, projectSummary, history) => {
    return dispatch => {
      if (CLIENT_DEV_ENV) {
          dispatch(createAssignOpening(opening.positionID, userId, confirmedUtilization, user))
      } else {
        return getHeaders(userRoles).then(headers => {
            return axios
            .put(`${baseURL}positions/${opening.positionID}/assign/${userId}`, {}, { headers })
                .then(response => {
                  dispatch(assignUpdateUserData(response.data.userID, response.data.confirmedUtilization, opening, projectSummary))
                  dispatch(createAssignOpening(response.data.openingId, response.data.userID, response.data.confirmedUtilization, user))
                  history.push('/projects/' + projectSummary.projectNumber);
                })
                .catch(error => {
                    errorHandler(error);
                })
        })
      }
    }
};
