import * as projectProfileActions from '../projectProfileActions';
import * as types from '../actionTypes';
import { SVC_ROOT } from '../../../config/config';
import * as authUtils from '../../../config/authUtils';
import configureStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import axios from 'axios';
import { createMemoryHistory } from 'history'

const adminRole = ["adminUser", "regularUser"];
const baseURL = `${SVC_ROOT}api/projects/`;

jest.mock('axios');
jest.mock('../../../config/authUtils');
const middlewares = [thunk];
const mockStore = configureStore(middlewares);
const store = mockStore({});

const projectProfile = {
    projectSummary: {
        title: "Martensville Athletic Pavillion",
        location: {
            locationID: 1,
            province: "Seskatchewan",
            city: "Saskatoon"
        },
        projectStartDate: "2020-10-31T00:00:00.0000000",
        projectEndDate: "2021-12-31T00:00:00.0000000",
        projectNumber: "2009-VD9D-15"
    },
    projectManager: {
        userID: 100,
        firstName: "Jason",
        lastName: "Bourne"
    },
    usersSummary: [
        {
            firstName: "Nicky",
            lastName: "Parsons",
            userID: 101,
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
            isConfirmed: true
        },
        {
            firstName: "Pamela",
            lastName: "Landy",
            userID: 102,
            location: {
                locationID: 3,
                province: "Ontario",
                city: "Toronto"
            },
            utilization: 100,
            resourceDiscipline: {
                disciplineID: 103,
                discipline: "Reconnaisance",
                yearsOfExp: "10+"
            },
            isConfirmed: false
        }
    ],
    openings: [
        {
            positionID: 1,
            discipline: "Environmental Design",
            skills: [
                "skill1", "skill2"
            ],
            yearsOfExp: "1-3",
            commitmentMonthlyHours: {
                "2020-01-01": 30,
                "2020-02-01": 50
             }
        },
        {
            positionID: 2,
            discipline: "Waste Management",
            skills: [],
            yearsOfExp: "1-3",
            commitmentMonthlyHours: {
                "2020-01-01": 30,
                "2020-02-01": 50
             }
        }
    ]
}

describe('loadUsers', () => {
    afterEach(() => {
      store.clearActions();
      axios.get.mockClear();
      axios.put.mockClear();
      axios.post.mockClear();
      axios.delete.mockClear();
      authUtils.getHeaders.mockClear();
  })

//loadSingleProject tests
    it('loadSingleProject: should successfully load single project action', async () => {
      const expectedAction = [{
        type: types.LOAD_SINGLE_PROJECT,
        projectProfile: projectProfile
    }];
      const resp = {data: {payload: projectProfile}};
      authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
  
      axios.get.mockResolvedValue(resp);
  
      await store.dispatch(projectProfileActions.loadSingleProject("2009-VD9D-15", adminRole));
  
      expect(store.getActions()).toEqual(expectedAction);
      expect(axios.get).toHaveBeenCalledTimes(1);
      expect(axios.get).toHaveBeenCalledWith(`${baseURL}2009-VD9D-15`, {headers:{ Authorization: `Bearer 100` }});
    });
  
    it('loadSingleProject: should handle error when load single project action is rejected', async () => {
        let errorMessage = "Error message";
        axios.get.mockRejectedValueOnce(() =>
            Promise.reject(new Error(errorMessage)),
        );
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
        let alert = jest.spyOn(window, 'alert').mockImplementation(() => {});

        await store.dispatch(projectProfileActions.loadSingleProject("2009-VD9D-15", adminRole));
        expect(alert).toHaveBeenCalledTimes(1);
        expect(axios.get).toHaveBeenCalledWith(`${baseURL}2009-VD9D-15`, {headers:{ Authorization: `Bearer 100` }});
    });

// createProject test
    it('createProject: should successfully create project action', async () => {
        const history = createMemoryHistory('/projects')
        const expectedAction = [{
            type: types.CREATE_PROJECT,
            projectProfile: projectProfile,
        },{
            type: types.ADD_PROJECT_SUMMARY,
            projectSummary: projectProfile.projectSummary
        },{
            type: types.UPDATE_USER_PROJECTS_CREATION,
            projectSummary: projectProfile.projectSummary,
            userID: projectProfile.projectManager.userID
        }];
        const resp = {data: {payload: projectProfile}};
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
    
        axios.post.mockResolvedValue(resp);
    
        await store.dispatch(projectProfileActions.createProject(projectProfile, history, adminRole));
    
        expect(store.getActions()).toEqual(expectedAction);
        expect(axios.post).toHaveBeenCalledTimes(1);
        expect(axios.post).toHaveBeenCalledWith(`${baseURL}`, projectProfile, {headers:{ Authorization: `Bearer 100` }});
    });

    it('createProject: should handle error when create project action is rejected', async () => {
        let errorMessage = "Error message";
        axios.get.mockImplementationOnce(() =>
        Promise.reject(new Error(errorMessage)),
        );
        const history = createMemoryHistory('/projects')
        const expectedAction = [{
            type: types.CREATE_PROJECT,
            projectProfile: projectProfile,
        },{
            type: types.ADD_PROJECT_SUMMARY,
            projectSummary: projectProfile.projectSummary
        },{
            type: types.UPDATE_USER_PROJECTS_CREATION,
            projectSummary: projectProfile.projectSummary,
            userID: projectProfile.projectManager.userID
        }];
        const resp = {data: {payload: projectProfile}};
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

        axios.post.mockResolvedValue(resp);

        await store.dispatch(projectProfileActions.createProject(projectProfile, history, adminRole));
        let alert = jest.spyOn(window, 'alert').mockImplementation(() => {});
        
        expect(alert).toHaveBeenCalledTimes(1);
        expect(store.getActions()).toEqual(expectedAction);
        expect(axios.post).toHaveBeenCalledTimes(1);
        expect(axios.post).toHaveBeenCalledWith(`${baseURL}`, projectProfile, {headers:{ Authorization: `Bearer 100` }}); 
    });

// updateProject tests
it('createProject: should successfully create project action', async () => {
    const history = createMemoryHistory('/projects')
    const expectedAction = [{
        type: types.UPDATE_PROJECT,
        projectProfile: projectProfile,
    },{
        type: types.UPDATE_PROJECT_SUMMARY,
        projectSummary: projectProfile.projectSummary
    }];
    const resp = {data: {payload: projectProfile}};
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

    axios.put.mockResolvedValue(resp);

    await store.dispatch(projectProfileActions.updateProject(projectProfile, history, adminRole));

    expect(store.getActions()).toEqual(expectedAction);
    expect(axios.put).toHaveBeenCalledTimes(1);
    expect(axios.put).toHaveBeenCalledWith(`${baseURL}2009-VD9D-15`, projectProfile, {headers:{ Authorization: `Bearer 100` }});
});

it('createProject: should handle error when create project action is rejected', async () => {
    let errorMessage = "Error message";
    axios.get.mockImplementationOnce(() =>
    Promise.reject(new Error(errorMessage)),
    );
    const history = createMemoryHistory('/projects')
    const expectedAction = [{
        type: types.UPDATE_PROJECT,
        projectProfile: projectProfile,
    },{
        type: types.UPDATE_PROJECT_SUMMARY,
        projectSummary: projectProfile.projectSummary
    }];
    const resp = {data: {payload: projectProfile}};
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

    axios.post.mockResolvedValue(resp);

    await store.dispatch(projectProfileActions.updateProject(projectProfile, history, adminRole));
    let alert = jest.spyOn(window, 'alert').mockImplementation(() => {});
    
    expect(alert).toHaveBeenCalledTimes(1);
    expect(store.getActions()).toEqual(expectedAction);
    expect(axios.put).toHaveBeenCalledTimes(1);
    expect(axios.put).toHaveBeenCalledWith(`${baseURL}2009-VD9D-15`, projectProfile, {headers:{ Authorization: `Bearer 100` }}); 
});

// deleteProject tests
it('createProject: should successfully create project action', async () => {
    const history = createMemoryHistory('/projects')
    const expectedAction = [{
        type: types.DELETE_PROJECT
    },{
        type: types.DELETE_PROJECT_SUMMARY,
        projectNumber: "2009-VD9D-15"
    },{
        type: types.UPDATE_USER_PROJECTS_DELETION,
        projectNumber: "2009-VD9D-15"
    }];
    const resp = {data: {payload: projectProfile}};
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

    axios.put.mockResolvedValue(resp);

    await store.dispatch(projectProfileActions.deleteProject("2009-VD9D-15", history, adminRole));

    expect(store.getActions()).toEqual(expectedAction);
    expect(axios.delete).toHaveBeenCalledTimes(1);
    expect(axios.delete).toHaveBeenCalledWith(`${baseURL}2009-VD9D-15`, {headers:{ Authorization: `Bearer 100` }});
});

it('createProject: should handle error when create project action is rejected', async () => {
    let errorMessage = "Error message";
    axios.get.mockImplementationOnce(() =>
    Promise.reject(new Error(errorMessage)),
    );
    const history = createMemoryHistory('/projects')
    const expectedAction = [{
        type: types.DELETE_PROJECT
    },{
        type: types.DELETE_PROJECT_SUMMARY,
        projectNumber: "2009-VD9D-15"
    },{
        type: types.UPDATE_USER_PROJECTS_DELETION,
        projectNumber: "2009-VD9D-15"
    }];
    const resp = {data: {payload: projectProfile}};
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

    axios.post.mockResolvedValue(resp);

    await store.dispatch(projectProfileActions.deleteProject("2009-VD9D-15", history, adminRole));
    let alert = jest.spyOn(window, 'alert').mockImplementation(() => {});
    
    expect(alert).toHaveBeenCalledTimes(1);
    expect(store.getActions()).toEqual(expectedAction);
    expect(axios.delete).toHaveBeenCalledTimes(1);
    expect(axios.delete).toHaveBeenCalledWith(`${baseURL}2009-VD9D-15`, {headers:{ Authorization: `Bearer 100` }}); 
});
});
