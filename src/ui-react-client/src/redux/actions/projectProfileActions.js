import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState_client from '../reducers/_initialState_client';
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
            let project = _initialState_client.projectProfiles.filter(projectProfile => {
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
                    project, { headers })
                .then(response => {
                    dispatch(updateProjectData(project));
                    alert('Successfully update project' + response.data.payload.projectNumber);
                })
        }
    };
};

export const deleteProject = number => {
    //TODO: Delete project api
    return dispatch => {
        if (CLIENT_DEV_ENV) {

        } else {
            return axios
                .delete(`${baseURL + number}`)
                .then(response => {
                    // stuff here
                });
        }
    };
};
