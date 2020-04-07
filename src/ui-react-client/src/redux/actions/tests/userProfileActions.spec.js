import * as userProfileActions from '../userProfileActions';
import * as types from '../actionTypes';
import { SVC_ROOT } from '../../../config/config';
import * as authUtils from '../../../config/authUtils';
import configureStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import axios from 'axios';
import { createMemoryHistory } from 'history'

const adminRole = ["adminUser", "regularUser"];
const baseURL = `${SVC_ROOT}api/users/`;

jest.mock('axios');
jest.mock('../../../config/authUtils');
const middlewares = [thunk];
const mockStore = configureStore(middlewares);
const store = mockStore({});
const userProfile = {
    userSummary: {
        userID: 100,
        firstName: "Jason",
        lastName: "Bourne",
        location: {
            locationID: 2,
            province: "British Columbia",
            city: "Vancouver"
        },
        utilization: 100,
        resourceDiscipline: {
            disciplineID: 123,
            discipline: "Logistical Operations and Mental Health Analysis",
            yearsOfExp: "3-5"
        },
        isConfirmed: false
    },
    currentProjects: [
        {
            title: "Martensville Athletic Pavillion",
            location: {
                locationID: 1,
                province: "Seskatchewan",
                city: "Saskatoon"
            },
            projectStartDate: "2020-10-31T00:00:00.0000000",
            projectEndDate: "2021-12-31T00:00:00.0000000",
            projectNumber: "2009-VD9D-15"
        }
    ],
    availability: [
        {
            fromDate: "2020-10-31T00:00:00",
            toDate: "2020-11-11T00:00:00",
            reason: "Paternal Leave"
        }
    ],
    disciplines: [
        {
            disciplineID: 105,
            name: "Intel",
            yearsOfExp: "10+",
            skills: [
                "Deception"
            ]
        },
        {
            disciplineID: 106,
            name: "Language",
            yearsOfExp: "10+",
            skills: [
                "Russian"
            ]
        }
    ],
    positions: [
        {
            positionID: 1,
            positionName: "Spy",
            projectTitle: "Martensville Athletic Pavillion",
            disciplineName: "Intel",
            projectedMonthlyHours: {
                "2020-01-01": 30,
                "2020-02-01": 50,
                "2020-03-01": 50
             }
        }
    ]
};

const updatedUserProfile = {
    userSummary: {
        userID: 100,
        firstName: "Jason",
        lastName: "Bourn",
        location: {
            locationID: 2,
            province: "British Columbi",
            city: "Vancouver"
        },
        utilization: 100,
        resourceDiscipline: {
            disciplineID: 124,
            discipline: "Logistical Operations and Mental Health Analysis",
            yearsOfExp: "3-5"
        },
        isConfirmed: false
    },
    currentProjects: [
        {
            title: "Martensville Athletic Pavillion",
            location: {
                locationID: 1,
                province: "Seskatchewan",
                city: "Saskatoon"
            },
            projectStartDate: "2020-10-31T00:00:00.0000000",
            projectEndDate: "2021-12-31T00:00:00.0000000",
            projectNumber: "2009-VD9D-15"
        }
    ],
    availability: [
        {
            fromDate: "2020-10-31T00:00:00",
            toDate: "2020-11-11T00:00:00",
            reason: "Paternal Leave"
        }
    ],
    disciplines: [
        {
            disciplineID: 105,
            name: "Intel",
            yearsOfExp: "10+",
            skills: [
                "Deception"
            ]
        },
        {
            disciplineID: 106,
            name: "Language",
            yearsOfExp: "10+",
            skills: [
                "Russian"
            ]
        }
    ],
    positions: [
        {
            positionID: 1,
            positionName: "Spy",
            projectTitle: "Martensville Athletic Pavillion",
            disciplineName: "Intel",
            projectedMonthlyHours: {
                "2020-01-01": 30,
                "2020-02-01": 50,
                "2020-03-01": 50
             }
        }
    ]
};

describe('loadSpecificUsers', () => {
  afterEach(() => {
    store.clearActions();
    axios.get.mockClear();
    axios.put.mockClear();
    authUtils.getHeaders.mockClear();
    })

  it('loadSpecificUser: should successfully load user profile', async () => {

    const expectedAction = [{
        type: types.LOAD_USERS_SPECIFIC,
        userProfile: userProfile
    }];
    const resp = {data: {payload: userProfile}};
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

    axios.get.mockResolvedValue(resp);

    await store.dispatch(userProfileActions.loadSpecificUser("100", adminRole));

    expect(store.getActions()).toEqual(expectedAction);
    expect(axios.get).toHaveBeenCalledTimes(1);
    expect(axios.get).toHaveBeenCalledWith(`${baseURL}100`, {headers:{ Authorization: `Bearer 100` }});
  })

  it('loadSpecificUser: should handle error when load users action is rejected', async () => {
    let errorMessage = "Error message";
    axios.get.mockImplementationOnce(() =>
      Promise.reject(new Error(errorMessage)),
    );
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
    let alert = jest.spyOn(window, 'alert').mockImplementation(() => {});

    await expect(store.dispatch(userProfileActions.loadSpecificUser("100", adminRole)));
    expect(alert).toHaveBeenCalledTimes(0);
    expect(axios.get).toHaveBeenCalledWith(`${baseURL}100`, {headers:{ Authorization: `Bearer 100` }});
  });

// updateSpecificUser
it('updateSpecificUser: should successfully update user profile', async () => {
    const history = createMemoryHistory('/users')
    const expectedAction = [{
        type: types.UPDATE_USERS_SPECIFIC,
        userProfile: updatedUserProfile
      },{
        type: types.UPDATE_USER_SUMMARIES,
        userSummary: updatedUserProfile.userSummary
    }];
    const resp = {data: {payload: updatedUserProfile}};
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

    axios.put.mockResolvedValue(resp);

    await store.dispatch(userProfileActions.updateSpecificUser(updatedUserProfile, history, adminRole));

    expect(store.getActions()).toEqual(expectedAction);
    expect(axios.put).toHaveBeenCalledTimes(1);
    expect(axios.put).toHaveBeenCalledWith(`${baseURL}100`, updatedUserProfile, {headers:{ Authorization: `Bearer 100` }});
  })

  it('updateSpecificUser: should handle error when update users action is rejected', async () => {
    let errorMessage = "Error message";
    axios.get.mockImplementationOnce(() =>
      Promise.reject(new Error(errorMessage)),
    );
    const history = createMemoryHistory('/users')
    const expectedAction = [{
        type: types.UPDATE_USERS_SPECIFIC,
        userProfile: updatedUserProfile
      },{
        type: types.UPDATE_USER_SUMMARIES,
        userSummary: updatedUserProfile.userSummary
    }];
    const resp = {data: {payload: updatedUserProfile}};
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

    axios.put.mockResolvedValue(resp);

    await store.dispatch(userProfileActions.updateSpecificUser(updatedUserProfile, history, adminRole));
    let alert = jest.spyOn(window, 'alert').mockImplementation(() => {});

    expect(store.getActions()).toEqual(expectedAction);
    expect(axios.put).toHaveBeenCalledTimes(1);
    expect(axios.put).toHaveBeenCalledWith(`${baseURL}100`, updatedUserProfile, {headers:{ Authorization: `Bearer 100` }}); });
});