import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadSpecificUserData = (action) => {
  return action.userProfile;
};

const executeUpdateSpecificUserData = (action) => {
  return action.userProfile;
};

const executeUpdateUnassignUserData = (state, action) => {
  if (action.userID === state.userSummary.userID) {
    // remove the project summary from the user's current projects
    let projectIndex = state.currentProjects.findIndex(project => {
      return project.projectNumber === action.projectNumber
    });
    let updatedProjects = state.currentProjects.splice(projectIndex, 1);

    // remove the position from the user's list of positions
    let updatedPositions = state.positions.filter(position => position.positionID !== action.openingID);

    // update the user's utilization, position and current projects
    return {
      ...state,
      userSummary: {
        ...state.userSummary,
        utilization: action.confirmedUtilization
      },
      currentProjects: updatedProjects,
      positions: updatedPositions
    }
  } else {
    return state;
  }
};

const executeUpdateAssignUserData = (state, action) => {
  if (state.userSummary.userID === action.userID) {
    // create a new position
    let newPosition = {
      positionID: action.opening.positionID,
      positionName:action.opening.discipline,
      projectTitle: action.projectSummary.title,
      disciplineName: action.opening.discipline,
      projectedMonthlyHours: action.opening.commitmentMonthlyHours
    };
    // update the user's utilization, project list, and position list
    return {
      ...state,
      userSummary: {
        ...state.userSummary,
        utilization: action.confirmedUtilization
      },
      currentProjects: [
          ...state.currentProjects,
          action.projectSummary
      ],
      positions: [
          ...state.positions,
          newPosition
      ]
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
      return executeUpdateUnassignUserData(state, action);
    case types.ASSIGN_UPDATE_USER_DATA:
      return executeUpdateAssignUserData(state, action);
    default:
      return state;
  }
};

export default userProfileReducer;
