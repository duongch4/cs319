import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadProjectsData = (action) => {
  return action.projects;
};

const executeUpdateProjectSummaryArray = (state, action) => {
  console.log(state);
  return state.projects.map(project => {
    if (project.projectNumber === action.projectSummary.projectNumber) {
      return action.projectSummary;
    } else {
      return project;
    }
  })
};

const executeCreateProjectSummary = (state, action) => {
  return [
      ...state.projects,
      action.projectSummary
  ]
};

const executeDeleteProjectSummary = (state, action) => {
  let newProjects = state.projects.slice();
  let index = newProjects.indexOf(project => project.projectNumber === action.projectSummary.projectNumber);
  if (index > -1) {
    newProjects.splice(index, 1);
  }
  return newProjects;
};

export const projectsReducer = (
  state = initialState.projects,
  action,
) => {
  switch (action.type) {
    case types.LOAD_PROJECTS_ALL:
      return executeLoadProjectsData(action);
    case types.UPDATE_PROJECT_SUMMARY:
      return executeUpdateProjectSummaryArray(state, action);
    case types.DELETE_PROJECT_SUMMARY:
      return executeDeleteProjectSummary(state, action);
    case types.CREATE_PROJECT_SUMMARY:
      return executeCreateProjectSummary(state, action);
    default:
      return state;
  }
};

export default projectsReducer;
