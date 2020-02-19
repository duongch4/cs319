import * as types from '../actions/actionTypes';
import initialState from './_initialState';

export const executeLoadMasterlistsData = action => {
    console.log(action);
    return action.masterlist;
};

export const masterlistsReducer = (
    state = initialState.masterlist,
    action
) => {
    switch(action.type) {
        case types.LOAD_MASTERLIST:
            return executeLoadMasterlistsData(action);
        default:
            return state;
    }
};

export default masterlistsReducer;
