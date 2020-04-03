import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadUsersAllData = action => {
  return action.users;
};

const executeUpdateUserSummaryData = (state, action) => {
  return state.map(userSummary => {
    if (userSummary.userID === action.userSummary.userID) {
      return action.userSummary;
    } else {
      return userSummary;
    }
  })
};

const executeSearch = (action, state) => {
  return action.users;
  /* TODO: Need to have cases for:
    matching location - city and/or province
    matching Name - first and/or last
    matchings Discipline(s)
    matchings Skills(s)
    matching Years of Experience
    Any combintation of the above
*/ 
};

const executeClearSearchResultsData = () => {
  return []
}

export const usersReducer = (
    state = initialState.users,
    action
) => {
  switch (action.type) {
    case types.LOAD_USERS_ALL:
      return executeLoadUsersAllData(action);
    case types.UPDATE_USER_SUMMARIES:
      return executeUpdateUserSummaryData(state, action);
    case types.PERFORM_USER_SEARCH:
      return executeSearch(action, state);
    case types.CLEAR_SEARCH_RESULTS:
      return executeClearSearchResultsData();
    default:
      return state;
  }
};

export default usersReducer;
