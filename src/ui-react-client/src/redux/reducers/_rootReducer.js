import { combineReducers } from 'redux';
import users from './usersReducer';
import projects from './projectsReducer';
import locations from './locationsReducer';

const rootReducer = combineReducers({
  users: users,
  projects: projects,
  locations: locations,
});

export default rootReducer;
