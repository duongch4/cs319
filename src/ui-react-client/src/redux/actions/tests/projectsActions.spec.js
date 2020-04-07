import * as projectsActions from '../projectsActions';
import * as types from '../actionTypes';
import { SVC_ROOT } from '../../../config/config';
import * as authUtils from '../../../config/authUtils';
import configureStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import axios from 'axios';

const adminRole = ["adminUser", "regularUser"];
const baseURL = `${SVC_ROOT}api/projects/`;

jest.mock('axios');
jest.mock('../../../config/authUtils');
const middlewares = [thunk];
const mockStore = configureStore(middlewares);
const store = mockStore({});
const projects = [
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
},
  {
  title: "University of British Columbia Science Building",
  location: {
      locationID: 2,
      province: "British Columbia",
      city: "Vancouver"
  },
  projectStartDate: "2020-10-31T00:00:00.0000000",
  projectEndDate: "2021-12-31T00:00:00.0000000",
  projectNumber: "2009-VD9D-16"
}];
const isLastPage = false;

describe('loadProjects', () => {
  afterEach(() => {
    store.clearActions();
    axios.get.mockClear();
    authUtils.getHeaders.mockClear();
  });
  it('should load projects action', async() => {
    const expectedAction = {
      type: types.LOAD_PROJECTS_ALL,
      projects: projects,
      isLastPage: isLastPage,
    };
    const resp = {data: {payload: projects, extra: {isLastPage: isLastPage}}};
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

    axios.get.mockResolvedValue(resp);

    await store.dispatch(projectsActions.loadProjects("", adminRole));

    expect(store.getActions()[0]).toEqual(expectedAction);
    expect(axios.get).toHaveBeenCalledTimes(1);
    expect(axios.get).toHaveBeenCalledWith(`${baseURL}`, {headers:{ Authorization: `Bearer 100` }});
    const action = projectsActions.loadProjectsData(projects, isLastPage);

    expect(action).toEqual(expectedAction);
  });

  it('should handle error when load projects action fails', async() => {
    let errorMessage = "Error message";
    axios.get.mockImplementationOnce(() =>
      Promise.reject(new Error(errorMessage)),
    );

    const resp = {data: {payload: projects}};
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
    let alert = jest.spyOn(window, 'alert').mockImplementation(() => {});

    axios.get.mockResolvedValue(resp);
    await expect(store.dispatch(projectsActions.loadProjects("", adminRole))).rejects.toThrow(errorMessage);

    expect(alert).toHaveBeenCalledTimes(1);
    expect(axios.get).toHaveBeenCalledTimes(1);
    expect(axios.get).toHaveBeenCalledWith(`${baseURL}`, {headers:{ Authorization: `Bearer 100` }});

  });

  it('should load projects action with filter', async() => {
    const expectedAction = {
      type: types.LOAD_PROJECTS_ALL,
      projects: projects,
      isLastPage: false,
    };
    const resp = {data: {payload: projects, extra: {isLastPage: isLastPage}}};
    authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});

    axios.get.mockResolvedValue(resp);

    await store.dispatch(projectsActions.loadProjects("?&orderKey=utilization&order=desc&page=1", adminRole));

    expect(store.getActions()[0]).toEqual(expectedAction);
    expect(axios.get).toHaveBeenCalledTimes(1);
    expect(axios.get).toHaveBeenCalledWith(`${baseURL}?&orderKey=utilization&order=desc&page=1`, {headers:{ Authorization: `Bearer 100` }});
    const action = projectsActions.loadProjectsData(projects, isLastPage);

    expect(action).toEqual(expectedAction);
  });
});
