import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadSpecificUserData = (state, action) => {
  // XXX TODO: This needs to be changed but I am unsure how. Ask Dave
  return state.filter(user => user.userID == action.userID);
};

export const userProfileReducer = (state = initialState.usersProfile, action) => {
  switch (action.type) {
    case types.LOAD_USERS_SPECIFIC:
      return executeLoadSpecificUserData(state, action);
    default:
      return state;
  }
};

export default userProfileReducer;
