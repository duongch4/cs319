import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadSingleProjectData = action => {
    return action.projectProfile;
};

const executeCreateProjectData = (state, action) => {
    return {
        projects: [
            ...state.projects,
            action.projectProfile
        ]
    };
};

const executeUpdateProjectData = (action) => {
    return action.projectProfile;
};

const executeDeleteProjectData = () => {
    // TODO maybe not the correct place for deleting data
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
            return executeCreateProjectData(action);
        case types.UPDATE_PROJECT:
            return executeUpdateProjectData(action);
        case types.DELETE_PROJECT:
            return executeDeleteProjectData();
        default:
            return state;
    }
};

export default projectProfileReducer;
