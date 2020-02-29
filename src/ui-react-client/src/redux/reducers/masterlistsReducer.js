import * as types from '../actions/actionTypes';
import initialState from './_initialState';

export const executeLoadMasterlistsData = action => {
    return action.masterlist;
};

export const executeCreateDiscipline = (action, state) => {
    let newDisciplines = state.disciplines
    newDisciplines[action.disciplines.name] = []
    let newState = {
        ...state,
        disciplines: newDisciplines
    }
    return newState
}

export const executeCreateSkill = (action, state) => {
    let newDisciplines = state.disciplines
    newDisciplines[action.skill.disciplineID] = [...state.disciplines[action.skill.disciplineID], action.skill.name]
    let newState = {
        ...state,
        disciplines: newDisciplines
    }
    return newState
}

export const executeCreateProvince = (action, state) => {
    let newLocation = state.locations
    newLocation[action.location.province] = []
    let newState = {
        ...state,
        locations: newLocation
    }
    return newState
}

export const executeCreateCity = (action, state) => {
    let newLocation = state.locations
    newLocation[action.location.province] = [...state.locations[action.location.province], action.location.city]
    let newState = {
        ...state,
        locations: newLocation
    }
    return newState
}

export const masterlistsReducer = (
    state = initialState.masterlist,
    action
) => {
    switch(action.type) {
        case types.LOAD_MASTERLIST:
            return executeLoadMasterlistsData(action);
        case types.CREATE_DISCIPLINE:
            return executeCreateDiscipline(action, state);
        case types.CREATE_SKILL:
            return executeCreateSkill(action, state);
        case types.CREATE_PROVINCE:
            return executeCreateProvince(action, state);
        case types.CREATE_CITY:
            return executeCreateCity(action,state);
        default:
            return state;
    }
};

export default masterlistsReducer;
