import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeSearch = (state, action) => {
    return {
        userProfiles: [
            ...state.userProfiles,
            action.userProfiles
        ]
    };
};

export const projectProfileReducer = (
    state = initialState.userProfiles,
    action
) => {
    switch (action.type) {
        case types.PERFORM_USER_SEARCH:
            return executeSearch(action);
        default:
            return state;
    }
};

export default searchReducer;