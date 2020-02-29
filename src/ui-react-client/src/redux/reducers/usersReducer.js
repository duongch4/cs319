import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadUsersAllData = action => {
  return action.users;
};

export const usersReducer = (
    state = initialState.userSummaries,
    action
) => {
  switch (action.type) {
    case types.LOAD_USERS_ALL:
      return executeLoadUsersAllData(action);
    default:
      return state;
  }
};

export default usersReducer;
