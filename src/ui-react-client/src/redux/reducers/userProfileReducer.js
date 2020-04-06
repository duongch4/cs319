import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadSpecificUserData = (action) => {
  return action.userProfile;
};

const executeUpdateSpecificUserData = (action) => {
  return action.userProfile;
};

const executeUpdateUserProjectsDeletionData = (state, action) => {
  let updatedProjects = state.currentProjects.filter(project => project.projectNumber !== action.projectNumber)
  return {
    ...state,
    currentProjects: updatedProjects
  }
};

const executeUpdateUserProjectsCreationData = (state, action) => {
  if (action.userID === state.userSummary.userID) {
    return {
      ...state,
      currentProjects: [
          ...state.currentProjects,
          action.projectSummary
      ]
    }
  } else {
    return state
  }
};

export const userProfileReducer = (state = initialState.userProfile, action) => {
  switch (action.type) {
    case types.LOAD_USERS_SPECIFIC:
      return executeLoadSpecificUserData(action);
    case types.UPDATE_USERS_SPECIFIC:
      return executeUpdateSpecificUserData(action);
    case types.UPDATE_USER_PROJECTS_DELETION:
      return executeUpdateUserProjectsDeletionData(state, action);
    case types.UPDATE_USER_PROJECTS_CREATION:
      return executeUpdateUserProjectsCreationData(state, action);
    default:
      return state;
  }
};

export default userProfileReducer;
