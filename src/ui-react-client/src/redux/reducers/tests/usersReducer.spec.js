import usersReducer from '../usersReducer';
import * as types from '../../actions/actionTypes';

let initialState = [{
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

afterEach(() => {
  initialState = [{
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
});

it('should load the initial state as default' , () => {
  let action = {type: 'random_string'};
  let received = usersReducer(initialState, action);

  expect(received).toEqual(initialState);
});

it('should load all users from action payload', () => {
  let userSummaries = [{user: 'test1'}, {user: 'test2'}, {user: 'test3'}];
  let action = {
    type: types.LOAD_USERS_ALL,
    users: userSummaries
  };
  let received = usersReducer(initialState, action);

  expect(received).not.toEqual(initialState);
  expect(received).toEqual(userSummaries);
});

it('should update user summary from action payload', () => {
  let oldSummary = initialState[0];
  let userSummary = {
    firstName: "New User",
    lastName: "Name",
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
  };
  let action = {
      type: types.UPDATE_USER_SUMMARIES,
      userSummary: userSummary
  };
  let received = usersReducer(initialState, action);

  expect(received).not.toEqual(initialState);
  expect(received).not.toContain(oldSummary);
  expect(received).toContain(userSummary);
  expect(received).toHaveLength(2);
});

it('should not update user summary if matching user id not found', () => {
  let userSummary = {
    firstName: "New User",
    lastName: "Name",
    userID: 42,
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
  };
  let action = {
      type: types.UPDATE_USER_SUMMARIES,
      userSummary: userSummary
  };
  let received = usersReducer(initialState, action);

  expect(received).toEqual(initialState);
});

it('should return the searched for users from action payload', () => {
  let users = [{user: 'testA'}, {user: 'testB'}, {user: 'testC'}];
  let action = {
    type: types.PERFORM_USER_SEARCH,
    users: users
  };
  let received = usersReducer(initialState, action);

  expect(received).not.toEqual(initialState);
  expect(received).toEqual(users);
});

it('should return an empty array upon clearing the search', () => {
  let action = {type: types.CLEAR_SEARCH_RESULTS};
  let received = usersReducer(initialState, action);

  expect(received).not.toEqual(initialState);
  expect(received).toEqual([]);
});