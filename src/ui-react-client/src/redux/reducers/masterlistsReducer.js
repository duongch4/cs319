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
    for(var discipline in newDisciplines){
        if(newDisciplines[discipline].disciplineID === action.skill.disciplineID){
            newDisciplines[discipline].skills = state.disciplines[discipline].skills 
                ? [...state.disciplines[discipline].skills, action.skill.name]
                : [action.skill.name]
        }
    }
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
    let newCity = state.locations[action.location.province]
    newCity[action.location.city] = 0
    for(var province in newLocation){
        if(province === action.location.province){
            newLocation[action.location.province] = newCity
        }
    }
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
