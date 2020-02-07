import { combineReducers } from 'redux';
import users from './usersReducer';
import projects from './projectsReducer';
import locations from './locationsReducer';
import disciplines from './disciplinesReducer';
import masterYearsOfExperience from './experienceReducer'

const rootReducer = combineReducers({
  users: users,
  projects: projects,
  locations: locations,
  disciplines: disciplines,
  masterYearsOfExperience: masterYearsOfExperience,
});

export default rootReducer;
