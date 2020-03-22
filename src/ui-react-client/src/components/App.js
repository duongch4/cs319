import React from 'react';
import { Route, Switch } from 'react-router-dom';
//Import all needed Components
import Header from './common/Header';
import HomePage from './home/HomePage';
import UsersPage from './users/UsersPage';
import ProjectsPage from './projects/ProjectsPage';
import ProjectDetails from './projects/ProjectDetails';
import LocationsPage from './locations/LocationsPage';
import PageNotFound from './PageNotFound';
import AddProject from './projects/AddProject.js';
import UserDetails from './users/UserDetails.js';
import EditProject from './projects/EditProject';
import EditUser from './users/EditUser.js';
import Admin from './admin/Admin.js'
import Search from './common/search/Search.js';

import AuthProvider from "../config/AuthProvider";
import PropTypes from "prop-types";
import { PropsRoute } from "../util/CustomRoute";
import {acquireToken, GRAPH_REQUESTS, isIE} from "../config/authUtils";

const App = (appProps) => {
  return (
    <div>
      <Header appProps={appProps} />
      <Switch>
        {/*All our Routes goes here!*/}
        <PropsRoute exact path="/" component={HomePage} profile={appProps.graphProfile} account={appProps.account} />
        <Route exact path="/users" component={UsersPage} />
        <PropsRoute exact path="/projects" component={ProjectsPage} profile={appProps.graphProfile} />
        <Route exact path="/search" component={Search} />
        <Route path="/locations" component={LocationsPage} />
        <Route path="/projects/:project_id" component={ProjectDetails} />
        <Route path="/users/:user_id" component={UserDetails} />
        <Route path="/add_project" component={AddProject} />
        <Route path="/editproject/:project_number" component={EditProject} />
        <Route path="/edituser/:user_id" component={EditUser} />
        <Route path="/admin" component={Admin} />
        <Route component={PageNotFound} />
      </Switch>
    </div>
  );
};

export const getHeaders = async () => {
  try {
    const tokenRequest = GRAPH_REQUESTS.API_ADMIN;
    const tokenResponse = await acquireToken(tokenRequest, isIE());
    // console.log("MY TOKEN RESPONSE", tokenResponse); // TODO: Lots of Info here!!!
    return { Authorization: `Bearer ${tokenResponse.accessToken}` };
  }
  catch(error) {
    throw error;
  }
}

App.propTypes = {
  account: PropTypes.object,
  emailMessages: PropTypes.object,
  error: PropTypes.string,
  graphProfile: PropTypes.object,
  onSignIn: PropTypes.func.isRequired,
  onSignOut: PropTypes.func.isRequired,
  onRequestEmailToken: PropTypes.func.isRequired
};

export default AuthProvider(App);
