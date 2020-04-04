import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
import axios from 'axios';
import _initialState_client from '../reducers/_initialState_client';
import { addProjectSummaryData, deleteProjectSummaryData, updateProjectSummaryData } from "./projectsActions";
import errorHandler from './errorHandler'

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

export const deleteProjectData = () => {
    return {
        type: types.DELETE_PROJECT
    };
};

export const loadSingleProject = (projectNumber, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            let project = _initialState_client.projectProfiles.filter(projectProfile => {
                return projectProfile.projectSummary.projectNumber === projectNumber;
            });
            dispatch(loadSingleProjectData(project[0]));
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios.get(`${baseURL + projectNumber}`, { headers });
            }).then(response => {
                dispatch(loadSingleProjectData(response.data.payload));
            }).catch(error => {
                errorHandler(error);
                });
        }
    };
};

export const createProject = (project, history, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createProjectData(project))
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios.post(baseURL, project, { headers });
            }).then(_ => {
                dispatch(createProjectData(project));
                dispatch(addProjectSummaryData(project.projectSummary));
                history.push('/projects/' + project.projectSummary.projectNumber);
            }).catch(error => {
                errorHandler(error);
            })
        }
    };
};

export const updateProject = (project, history, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(updateProjectData(project));
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios.put(`${baseURL + project.projectSummary.projectNumber}`, project, { headers });
            }).then(_ => {
                dispatch(updateProjectData(project));
                dispatch(updateProjectSummaryData(project.projectSummary));
                history.push('/projects/' + project.projectSummary.projectNumber);
            }).catch(error => {
                errorHandler(error);
            })
        }
    };
};

export const deleteProject = (number, history, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteProjectData(number))
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios.delete(`${baseURL + number}`, { headers });
            }).then(_ => {
                dispatch(deleteProjectData());
                dispatch(deleteProjectSummaryData(number));
                history.push('/projects');
            }).catch(error => {
                errorHandler(error);
            })
        }
    };
};
