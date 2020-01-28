import projectsReducer from '../projectsReducer';
import * as actions from '../../actions/projectsActions';

const initialState = [
  {
    id: 1,
    number: '111',
    title: 'Test1',
    location: '',
    createdAt: '',
    updatedAt: '',
  },
  {
    id: 2,
    number: '222',
    title: 'Test2',
    location: '',
    createdAt: '',
    updatedAt: '',
  },
];

const newProject = {
  number: '',
  title: 'Test3',
  location: '',
};

const updateProject = {
  id: 2,
  number: '',
  title: 'Test22',
  location: '',
  createdAt: '',
  updatedAt: '',
};

const deleteProject = {
  id: 2,
  number: '222',
  title: 'Test2',
  location: '',
  createdAt: '',
  updatedAt: '',
};

it('should load most recent projects when passed LOAD_PROJECTS_MOST_RECENT', () => {
  const action = actions.loadProjectsMostRecentData(initialState);

  const newState = projectsReducer(initialState, action);

  expect(newState.length).toEqual(2);
  expect(newState[0].title).toEqual('Test1');
  expect(newState[1].title).toEqual('Test2');
});

it('should load all projects when passed LOAD_PROJECTS_ALL', () => {
  const action = actions.loadProjectsData(initialState);

  const newState = projectsReducer(initialState, action);

  expect(newState.length).toEqual(2);
  expect(newState[0].title).toEqual('Test1');
  expect(newState[1].title).toEqual('Test2');
});

it('should add project when passed CREATE_PROJECT', () => {
  const action = actions.createProjectData(newProject);

  const newState = projectsReducer(initialState, action);

  expect(newState.length).toEqual(3);
  expect(newState[0].title).toEqual('Test1');
  expect(newState[1].title).toEqual('Test2');
  expect(newState[2].title).toEqual('Test3');
});

it('should update project when passed UPDATE_PROJECT', () => {
  const action = actions.updateProjectData(updateProject);

  const newState = projectsReducer(initialState, action);
  const updatedProject = newState.find(
    element => element.id == updateProject.id,
  );
  const untouchedProject = newState.find(element => element.id == 1);

  expect(newState.length).toEqual(2);
  expect(untouchedProject.title).toEqual('Test1');
  expect(updatedProject.title).toEqual('Test22');
});

it('should delete project when passed DELETE_PROJECT', () => {
  const action = actions.deleteProjectData(deleteProject);

  const newState = projectsReducer(initialState, action);
  const deletedProject = newState.find(
    element => element.id == deleteProject.id,
  );
  const untouchedProject = newState.find(element => element.id == 1);

  expect(newState.length).toEqual(1);
  expect(untouchedProject.title).toEqual('Test1');
});
