import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadSingleProjectData = action => {
    return action.projectProfile;
};

const executeCreateProjectData = (state, action) => {
    return action.projectProfile;
};

const executeUpdateProjectData = (action) => {
    return action.projectProfile;
};

const executeDeleteProjectData = () => {
    return {};
};

export const projectProfileReducer = (
    state = initialState.projectProfile,
    action
) => {
    switch (action.type) {
        case types.LOAD_SINGLE_PROJECT:
            return executeLoadSingleProjectData(action);
        case types.CREATE_PROJECT:
            return executeCreateProjectData(state, action);
        case types.UPDATE_PROJECT:
            return executeUpdateProjectData(action);
        case types.DELETE_PROJECT:
            return executeDeleteProjectData();
        default:
            return state;
    }
};

export default projectProfileReducer;
