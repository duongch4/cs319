import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadSpecificUserData = (action) => {
  return action.userProfile;
};

const executeUpdateSpecificUserData = (action) => {
  return action.userProfile;
};

const executeUpdateUnassigncUserData = (state, action) => {
  console.log("in unassign reducer action", action)
  console.log("in unassign reducer state", state)
  if (action.userId == state.userSummary.userID){
      console.log("IF STATEMENT TRUE")
        let openings = state.positions.filter(openings => openings.positionID !== action.openingId);
        let openingsInProject = openings.filter(openings => openings.projectTitle == action.projectTitle);
        if (openingsInProject){
          console.log("person assigned to two positions in same project")
          let newState = {
              ...state,
              positions: openings,
              userSummary: {
                ...state.userSummary,
                utilization: action.confirmedUtilization
              }
          }
          console.log("new state", newState)
          return newState;
        }
        else {
          console.log("person NOT assigned to two positions in same project")
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
          console.log("new state", newState)
          return newState;

        }
  }

  return state;
};

export const userProfileReducer = (state = initialState.userProfile, action) => {
  switch (action.type) {
    case types.LOAD_USERS_SPECIFIC:
      return executeLoadSpecificUserData(action);
    case types.UPDATE_USERS_SPECIFIC:
      return executeUpdateSpecificUserData(action);
    case types.UNASSIGN_UPDATE_USER_DATA:
      return executeUpdateUnassigncUserData(state, action);
    default:
      return state;
  }
};

export default userProfileReducer;
