import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadProjectsData = action => {
  return action.projects;
};

const executeLoadProjectsMostRecentData = action => {
  return action.projects;
};

const executeCreateProjectData = (state, action) => {
  return [...state, { ...action.project }];
};

const executeUpdateProjectData = (state, action) => {
  return state.map(project =>
    project.id === action.project.id ? action.project : project,
  );
};

const executeDeleteProjectData = (state, action) => {
  return state.filter(project => project.id !== action.project.id);
};

export const projectsReducer = (
  state = initialState.projects,
  action,
) => {
  switch (action.type) {
    case types.LOAD_PROJECTS_ALL:
      return executeLoadProjectsData(action);
    case types.LOAD_PROJECTS_MOST_RECENT:
      return executeLoadProjectsMostRecentData(action);
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
