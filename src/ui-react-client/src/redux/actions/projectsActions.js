import * as types from './actionTypes';
import { SVC_ROOT } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}projects/`;

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

// export const loadSingleProject = (id) => {
//   return dispatch => {

//     dispatch(loadSingleProjectData(id));
//   };
// };

export const createProject = (project) => {
  return dispatch => {
    dispatch(createProjectData(project))
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
