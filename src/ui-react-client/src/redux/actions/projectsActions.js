import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState_client from '../reducers/_initialState_client';

const baseURL = `${SVC_ROOT}api/projects/`;

export const loadProjectsData = projectSummaries => {
  return {
    type: types.LOAD_PROJECTS_ALL,
    projectSummaries: projectSummaries,
  };
};

export const updateProjectSummary = projectSummary => {
    return {
        type: types.UPDATE_PROJECT_SUMMARY,
        projectSummary: projectSummary
    }
};

export const deleteProjectSummary = projectSummary => {
    return {
        type: types.DELETE_PROJECT_SUMMARY,
        projectSummary: projectSummary
    }
};

export const createProjectSummary = projectSummary => {
    return {
        type: types.CREATE_PROJECT_SUMMARY,
        projectSummary: projectSummary
    }
};

export const loadProjects = () => {
  return dispatch => {
    if (CLIENT_DEV_ENV) {
      dispatch(loadProjectsData(_initialState_client.projectSummaries));
    } else {
      return axios
          .get(baseURL, { headers })
          .then(response => {
            dispatch(loadProjectsData(response.data.payload));
          })
          .catch(error => {
            throw error;
          });
    }
  };
};

export const updateProjectSummaries = (projectSummary) => {
    return dispatch => {
        dispatch(updateProjectSummary(projectSummary))
    }
};

export const deleteProjectSummaries = (projectSummary) => {
    return dispatch => {
        dispatch(deleteProjectSummary(projectSummary))
    }
};

export const createProjectSummaries = (projectSummary) => {
    return dispatch => {
        dispatch(createProjectSummary(projectSummary))
    }
};
