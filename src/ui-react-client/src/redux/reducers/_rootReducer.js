import { combineReducers } from 'redux';
import users from './usersReducer';
import projects from './projectsReducer';
import masterlist from './masterlistsReducer';

const rootReducer = combineReducers({
  users: users,
  projects: projects,
  masterlist: masterlist,
});

export default rootReducer;
