import { createStore } from 'redux';
import rootReducer from './reducers/_rootReducer';
import initialState from './reducers/_initialState';
import * as projectsActions from './actions/projectsActions';

it('Should handle creating projects', () => {
  const store = createStore(rootReducer, initialState);
  const project = {
    number: '2020-ABC-001',
  };

  const action = projectsActions.createProjectData(project);
  store.dispatch(action);

  const createdProject = store.getState().projects[0];
  expect(createdProject).toEqual(project);
});
