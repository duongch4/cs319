import * as usersActions from '../usersActions';
import * as types from '../actionTypes';
import { SVC_ROOT } from '../../../config/config';
import * as authUtils from '../../../config/authUtils';
import configureStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import axios from 'axios';

const adminRole = ["adminUser", "regularUser"];
const baseURL = `${SVC_ROOT}api/users/`;

jest.mock('axios');
jest.mock('../../../config/authUtils');
const middlewares = [thunk];
const mockStore = configureStore(middlewares);
const store = mockStore({});
const userSummaries = [
  {
      firstName: "Jason",
      lastName: "Bourne",
      userID: 100,
      location: {
          locationID: 2,
          province: "British Columbia",
          city: "Vancouver"
      },
      utilization: 100,
      resourceDiscipline: {
          disciplineID: 456,
          discipline: "Intel",
          yearsOfExp: "3-5"
      },
      isConfirmed: true
  }, {
    firstName: "Nicky",
    lastName: "Parsons",
    userID: 101,
    location: {
        locationID: 2,
        province: "British Columbia",
        city: "Vancouver"
    },
    utilization: 75,
    resourceDiscipline: {
        disciplineID: 123,
        discipline: "Logistical Operations and Mental Health Analysis",
        yearsOfExp: "3-5"
    },
    isConfirmed: true
}];

const extra = {
  isLastPage: false
};

describe('loadUsers', () => {
  afterEach(() => {
    store.clearActions();
    axios.get.mockClear();
    authUtils.getHeaders.mockClear();
})

  it('should successfully load users action', async () => {

    const expectedAction = [{
      type: types.LOAD_USERS_ALL,
      users: userSummaries,
      isLastPage: extra.isLastPage,
    }];
    const resp = {data: {payload: userSummaries, extra: extra}};
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

    axios.get.mockResolvedValue(resp);

    await store.dispatch(usersActions.loadUsers("", adminRole));

    expect(store.getActions()).toEqual(expectedAction);
    expect(axios.get).toHaveBeenCalledTimes(1);
    expect(axios.get).toHaveBeenCalledWith(`${baseURL}`, {headers:{ Authorization: `Bearer 100` }});
  });

  it('should handle error when load users action is rejected', async () => {
    let errorMessage = "Error message";
    axios.get.mockImplementationOnce(() =>
      Promise.reject(new Error(errorMessage)),
    );
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
    let alert = jest.spyOn(window, 'alert').mockImplementation(() => {});

    await expect(store.dispatch(usersActions.loadUsers("", adminRole))).rejects.toThrow(errorMessage);
    expect(alert).toHaveBeenCalledTimes(1);
    expect(axios.get).toHaveBeenCalledWith(`${baseURL}`, {headers:{ Authorization: `Bearer 100` }});
  });

  it('should successfully load users action with search filters', async () => {

    const expectedAction = [{
      type: types.LOAD_USERS_ALL,
      users: userSummaries,
      isLastPage: extra.isLastPage,
    }];
    const resp = {data: {payload: userSummaries, extra: extra}};
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

    axios.get.mockResolvedValue(resp);

    await store.dispatch(usersActions.loadUsers("?&orderKey=utilization&order=desc&page=1", adminRole));

    expect(store.getActions()).toEqual(expectedAction);
    expect(axios.get).toHaveBeenCalledTimes(1);
    expect(axios.get).toHaveBeenCalledWith(`${baseURL}?&orderKey=utilization&order=desc&page=1`, {headers:{ Authorization: `Bearer 100` }});
  });
});
