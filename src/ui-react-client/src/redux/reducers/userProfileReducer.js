import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadSpecificUserData = (action) => {
  return action.userProfile;
};

const executeUpdateSpecificUserData = (state, action) => {
  let profile = action.userProfile;
  let indexOfUserSummary = state.users.indexOf(userSummary => {
    return userSummary.userID === profile.userSummary.userID;
  });
  let newUserSummaries = [...state.users];
  if (indexOfUserSummary >= 0) {
    newUserSummaries = [...state.users];
    newUserSummaries[indexOfUserSummary] = profile.userSummary;
  }
  return  profile;
};

export const userProfileReducer = (state = initialState.userProfile, action) => {
  switch (action.type) {
    case types.LOAD_USERS_SPECIFIC:
      return executeLoadSpecificUserData(action);
    case types.UPDATE_USERS_SPECIFIC:
      return executeUpdateSpecificUserData(state, action);
    default:
      return state;
  }
};

export default userProfileReducer;
