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

const App = () => {
  return (
    <div>
      <Header />
      <Switch>
        {/*All our Routes goes here!*/}
        <Route exact path="/" component={HomePage} />
        <Route exact path="/users" component={UsersPage} />
        <Route exact path="/projects" component={ProjectsPage} />
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

export default App;
