import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}api/`;

export const loadMasterlistsData = masterlist => {
    return {
      type: types.LOAD_MASTERLIST,
      masterlist: masterlist
    };
};

export const createDiscplineData = disciplines => {
    return {
        type: types.CREATE_DISCIPLINE,
        disciplines: disciplines
    }
}

export const loadMasterlists = () => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(loadMasterlistsData(_initialState.masterlist));
        } else {
            return axios
                .get(`${baseURL}masterlists/`, { headers })
                .then (response => {
                    dispatch(loadMasterlistsData(response.data.payload));
                })
                .catch(error => {
                    throw error;
                });
        }
    }
};

export const createDiscpline = (discipline) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createDiscplineData(discipline))
        } else {
            // TODO
        }
    }
}