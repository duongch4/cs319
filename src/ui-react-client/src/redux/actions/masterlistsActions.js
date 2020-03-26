import * as types from './actionTypes';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../config/config';
import { getHeaders } from '../../config/authUtils';
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

export const errorCreating = (error) => {
    return {
        type: types.ERROR_CREATING,
        error: error
    }
}

export const errorDeleting = (error) => {
    return {
        type: types.ERROR_DELETING,
        error: error
    }
}

export const loadMasterlists = (userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(loadMasterlistsData(_initialState.masterlist));
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios.get(`${baseURL}masterlists/`, { headers });
            }).then (response => {
                dispatch(loadMasterlistsData(response.data.payload));
            }).catch(error => {
                throw error;
            });
        }
    }
};

export const createDiscpline = (discipline, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createDiscplineData(discipline))
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios
                    .post(`${baseURL}admin/disciplines`, discipline, { headers })
                    .then(response => {
                        discipline.id = response.data.payload;
                        dispatch(createDiscplineData(discipline))
                    })
                    .catch(error => {
                        dispatch(errorCreating(error));
                        // throw error;
                    })
            })
        }
    }
}

export const createSkill = (skill, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createSkillData(skill))
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios
                    .post(`${baseURL}admin/disciplines/${skill.disciplineID}/skills`, skill, { headers })
                    .then(response => {
                        dispatch(createSkillData(skill))
                    })
                    .catch(error => {
                        dispatch(errorCreating(error));
                        // throw error;
                    })
            })
        }
    } 
}

export const createProvince = (location, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createProvinceData(location))
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios
                    .post(`${baseURL}admin/provinces`, location, { headers })
                    .then(response => {
                        dispatch(createProvinceData(location))
                    })
                    .catch(error => {
                        dispatch(errorCreating(error));
                        // throw error;
                    })
            })
        }
    } 
}

export const createCity = (location, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(createCityData(location))
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios
                    .post(`${baseURL}admin/locations`, location, { headers })
                    .then(response => {
                        location.id = response.data.payload;
                        dispatch(createCityData(location))
                    })
                    .catch(error => {
                        dispatch(errorCreating(error));
                        // throw error;
                    })
            })
        }
    } 
}

export const deleteDiscipline = (id, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteDisciplineData(id))
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios
                    .delete(`${baseURL}admin/disciplines/${id}`, { headers })
                    .then(response => {
                        dispatch(deleteDisciplineData(id))
                    })
                    .catch(error => {
                        // throw error;
                        dispatch(errorDeleting(error));
                    })
            })
        }
    }
}

export const deleteSkill = (disciplineID, skillName, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteSkillData(disciplineID, skillName))
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios
                    .delete(`${baseURL}admin/disciplines/${disciplineID}/skills/${skillName}`, { headers })
                    .then(response => {
                        dispatch(deleteSkillData(disciplineID, skillName));
                    })
                    .catch(error => {
                        // throw error;
                        dispatch(errorDeleting(error));
                    })
            })
        }
    }
}

export const deleteProvince = (provinceName, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteProvinceData(provinceName))
        } else {
            return getHeaders(userRoles).then(headers => {
                return axios
                    .delete(`${baseURL}admin/provinces/${provinceName}`, { headers })
                    .then(response => {
                        dispatch(deleteProvinceData(response.data.payload))
                    })
                    .catch(error => {
                        // throw error;
                        dispatch(errorDeleting(error));
                    })
            })
        }
    }
}

export const deleteCity = (cityName, id, userRoles) => {
    return dispatch => {
        if (CLIENT_DEV_ENV) {
            dispatch(deleteCityData(cityName, id))
        } else {
            // TODO - backend only needs id, but keep cityName to make the reducer easier to deal with
            return getHeaders(userRoles).then(headers => {
                return axios
                    .delete(`${baseURL}admin/locations/${id}`, { headers })
                    .then(response => {
                        dispatch(deleteCityData(cityName, id))
                    })
                    .catch(error => {
                        // throw error
                        dispatch(errorDeleting(error));
                    })
            })
        }
    }
}
