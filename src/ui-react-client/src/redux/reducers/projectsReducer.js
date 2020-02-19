import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadProjectsData = (state, action) => {
  return {
    ...state,
    projectSummaries: action.projectSummaries
  }
};

const executeLoadSingleProjectData = (state, action) => {
  return {
    ...state,
    projectProfile: action.projectProfile
  }
};

const executeCreateProjectData = (state, action) => {
  return {
    ...state,
    projectProfiles: [
        ...state.projectProfiles,
      action.projectProfile
    ],
    projectSummaries: [
        ...state.projectSummaries,
        action.projectProfile.projectSummary
    ],
    projectProfile: action.projectProfile
  };
};

const executeUpdateProjectData = (state, action) => {
  let projProfileIndex = state.projectProfiles.indexOf( projProfile => {
      return projProfile.projectSummary.projectNumber == action.projectSummary.projectNumber;
  });

  let projSummaryIndex = state.projectSummaries.indexOf(projSummary => {
    return projSummary.projectNumber == action.projectSummary.projectNumber;
  });

  let newProjectProfiles = JSON.parse(JSON.stringify(state.projectProfiles));
  let newProjectSummaries = JSON.parse(JSON.stringify(state.projectSummaries));

  if (projProfileIndex > -1) {
    newProjectProfiles[projProfileIndex] = action.projectProfile;
  }

  if (projSummaryIndex > -1) {
    newProjectSummaries[projSummaryIndex] = action.projectProfile.projectSummary;
  }

  return {
    ...state,
    projectSummaries: newProjectSummaries,
    projectProfiles: newProjectProfiles,
    projectProfile: action.projectProfile
  };
};

const executeDeleteProjectData = (state, action) => {
  let projSummaryIndex = state.projectSummaries.indexOf(projSummary => {
    return projSummary.projectNumber == action.projectProfile.projectSummary.projectNumber;
  });

  let projProfileIndex = state.projectProfiles.indexOf(projProfile => {
    return projProfile.projectSummary.projectNumber == action.projectProfile.projectSummary.projectNumber;
  });

  let newProjectProfiles = JSON.parse(JSON.stringify(state.projectProfiles));
  let newProjectSummaries = JSON.parse(JSON.stringify(state.projectSummaries));

  if (projSummaryIndex > -1) {
    newProjectSummaries = newProjectSummaries.slice(projSummaryIndex, 1);
  }

  if (projProfileIndex > -1) {
    newProjectProfiles = newProjectProfiles.slice(projProfileIndex, 1);
  }

  return {
    ...state,
    projectSummaries: newProjectSummaries,
    projectProfiles: newProjectProfiles,
    projectProfile: null
  };
};

export const projectsReducer = (
  state = {
    projectSummaries: initialState.projectSummaries,
    projectProfiles: initialState.projectProfiles,
    projectProfile: initialState.projectProfile
  },
  action,
) => {
  switch (action.type) {
    case types.LOAD_PROJECTS_ALL:
      return executeLoadProjectsData(state, action);
    case types.LOAD_SINGLE_PROJECT:
      return executeLoadSingleProjectData(state, action);
    case types.CREATE_PROJECT:
      return executeCreateProjectData(state, action);
    case types.UPDATE_PROJECT:
      return executeUpdateProjectData(state, action);
    case types.DELETE_PROJECT:
      return executeDeleteProjectData(state, action);
    default:
      return state;
  }
};

export default projectsReducer;
