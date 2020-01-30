import * as types from './actionTypes';
import { SVC_ROOT } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}projects/`;
// XXX DO NOT LET ME COMMIT THESE CHANGES XXX

export const loadProjectsMostRecentData = projects => {
  return {
    type: types.LOAD_PROJECTS_MOST_RECENT,
    projects: projects,
  };
};

export const loadProjectsData = projects => {
  return {
    type: types.LOAD_PROJECTS_ALL,
    projects: projects,
  };
};

export const createProjectData = project => {
  return {
    type: types.CREATE_PROJECT,
    project: project,
  };
};

export const updateProjectData = project => {
  return {
    type: types.UPDATE_PROJECT,
    project: project,
  };
};

export const deleteProjectData = project => {
  return {
    type: types.DELETE_PROJECT,
    project: project,
  };
};

export const loadProjectsMostRecent = () => {
  return dispatch => {
// XXX DO NOT LET ME COMMIT THESE CHANGES XXX
    dispatch(loadProjectsMostRecentData(_initialState.projects));
    // return axios
    //   .get(`${baseURL}most-recent`, { headers })
    //   .then(response => {
    //     dispatch(loadProjectsMostRecentData(response.data));
    //   })
    //   .catch(error => {
    //     throw error;
    //   });
  };
};

export const loadProjects = () => {
  return dispatch => {
    // XXX DO NOT LET ME COMMIT THESE CHANGES XXX

    dispatch(loadProjectsData(_initialState.projects));
    // return axios
    //   .get(baseURL, { headers })
    //   .then(response => {
    //     dispatch(loadProjectsData(response.data));
    //   })
    //   .catch(error => {
    //     throw error;
    //   });
  };
};

export const createProject = () => {
  return dispatch => {
    // XXX DO NOT LET ME COMMIT THESE CHANGES XXX
    // dispatch(createProjectData(response.data));
    // return axios
    //   .post(baseURL, { headers })
    //   .then(response => {
    //     dispatch(createProjectData(response.data));
    //   })
    //   .catch(error => {
    //     throw error;
    //   });
  };
};

export const updateProject = () => {
  return dispatch => {
    // XXX DO NOT LET ME COMMIT THESE CHANGES XXX
    // dispatch(updateProjectData(response.data));
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
    // XXX DO NOT LET ME COMMIT THESE CHANGES XXX
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
