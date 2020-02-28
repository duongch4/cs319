import * as types from './actionTypes';
// import { SVC_ROOT } from '../../config/config';
// import { headers } from '../../config/adalConfig';
// import axios from 'axios';''
import _initialState from '../reducers/_initialState';

// const baseURL = `${SVC_ROOT}locations/`;

export const loadLocationsAllData = locations => {
  return { type: types.LOAD_LOCATIONS_ALL, locations: locations };
};

export const loadLocations = () => {
  return dispatch => {
    dispatch(loadLocationsAllData(_initialState.locations));
    // return axios
    //   .get(baseURL, { headers })
    //   .then(response => {
        // dispatch(loadLocationsAllData(response.data));
      // })
      // .catch(error => {
      //   throw error;
      // });
  };
};
