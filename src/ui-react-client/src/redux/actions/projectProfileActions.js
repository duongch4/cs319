import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
// import _initialState from '../reducers/_initialState';
import _initialState_dev from '../reducers/_initialState_client';
import {updateProjectSummary} from "./projectsActions";

const baseURL = `${SVC_ROOT}api/projects/`;

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

export const loadSingleProject = (projectNumber) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            let project = _initialState_dev.projectProfiles.filter(projectProfile => {
                return projectProfile.projectSummary.projectNumber === projectNumber;
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
                    dispatch(createProjectData(project));
                    alert('Successfully created project ' + response.data.payload.projectNumber);
                })
                .catch(error => {
                    throw error;
                })
        }
    };
};

export const updateProject = (project) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(updateProjectData(project));
        } else {
            return axios
                .put(`${baseURL + project.projectSummary.projectNumber}`,
                    { headers })
                .then(response => {
                    dispatch(updateProjectData(project));
                    dispatch(updateProjectSummary(project.projectSummary));
                    alert('Successfully update project' + response.data.payload.projectNumber);
                })
        }
    };
};

export const deleteProject = number => {
    return dispatch => {
        return axios
            .delete(`${baseURL + number}`)
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
