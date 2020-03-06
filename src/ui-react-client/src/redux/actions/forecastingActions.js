import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}api/`;

export const createAssignOpening = (projectSummary) => {
    return {
      //add if client dev and todo for axios
        type: types.UPDATE_ASSIGN_OPENING,
        projectSummary: projectSummary
    }
};

export const createAssignOpenings = (projectSummary) => {
    return dispatch => {
      if (CLIENT_DEV_ENV) {
          dispatch(createAssignOpening(projectSummary))
      } else {
          // TODO
      }
    }
};
