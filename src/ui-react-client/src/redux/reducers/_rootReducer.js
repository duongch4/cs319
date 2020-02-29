import { combineReducers } from 'redux';
import users from './usersReducer';
import projects from './projectsReducer';
import masterlist from './masterlistsReducer';
import userProfile from "./userProfileReducer";
import projectProfile from "./projectProfileReducer";
import userProfiles from './userProfileReducer'

const rootReducer = combineReducers({
  users: users,
  projects: projects,
  masterlist: masterlist,
  userProfile: userProfile,
  projectProfile: projectProfile,
  userProfiles: userProfiles
});

export default rootReducer;
