import { combineReducers } from 'redux';
import users from './usersReducer';
import projects from './projectsReducer';
import masterlist from './masterlistsReducer';
import userProfile from "./userProfileReducer";
import projectProfile from "./projectProfileReducer";

const rootReducer = combineReducers({
  users: users,
  projects: projects,
  masterlist: masterlist,
  userProfile: userProfile,
  projectProfile: projectProfile
});

export default rootReducer;
