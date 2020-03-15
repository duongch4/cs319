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

export const deleteDisciplineData = id => {
    return {
        type: types.DELETE_DISCIPLINE,
        id: id
    }
}

export const deleteSkillData = (disciplineID, skillName) => {
    return {
        type: types.DELETE_SKILL,
        disciplineID: disciplineID,
        skillName: skillName
    }
}

export const deleteProvinceData = (provinceName) => {
    return {
        type: types.DELETE_PROVINCE,
        provinceName: provinceName,
    }
}

export const deleteCityData = (cityName, id) => {
    return {
        type: types.DELETE_CITY,
        name: cityName,
        id: id
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

export const deleteDiscipline = (id) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteDisciplineData(id))
        } else {
            // TODO
            dispatch(deleteDisciplineData(id))
        }
    } 
}

export const deleteSkill = (disciplineID, skillName) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteSkillData(disciplineID, skillName))
        } else {
            // TODO
            dispatch(deleteSkillData(disciplineID, skillName))
        }
    } 
}

export const deleteProvince = (provinceName) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteProvinceData(provinceName))
        } else {
            // TODO
            dispatch(deleteProvinceData(provinceName))
        }
    } 
}

export const deleteCity = (cityName, id) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteCityData(cityName, id))
        } else {
            // TODO - backend only needs id, but keep cityName to make the reducer easier to deal with
            dispatch(deleteCityData(cityName, id))
        }
    } 
}