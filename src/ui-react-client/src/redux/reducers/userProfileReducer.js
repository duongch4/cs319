import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadSpecificUserData = (state, action) => {
  // XXX TODO: This needs to be changed but I am unsure how. Ask Dave
  return state.filter(user => user.userID == action.userID);
};

const executeUpdateSpecificUserData = (state, action) => {
  return state.map(user => user.userID === action.user.userID ? action.user : user);
};

export const userProfileReducer = (state = initialState.usersProfile, action) => {
  switch (action.type) {
    case types.LOAD_USERS_SPECIFIC:
      return executeLoadSpecificUserData(state, action);
    case types.UPDATE_USERS_SPECIFIC:
      return executeUpdateSpecificUserData(state, action);
    default:
      return state;
  }
};

export default userProfileReducer;
