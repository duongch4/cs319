import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeSearch = (action) => {
    return action.userSummary;
};

export const searchReducer = (
    state = initialState.users,
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