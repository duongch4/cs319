import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState_client from '../reducers/_initialState_client';

const baseURL = `${SVC_ROOT}api/projects/`;


export const createProject = project => {
  return {
    type: types.CREATE_PROJECT,
    project: project,
  };
};

export const loadProjectsData = projectSummaries => {
  return {
    type: types.LOAD_PROJECTS_ALL,
    projects: projectSummaries,
  };
};

export const deleteProjectSummaryData = projectNumber => {
    return {
        type: types.DELETE_PROJECT_SUMMARY,
        projectNumber: projectNumber
    }
};

export const addProjectSummaryData = projectSummary => {
    return {
        type: types.ADD_PROJECT_SUMMARY,
        projectSummary: projectSummary
    }
};

export const updateProjectSummaryData = projectSummary => {
    return {
        type: types.UPDATE_PROJECT_SUMMARY,
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

// export const updateProjectSummaries = (projectSummary) => {
//     return dispatch => {
//         dispatch(updateProjectSummary(projectSummary))
//     }
// };

// export const deleteProjectSummaries = (projectSummary) => {
//     return dispatch => {
//         dispatch(deleteProjectSummary(projectSummary))
//     }
// };

// export const createProjectSummaries = (projectSummary) => {
//     return dispatch => {
//         dispatch(createProjectSummary(projectSummary))
//     }
// };


