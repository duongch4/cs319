import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
import axios from 'axios';
import _initialState_client from '../reducers/_initialState_client';
import errorHandler from './errorHandler'

const baseURL = `${SVC_ROOT}api/projects/`;

export const loadProjectsData = (projectSummaries, isLastPage) => {
  return {
    type: types.LOAD_PROJECTS_ALL,
    projects: projectSummaries,
    isLastPage: isLastPage,
  };
};

export const loadProjects = (filterParams, userRoles) => {
  return async dispatch => {
    if (CLIENT_DEV_ENV) {
        dispatch(loadProjectsData(_initialState_client.projectSummaries));
    } else {
        return getHeaders(userRoles).then(headers => {
          var url = baseURL.concat(filterParams);
          return axios
          .get(`${url}`, { headers })
          .then(response => {
              dispatch(loadProjectsData(response.data.payload, response.data.extra.isLastPage));
          })
          .catch(error => {
            errorHandler(error);
            throw(error)
          })
        })
    }
  }
};