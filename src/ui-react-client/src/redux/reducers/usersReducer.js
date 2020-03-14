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

export const usersReducer = (
    state = initialState.users,
    action
) => {
  switch (action.type) {
    case types.LOAD_USERS_ALL:
      return executeLoadUsersAllData(action);
    case types.UPDATE_USER_SUMMARIES:
      return executeUpdateUserSummaryData(state, action);
    default:
      return state;
  }
};

export default usersReducer;
