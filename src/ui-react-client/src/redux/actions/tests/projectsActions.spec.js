import * as projectsActions from '../projectsActions';
import * as types from '../actionTypes';

const projects = [
  {
    id: 1,
    number: '',
    title: '',
    location: '',
    createdAt: '',
    updatedAt: '',
  },
];

const project = {
  number: '',
  title: '',
  location: '',
};

describe('loadProjects', () => {
  it('should load projects action', () => {
    const expectedAction = {
      type: types.LOAD_PROJECTS_ALL,
      projects: projects,
    };

    const action = projectsActions.loadProjectsData(projects);

    expect(action).toEqual(expectedAction);
  });
});

describe('loadProjectsMostRecent', () => {
  it('should load most recent projects action', () => {
    const expectedAction = {
      type: types.LOAD_PROJECTS_MOST_RECENT,
      projects: projects,
    };

    const action = projectsActions.loadProjectsMostRecentData(
      projects,
    );

    expect(action).toEqual(expectedAction);
  });
});

describe('createProject', () => {
  it('should createProject action', () => {
    const expectedAction = {
      type: types.CREATE_PROJECT,
      project: project,
    };

    const action = projectsActions.createProjectData(project);

    expect(action).toEqual(expectedAction);
  });
});

describe('updateProject', () => {
  it('should updateProject action', () => {
    const expectedAction = {
      type: types.UPDATE_PROJECT,
      project: project,
    };

    const action = projectsActions.updateProjectData(project);

    expect(action).toEqual(expectedAction);
  });
});

describe('deleteProject', () => {
  it('should deleteProject action', () => {
    const expectedAction = {
      type: types.DELETE_PROJECT,
      project: project,
    };

    const action = projectsActions.deleteProjectData(project);

    expect(action).toEqual(expectedAction);
  });
});
