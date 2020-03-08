import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadProjectsData = (action) => {
  return action.projects;
};

const executeUpdateProjectSummaryArray = (state, action) => {
  return state.map(project => {
    if (project.projectNumber === action.projectSummary.projectNumber) {
      return action.projectSummary;
    } else {
      return project;
    }
  })
};

const executeAddProjectSummary = (state, action) => {
  let projects = state.map(project => project);
  projects.push(action.projectSummary);
  return projects;
};

const executeDeleteProjectSummary = (state, action) => {
  return state.filter(project => project.projectNumber !== action.projectNumber);
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
    case types.ADD_PROJECT_SUMMARY:
      return executeAddProjectSummary(state, action);
    default:
      return state;
  }
};

export default projectsReducer;
