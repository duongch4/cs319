import * as searchActions from '../searchActions';
import * as types from '../actionTypes';
import axios from 'axios';
import * as authUtils from '../../../config/authUtils';
import configureStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import { SVC_ROOT } from '../../../config/config';

jest.mock('axios');
jest.mock('../../../config/authUtils');
const middlewares = [thunk];
const mockStore = configureStore(middlewares);
const store = mockStore({});

const adminRole = ["adminUser", "regularUser"];
const baseURL = `${SVC_ROOT}api/`;
let alert = jest.spyOn(window, 'alert').mockImplementation(() => {});

const users = [{
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
  },
  {
    firstName: "Natasha",
    lastName: "Romanov",
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
const filterParams = {
    "filter": {
        "utilization": {
            "min": 50,
            "max": 160
        },
        "locations": [
            {
                "locationID": 8,
                "province": "British Columbia",
                "city": "Vancouver"
            },
            {
                "locationID": 5,
                "province": "Alberta",
                "city": "Edmonton"
            }
        ],
        "disciplines": {
            "Intel": [
                "Deception",
                "False Identity Creation"
            ],
            "Martial Arts": [
                "Kali"
            ],
            "Weapons": []
        },
        "yearsOfExps": [
            "1-3",
            "3-5",
            "10+"
        ],
        "startDate": "2021-10-31T00:00:00",
        "endDate": "2022-02-12T00:00:00"
    },
    "searchWord": "e",
    "orderKey": "utilization",
    "order": "asc",
    "page": 1
};

describe('User Search', () => {
    afterEach(() => {
        store.clearActions();
        axios.post.mockClear();
        authUtils.getHeaders.mockClear();
    });

    it('should successfully perform user search', async () => {
        let isLastPage = false;
        let expectedActions = [
            {
            type: types.PERFORM_USER_SEARCH,
            users: users,
            isLastPage: isLastPage,
        }];
        let response = {data: {
            code: 200,
            status: "OK",
            payload: users,
            message: "Successfully retrieved information",
            extra: {isLastPage: isLastPage}
        }};

        axios.post.mockResolvedValueOnce(response);
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

        await store.dispatch(searchActions.performUserSearch(filterParams, adminRole));

        expect(store.getActions()).toEqual(expectedActions);
        expect(axios.post).toHaveBeenCalledTimes(1);
        expect(axios.post).toHaveBeenCalledWith(`${baseURL}users/search`, filterParams, {headers:{ Authorization: `Bearer 100` }});
    
    });

    it('should handle error when user search is rejected', async () => {
        const expectedActions = [];
        let errorMessage = 'Error';
        axios.post.mockImplementationOnce(() =>
            Promise.reject(new Error(errorMessage)),
        );

        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

        await store.dispatch(searchActions.performUserSearch(filterParams, adminRole));
       
        expect(alert).toHaveBeenCalledTimes(1);
        expect(store.getActions()).toEqual(expectedActions);
        expect(axios.post).toHaveBeenCalledTimes(1);
        expect(axios.post).toHaveBeenCalledWith(`${baseURL}users/search`, filterParams, {headers:{ Authorization: `Bearer 100` }});
    })
})

describe('clear search', () => {
    it('should clear search results', () => {
        let expectedAction = [{
            type: types.CLEAR_SEARCH_RESULTS
        }];

        store.dispatch(searchActions.clearSearchResults());

        expect(store.getActions()).toEqual(expectedAction);
    })
})