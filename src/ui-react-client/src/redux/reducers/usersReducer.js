import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadUsersAllData = (state, action) => {
  return {
    ...state,
    userSummaries: action.userSummaries
  };
};

const executeLoadSpecificUserData = (state, action) => {
  return {
    ...state,
    userProfile: action.userProfile
  };
};

const executeUpdateSpecificUserData = (state, action) => {
  let profile = action.userProfile;
  let indexOfUserSummary = state.userSummaries.indexOf(userSummary => {
    return userSummary.userID === profile.userSummary.userID;
  });
  let newUserSummaries = state.userSummaries;
  if (indexOfUserSummary >= 0) {
    newUserSummaries = JSON.parse(JSON.stringify(state.userSummaries));
    newUserSummaries[indexOfUserSummary] = profile.userSummary;
  }
  return {
    ...state,
    userSummaries: newUserSummaries,
    userProfile: profile
  }
};

export const usersReducer = (
    state = {
      userSummaries: initialState.userSummaries,
      userProfiles: initialState.userProfiles,
      userProfile: initialState.userProfile
    },
    action
) => {
  switch (action.type) {
    case types.LOAD_USERS_ALL:
      return executeLoadUsersAllData(state, action);
    case types.LOAD_USERS_SPECIFIC:
      return executeLoadSpecificUserData(state, action);
    case types.UPDATE_USERS_SPECIFIC:
      return executeUpdateSpecificUserData(state, action);
    default:
      return state;
  }
};

export default usersReducer;
