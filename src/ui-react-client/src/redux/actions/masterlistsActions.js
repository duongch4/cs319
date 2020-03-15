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
            return axios
                .post(`${baseURL}admin/disciplines`, discipline, { headers })
                .then(response => {
                    discipline.id = response.data.payload;
                    dispatch(createDiscplineData(discipline))
                })
                .catch(error => {
                    throw error;
                })
        }
    }
}

export const createSkill = (skill) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createSkillData(skill))
        } else {
            return axios
                .post(`${baseURL}admin/disciplines/${skill.disciplineID}/skills`, skill, { headers })
                .then(response => {
                    dispatch(createSkillData(skill))
                })
                .catch(error => {

                    throw error;
                })
        }
    } 
}

export const createProvince = (location) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createProvinceData(location))
        } else {
            // TODO
            return axios
                .post(`${baseURL}admin/provinces`, location, { headers })
                .then(response => {
                    dispatch(createProvinceData(location))
                })
                .catch(error => {

                    throw error;
                })
        }
    } 
}

export const createCity = (location) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createCityData(location))
        } else {
            return axios
            .post(`${baseURL}admin/locations`, location, { headers })
            .then(response => {
                location.id = response.data.payload;
                dispatch(createCityData(location))
            })
            .catch(error => {

                throw error;
            })
        }
    } 
}

export const deleteDiscipline = (id) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteDisciplineData(id))
        } else {
            return axios
                .delete(`${baseURL}admin/disciplines/${id}`)
                .then(response => {                    
                    dispatch(deleteDisciplineData(id))
                })
                .catch(error => {
                    throw error;
                })
        }
    } 
}

export const deleteSkill = (disciplineID, skillName) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteSkillData(disciplineID, skillName))
        } else {
            return axios
            .delete(`${baseURL}admin/disciplines/${disciplineID}/skills/${skillName}`)
            .then(response => {            
                dispatch(deleteSkillData(disciplineID, skillName));
            })
            .catch(error => {
                throw error;
            })
        }
    } 
}

export const deleteProvince = (provinceName) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteProvinceData(provinceName))
        } else {
            return axios
                .delete(`${baseURL}admin/provinces/${provinceName}`)
                .then(response => {
                    dispatch(deleteProvinceData(response.data.payload))
                })
                .catch(error => {
                    throw error;
                })
        }
    } 
}

export const deleteCity = (cityName, id) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteCityData(cityName, id))
        } else {
            // TODO - backend only needs id, but keep cityName to make the reducer easier to deal with
            return axios
            .delete(`${baseURL}admin/locations/${id}`)
            .then(response => {
                dispatch(deleteCityData(cityName, id))
            })
            .catch(error => {
                throw error
            })
        }
    } 
}