import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}projects/`;

export const loadProjectsData = projectSummaries => {
  return {
    type: types.LOAD_PROJECTS_ALL,
    projectSummaries: projectSummaries,
  };
};

export const loadSingleProjectData = projectProfile => {
  return {
    type: types.LOAD_SINGLE_PROJECT,
    projectProfile: projectProfile
  }
};

export const createProjectData = projectProfile => {
  return {
    type: types.CREATE_PROJECT,
    projectProfile: projectProfile,
  };
};

export const updateProjectData = projectProfile => {
  return {
    type: types.UPDATE_PROJECT,
    projectProfile: projectProfile,
  };
};

export const deleteProjectData = projectProfile => {
  return {
    type: types.DELETE_PROJECT,
    projectProfile: projectProfile,
  };
};

export const loadProjects = () => {
  return dispatch => {
    if (CLIENT_DEV_ENV) {
      dispatch(loadProjectsData(_initialState.projectSummaries));
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

export const loadSingleProject = (projectNumber) => {
  return dispatch => {
    if (CLIENT_DEV_ENV) {
      let project = _initialState.projectProfiles.filter(projectProfile => {
        return projectProfile.projectSummary.projectNumber == projectNumber;
      });
      dispatch(loadSingleProjectData(project[0]));
    } else {
      return axios
          .get(`${baseURL + projectNumber}`, { headers })
          .then(response => {
            dispatch(loadSingleProjectData(response.data.payload))
          })
          .catch(error => {
            throw error;
          });
    }
  };
};

export const createProject = (project) => {
  return dispatch => {
    if (CLIENT_DEV_ENV) {
      dispatch(createProjectData(project))
    } else {
      return axios
          .post(baseURL, project,{ headers })
          .then(response => {
            dispatch(createProjectData(project))
          })
          .catch(error => {
            throw error;
          })
    }
  };
};

export const updateProject = (project) => {
  return dispatch => {
    dispatch(updateProjectData(project));
    // return axios
    //   .put(baseURL, { headers })
    //   .then(response => {
    //     dispatch(updateProjectData(response.data));
    //   })
    //   .catch(error => {
    //     throw error;
    //   });
  };
};

export const deleteProject = number => {
  return dispatch => {
    // dispatch(deleteProjectData(response.data));
    // return axios
    //   .delete(`${baseURL}${number}`, { headers })
    //   .then(response => {
    //     dispatch(deleteProjectData(response.data));
    //   })
    //   .catch(error => {
    //     throw error;
    //   });
  };
};
