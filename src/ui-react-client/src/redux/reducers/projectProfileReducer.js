import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadSingleProjectData = action => {
    return action.projectProfile;
};

const executeCreateProjectData = (state, action) => {
    return action.projectProfile;
};

const executeUpdateProjectData = (action) => {
    return action.projectProfile;
};

const executeDeleteProjectData = () => {
    return {};
};


const executeCreateAssignOpening = (state, action) => {
  let newOpenings = state.openings.filter(opening => opening.positionID !== action.openingID);
  let newState = {
  ...state,
  openings: newOpenings,
  usersSummary: [...state.usersSummary, action.user]
  };
  return newState;
};

const executeConfirmAssignOpening = (state, action) => {
  const usersSummaryCopy = state.usersSummary.slice();
  const disciplineName = action.userSummaryDisciplineName;

  usersSummaryCopy.forEach(summary => {
    if (summary.userID === action.userId
    && summary.resourceDiscipline.discipline === disciplineName){
          summary.utilization = action.confirmedUtilization;
          summary.isConfirmed = true;
    }

  });
  let newState = {
    ...state,
    usersSummary: usersSummaryCopy
  };
  return newState;
};

const executeUnassignOpening = (state, action) => {
  const usersSummaryCopy = state.usersSummary.slice();
  const disciplineName = action.userSummaryDisciplineName;
  let newUsersSummaryCopy = usersSummaryCopy.filter(userSummary => userSummary.userID !== action.userId
  || userSummary.resourceDiscipline.discipline !== action.userSummaryDisciplineName);
  //TODO once backend done updateopenings to have previosuly assigned opening back in openings array
  let newState = {
    ...state,
    usersSummary: newUsersSummaryCopy
  };
  return newState;
};


export const projectProfileReducer = (
    state = initialState.projectProfile,
    action
) => {
    switch (action.type) {
        case types.LOAD_SINGLE_PROJECT:
            return executeLoadSingleProjectData(action);
        case types.CREATE_PROJECT:
            return executeCreateProjectData(state, action);
        case types.UPDATE_PROJECT:
            return executeUpdateProjectData(action);
        case types.DELETE_PROJECT:
            return executeDeleteProjectData();
        case types.UPDATE_ASSIGN_OPENING:
          return executeCreateAssignOpening(state, action, initialState);
        case types.CONFIRM_ASSIGN_OPENING:
          return executeConfirmAssignOpening(state, action);
        case types.UNASSIGN_OPENING:
          return executeUnassignOpening(state, action);
        default:
            return state;
    }
};

export default projectProfileReducer;
