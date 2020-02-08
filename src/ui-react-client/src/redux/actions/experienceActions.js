import * as types from './actionTypes';
import { SVC_ROOT } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}projects/`;

export const loadExperienceData = masterYearsOfExperience => {
  return {
    type: types.LOAD_EXPERIENCES,
    masterYearsOfExperience: masterYearsOfExperience
  }
}

export const loadExperiences = () => {
  return dispatch => {
    dispatch(loadExperienceData(_initialState.masterYearsOfExperience));
  }
}

