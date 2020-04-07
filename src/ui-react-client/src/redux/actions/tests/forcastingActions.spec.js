import * as forcastingActions from '../forecastingActions';
import * as types from '../actionTypes';
import axios from 'axios';
import * as authUtils from '../../../config/authUtils';
import configureStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../../config/config';
import { createMemoryHistory } from 'history'

jest.mock('axios');
jest.mock('../../../config/authUtils');
const middlewares = [thunk];
const mockStore = configureStore(middlewares);
const store = mockStore({});

const adminRole = ["adminUser", "regularUser"];
const baseURL = `${SVC_ROOT}api/`;
let alert = jest.spyOn(window, 'alert').mockImplementation(() => {});

const opening = {
    positionID: 2,
    discipline: "Waste Management",
    skills: [],
    yearsOfExp: "1-3",
    commitmentMonthlyHours: {
        "2020-01-01": 30,
        "2020-02-01": 50
     }
};
const projectSummary = {
    title: "Martensville Athletic Pavillion",
    location: {
        locationID: 1,
        province: "Seskatchewan",
        city: "Saskatoon"
    },
    projectStartDate: "2020-10-31T00:00:00.0000000",
    projectEndDate: "2021-12-31T00:00:00.0000000",
    projectNumber: "2009-VD9D-15"
};

let user =  {
    firstName: 'Thor',
    lastName: 'Odinson',
    userID: 42,
    location: {
        locationID: 2,
        province: "British Columbia",
        city: "Vancouver"
    },
    utilization: 99,
    resourceDiscipline: {
        disciplineID: 300,
        discipline: "Lighting Bolts",
        yearsOfExp: "3-5"
    },
    isConfirmed: false
};

describe('Assign Opening', () => {
    afterEach(() => {
        store.clearActions();
        axios.put.mockClear();
        authUtils.getHeaders.mockClear();
    });

    it('should successfully assign a user to opening', async () => {
        const history = createMemoryHistory('/projects');
        const confirmedUtilization = 101;
        const expectedActions = [
            {
                type: types.ASSIGN_UPDATE_USER_DATA,
                userID: user.userID,
                confirmedUtilization: confirmedUtilization,
                opening: opening,
                projectSummary: projectSummary
            },
            {
            type: types.UPDATE_ASSIGN_OPENING,
            openingId: opening.positionID,
            userId: user.userID,
            confirmedUtilization: confirmedUtilization,
            user: user
        }];
        axios.put.mockResolvedValueOnce({data: {
            code: 200,
            status: "OK",
            openingId: opening.positionID,
            userID: user.userID,
            confirmedUtilization: confirmedUtilization,
            message: "Successfully retrieved information",
            extra: {}
        }});
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

        await store.dispatch(forcastingActions.createAssignOpenings(opening, user.userID, confirmedUtilization, user, adminRole, projectSummary, history));

        expect(store.getActions()).toEqual(expectedActions);
        expect(axios.put).toHaveBeenCalledTimes(1);
        expect(axios.put).toHaveBeenCalledWith(`${baseURL}positions/${opening.positionID}/assign/${user.userID}`, {}, { headers: { Authorization: `Bearer 100` } });
    });

    it('should handle error when assign is rejected', async () => {
        const history = createMemoryHistory('/projects');
        const confirmedUtilization = 101;
        const expectedActions = [];
        const errMessage = 'Error';
        axios.put.mockRejectedValueOnce(new Error(errMessage));
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

        await store.dispatch(forcastingActions.createAssignOpenings(opening, user.userID, confirmedUtilization, user, adminRole, projectSummary, history));

        expect(alert).toHaveBeenCalled();
        expect(store.getActions()).toEqual(expectedActions);
        expect(axios.put).toHaveBeenCalledTimes(1);
        expect(axios.put).toHaveBeenCalledWith(`${baseURL}positions/${opening.positionID}/assign/${user.userID}`, {}, { headers: { Authorization: `Bearer 100` } });
    })
});

describe('Confirm Opening', () => {
    afterEach(() => {
        store.clearActions();
        axios.put.mockClear();
        authUtils.getHeaders.mockClear();
    });

    it('should successfuly confirm a user assigned to an opening', async () => {
        const confirmedUtilization = 101;
        const expectedActions = [
            {
                type: types.CONFIRM_ASSIGN_OPENING,
                openingId: opening.positionID,
                userId: user.userID,
                confirmedUtilization: confirmedUtilization,
                userSummaryDisciplineName: user.resourceDiscipline.discipline
            },
            {
                type: types.UPDATE_USER_UTILIZATION,
                userID: user.userID,
                utilization: confirmedUtilization
              }
        ]
        axios.put.mockResolvedValueOnce({data: {
            code: 200,
            status: "OK",
            openingId: opening.positionID,
            userID: user.userID,
            confirmedUtilization: confirmedUtilization,
            message: "Successfully retrieved information",
            extra: {}
        }});
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
        
        await store.dispatch(forcastingActions.confirmAssignOpenings(opening.positionID, user.userID, confirmedUtilization, adminRole, user.resourceDiscipline.discipline));

        expect(store.getActions()).toEqual(expectedActions);
        expect(axios.put).toHaveBeenCalledTimes(1);
        expect(axios.put).toHaveBeenCalledWith(`${baseURL}positions/${opening.positionID}/confirm`, {}, { headers: { Authorization: `Bearer 100` } });
    
    });

    it('should handle error when confirmation is rejected', async () => {
        const confirmedUtilization = 101;
        const expectedActions = [];
        const errMessage = 'Error';
        axios.put.mockRejectedValueOnce(new Error(errMessage));
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

        await store.dispatch(forcastingActions.confirmAssignOpenings(opening.positionID, user.userID, confirmedUtilization, adminRole, user.resourceDiscipline.discipline));

        expect(alert).toHaveBeenCalled();
        expect(store.getActions()).toEqual(expectedActions);
        expect(axios.put).toHaveBeenCalledTimes(1);
        expect(axios.put).toHaveBeenCalledWith(`${baseURL}positions/${opening.positionID}/confirm`, {}, { headers: { Authorization: `Bearer 100` } });
    })
});

describe('Unassign Opening', () => {
    afterEach(() => {
        store.clearActions();
        axios.put.mockClear();
        authUtils.getHeaders.mockClear();
    });

    it('should successfully unassign a user from an opening', async () => {
        const confirmedUtilization = 101;
        const expectedActions = [
            {
                type: types.UNASSIGN_UPDATE_USER_DATA,
                openingID: opening.positionID,
                userID: user.userID,
                confirmedUtilization: confirmedUtilization,
                opening: opening,
                projectNumber: projectSummary.projectNumber
              },
              {
                type: types.UNASSIGN_OPENING,
                openingId: opening.positionID,
                userId: user.userID,
                confirmedUtilization: confirmedUtilization,
                userSummaryDisciplineName: user.resourceDiscipline.discipline,
                opening: opening
            }
        ];
        axios.put.mockResolvedValueOnce({data: {
            code: 200,
            status: "OK",
            openingId: opening.positionID,
            userId: user.userID,
            confirmedUtilization: confirmedUtilization,
            opening: opening,
            message: "Successfully retrieved information",
            extra: {}
        }});
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

        await store.dispatch(forcastingActions.unassignOpenings(opening.positionID, user.userID, confirmedUtilization, adminRole, user.resourceDiscipline.discipline, projectSummary.projectNumber));

        expect(store.getActions()).toEqual(expectedActions);
        expect(axios.put).toHaveBeenCalledTimes(1);
        expect(axios.put).toHaveBeenCalledWith(`${baseURL}positions/${opening.positionID}/unassign`, {}, { headers: { Authorization: `Bearer 100` } });
    });

    it('should handle error when unassign is rejected', async () => {
        const confirmedUtilization = 101;
        const expectedActions = [];
        const errMessage = 'Error';
        axios.put.mockRejectedValueOnce(new Error(errMessage));
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

        await store.dispatch(forcastingActions.unassignOpenings(opening.positionID, user.userID, confirmedUtilization, adminRole, user.resourceDiscipline.discipline, projectSummary.projectNumber));

        expect(alert).toHaveBeenCalled();
        expect(store.getActions()).toEqual(expectedActions);
        expect(axios.put).toHaveBeenCalledTimes(1);
        expect(axios.put).toHaveBeenCalledWith(`${baseURL}positions/${opening.positionID}/unassign`, {}, { headers: { Authorization: `Bearer 100` } });
    
    })
})