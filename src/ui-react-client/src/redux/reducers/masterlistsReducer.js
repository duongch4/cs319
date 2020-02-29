import * as types from '../actions/actionTypes';
import initialState from './_initialState';

export const executeLoadMasterlistsData = action => {
    return action.masterlist;
};

export const executeCreateDiscipline = (action, state) => {
    let newDisciplines = state.disciplines
    newDisciplines[action.disciplines] = []
    let newState = {
        ...state,
        disciplines: newDisciplines
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
        default:
            return state;
    }
};

export default masterlistsReducer;
