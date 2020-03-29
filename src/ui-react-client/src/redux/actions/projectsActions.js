import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
import axios from 'axios';
import _initialState_client from '../reducers/_initialState_client';

const baseURL = `${SVC_ROOT}api/projects/`;

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
              dispatch(loadProjectsData(response.data.payload));
          })
          .catch(error => {
            let errorParsed = ""
            console.log(error.response)
            if(error.response.status === 500){
                let err = error.response.data.message
                errorParsed = err.substr(err.indexOf('Message') + 8, err.indexOf('StackTrace') - err.indexOf('Message') - 8);
                console.log(err)                 
            } else {
                errorParsed = error.response.statusText
            }
            throw(error)
          })
        })
    }
  }
};