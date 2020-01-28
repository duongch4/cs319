import * as types from '../actions/actionTypes';
import initialState from './_initialState';

const executeLoadLocationsAllData = action => {
  return action.locations;
};

export const locationsReducer = (
  state = initialState.locations,
  action,
) => {
  switch (action.type) {
    case types.LOAD_LOCATIONS_ALL:
      return executeLoadLocationsAllData(action);
    default:
      return state;
  }
};

export default locationsReducer;
