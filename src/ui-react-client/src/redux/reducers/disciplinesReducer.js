import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadDisciplines = (state, action) => {
  return action.disciplines;
}

export const disciplineReducer = (
  state = initialState.disciplines,
  action,
) => {
  switch (action.type) {
    case types.LOAD_DISCIPLINES:
      return executeLoadDisciplines(state, action);
    default:
      return state;
  }
};

export default disciplineReducer;
