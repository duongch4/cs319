import projectsReducer from '../projectsReducer';
import * as types from '../../actions/actionTypes';

let initialState = [
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

afterEach(() => {
  initialState = [
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
    }]
});

it('should load the initial state as default' , () => {
  let action = {type: 'random_string'};
  let received = projectsReducer(initialState, action);

  expect(received).toEqual(initialState);
});

it('should load all projects from action payload', () => {
  let projectSummaries = [{project:'test1'}, {project:'test3'}, {project:'test3'}]
  let action = {
    type: types.LOAD_PROJECTS_ALL,
    projects: projectSummaries,
  };
  let received = projectsReducer(initialState, action);

  expect(received).not.toEqual(initialState);
  expect(received).toHaveLength(3);
  expect(received).toEqual(projectSummaries);
})

it('should remove project matching number from action payload', () => {
  let projectNumber = '2009-VD9D-16';
  let removedProject = initialState[1];
  let action = {
    type: types.DELETE_PROJECT_SUMMARY,
    projectNumber: projectNumber
  };
  let received = projectsReducer(initialState, action);

  expect(received).not.toEqual(initialState);
  expect(received).toHaveLength(1);
  expect(received).not.toContain(removedProject);
});

it('should not remove project if matching project number not found', () => {
  let projectNumber = 'stark-3000';
  let action = {
      type: types.DELETE_PROJECT_SUMMARY,
      projectNumber: projectNumber
  };
  let received = projectsReducer(initialState, action);

  expect(received).toEqual(initialState);
});

it('should add a project summary from action payload', () => {
  let projectSummary =   {
    title: "Defeat Vanko and Hammer Drones",
    location: {
        locationID: 2,
        province: "British Columbia",
        city: "Vancouver"
    },
    projectStartDate: "2020-10-31T00:00:00.0000000",
    projectEndDate: "2021-12-31T00:00:00.0000000",
    projectNumber: "stark-3000"
  };
  let action = {
    type: types.ADD_PROJECT_SUMMARY,
    projectSummary: projectSummary
  };
  let received = projectsReducer(initialState, action);

  expect(received).not.toEqual(initialState);
  expect(received).toHaveLength(3);
  expect(received).toContain(projectSummary);
})

it('should update the project matching the action payload', () => {
  let projectSummary =   {
    title: "Find and defeat the Mandarin",
    location: {
        locationID: 10,
        province: "Florida",
        city: "Miami"
    },
    projectStartDate: "2020-10-31T00:00:00.0000000",
    projectEndDate: "2021-12-31T00:00:00.0000000",
    projectNumber: "2009-VD9D-15"
  };
  let oldProjectSummary = initialState[0];
  let action = {
    type: types.UPDATE_PROJECT_SUMMARY,
    projectSummary: projectSummary
  };
  let received = projectsReducer(initialState, action);

  expect(received).not.toEqual(initialState);
  expect(received).toHaveLength(2);
  expect(received).not.toContain(oldProjectSummary);
  expect(received).toContain(projectSummary);
});

it('should not update project if matching project number not found', () => {
  let projectSummary =   {
    title: "Defeat Vanko and Hammer Drones",
    location: {
        locationID: 2,
        province: "British Columbia",
        city: "Vancouver"
    },
    projectStartDate: "2020-10-31T00:00:00.0000000",
    projectEndDate: "2021-12-31T00:00:00.0000000",
    projectNumber: "stark-3000"
  };
  let action = {
    type: types.UPDATE_PROJECT_SUMMARY,
    projectSummary: projectSummary
  };
  let received = projectsReducer(initialState, action);

  expect(received).toEqual(initialState);
})

