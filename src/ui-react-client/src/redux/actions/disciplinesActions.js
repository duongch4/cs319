import * as types from './actionTypes';
import { SVC_ROOT } from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState from '../reducers/_initialState';

const baseURL = `${SVC_ROOT}projects/`;

export const loadDisciplinesData = disciplines => {
  return {
    type: types.LOAD_DISCIPLINES,
    disciplines: disciplines
  }
}

export const loadDisciplines = () => {
  return dispatch => {
    dispatch(loadDisciplinesData(_initialState.disciplines));
    // Ready for DB call
  }
}
