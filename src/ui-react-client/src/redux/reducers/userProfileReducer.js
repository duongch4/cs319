import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadSpecificUserData = (action) => {
  return action.userProfile;
};

const executeUpdateSpecificUserData = (action) => {
  return action.userProfile;
};

const executeUpdateUnassignUserData = (state, action) => {
  if (action.userId == state.userSummary.userID){
        let openings = state.positions.filter(openings => openings.positionID !== action.openingId);
        let openingsInProject = openings.filter(openings => openings.projectTitle == action.projectTitle);
        if (openingsInProject){
          let newState = {
              ...state,
              positions: openings,
              userSummary: {
                ...state.userSummary,
                utilization: action.confirmedUtilization
              }
          }
          return newState;
        }
        else {
          let updatedCurrentProjects = state.currentProjects.filter(currentProject => currentProject.title !== action.projectTitle);
          let newState = {
              ...state,
              positions: openings,
              userSummary: {
                ...state.userSummary,
                utilization: action.confirmedUtilization
              },
              currentProjects: updatedCurrentProjects
          }
          return newState;
        }
  }

  return state;
};

const executeUpdateAssignUserData = (state, action) => {
//TODO add logic here

  return state;
};

export const userProfileReducer = (state = initialState.userProfile, action) => {
  switch (action.type) {
    case types.LOAD_USERS_SPECIFIC:
      return executeLoadSpecificUserData(action);
    case types.UPDATE_USERS_SPECIFIC:
      return executeUpdateSpecificUserData(action);
    case types.UNASSIGN_UPDATE_USER_DATA:
      return executeUpdateUnassignUserData(state, action);
    case types.ASSIGN_UPDATE_USER_DATA:
      return executeUpdateAssignUserData(state, action);
    default:
      return state;
  }
};

export default userProfileReducer;
