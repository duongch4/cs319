import * as types from './actionTypes';
import {CLIENT_DEV_ENV, SVC_ROOT} from '../../config/config';
import { headers } from '../../config/adalConfig';
import axios from 'axios';
import _initialState_client from '../reducers/_initialState_client';

const baseURL = `${SVC_ROOT}api/users/search`;

export const performSearch = userSummaries => {
    return {
      type: types.LOAD_USERS_SPECIFIC,
      userSummaries: userSummaries
    };
  };