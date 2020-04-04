import * as types from '../actions/actionTypes';
import initialState from './_initialState';

export const executeLoadMasterlistsData = action => {
    return action.masterlist;
};

export const executeCreateDiscipline = (action, state) => {
    let newDisciplines = state.disciplines
    newDisciplines[action.disciplines.name] = {
        disciplineID: action.disciplines.id,
        skills: []
    }
    let newState = {
        ...state,
        error: null,
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
        error: null,
        disciplines: newDisciplines
    }
    return newState
}

export const executeCreateProvince = (action, state) => {
    let newLocation = state.locations
    newLocation[action.location.province] = {}
    let newState = {
        ...state,
        error: null,
        locations: newLocation
    }
    return newState
}

export const executeCreateCity = (action, state) => {
    let newLocation = state.locations
    let newCity = state.locations[action.location.province]
    newCity[action.location.city] = action.location.id
    for(var province in newLocation){
        if(province === action.location.province){
            newLocation[action.location.province] = newCity
        }
    }
    let newState = {
        ...state,
        error: null,
        locations: newLocation
    }
    return newState
}

export const executeDeleteDiscipline = (action, state) => {
 
    const newDisciplines = Object.keys(state.disciplines).reduce((object, key) => {
        if(state.disciplines[key].disciplineID !== action.id){
            object[key] = state.disciplines[key]
        }
        return object
      }, {})
    let newState = {
        ...state,
        error: null,
        disciplines: newDisciplines
    }
    return newState;
}

export const executeDeleteSkill = (action, state) => {
    const newDisciplines = Object.keys(state.disciplines).reduce((object, key) => {
        if(state.disciplines[key].disciplineID === action.disciplineID){
            var newSkills = state.disciplines[key].skills.filter(elem => {
                return elem !== action.skillName}
            )
            object[key] = {
                disciplineID: state.disciplines[key].disciplineID,
                skills: newSkills
            }

        } else {
            object[key] = state.disciplines[key]
        }
        return object
      }, {})
    let newState = {
        ...state,
        error: null,
        disciplines: newDisciplines
    }
    return newState;
}

export const executeDeleteProvince = (action, state) => {
    const newLocations = Object.keys(state.locations).reduce((object, key) => {
        if(key !== action.provinceName){
            object[key] = state.locations[key]
        }
        return object
      }, {})
    let newState = {
        ...state,
        error: null,
        locations: newLocations
    }
    return newState;
}

export const executeDeleteCity = (action, state) => {
    const newLocations = Object.keys(state.locations).reduce((object, key) => {
        Object.keys(state.locations[key]).forEach(item => {
            if(state.locations[key][item] !== action.id){
                let newObj = object[key] ? object[key] : {};
                newObj[item] = state.locations[key][item];
                object[key] = newObj;
            } else {
                if(object[key]){
                    // do nothing
                } else {
                    object[key] = {}
                }
            }
        })
        return object
      }, {})
      let newState = {
          ...state,
          error: null,
          locations: newLocations
      }
    return newState;
}

export const executeDeleteError = (action, state) => {
    let newState = {
        ...state,
        error: action.error
    }
    return newState;
}

export const executeCreateError = (action, state) => {
    let newState = {
        ...state,
        error: action.error
    }
    return newState;
}

export const executeClearError = (state) => {
    let newState = {
        ...state,
        error: null
    }
    return newState;
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
        case types.DELETE_DISCIPLINE:
            return executeDeleteDiscipline(action, state);
        case types.DELETE_SKILL:
            return executeDeleteSkill(action, state);
        case types.DELETE_PROVINCE:
            return executeDeleteProvince(action, state);
        case types.DELETE_CITY:
            return executeDeleteCity(action, state);
        case types.ERROR_DELETING:
            return executeDeleteError(action, state);
        case types.ERROR_CREATING:
            return executeCreateError(action, state);
        case types.CLEAR_ERROR:
            return executeClearError(state);
        default:
            return state;
    }
};

export default masterlistsReducer;
