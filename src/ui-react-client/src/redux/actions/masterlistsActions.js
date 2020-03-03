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

export const createSkillData = skill => {
    return {
        type: types.CREATE_SKILL,
        skill: skill
    }
}

export const createProvinceData = location => {
    return {
        type: types.CREATE_PROVINCE,
        location: location
    }
}

export const createCityData = location => {
    return {
        type: types.CREATE_CITY,
        location: location
    }
}

export const editCityData = location => {
    return {
        type: types.EDIT_CITY,
        location: location
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
    // TODO: disciplineID is currently the discipline name - once IDs are brought in we will change it to ID
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createDiscplineData(discipline))
        } else {
            // TODO
        }
    }
}

export const createSkill = (skill) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createSkillData(skill))
        } else {
            // TODO
        }
    } 
}

export const createProvince = (location) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createProvinceData(location))
        } else {
            // TODO
        }
    } 
}

export const createCity = (location) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createCityData(location))
        } else {
            // TODO
        }
    } 
}

export const editCity = (location) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(editCityData(location))
        } else {
            // TODO
        }
    } 
}