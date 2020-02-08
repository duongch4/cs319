import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadExperiences = (state, action) => {
  return action.masterYearsOfExperience;
}

export const experienceReducer = (
  state = initialState.masterYearsOfExperience,
  action,
) => {
  switch (action.type) {
    case types.LOAD_EXPERIENCES:
      return executeLoadExperiences(state, action);
    default:
      return state;
  }
};

export default experienceReducer;
