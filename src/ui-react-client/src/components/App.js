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

const App = () => {
  return (
    <>
      <Header />
      <Switch>
        {/*All our Routes goes here!*/}
        <Route exact path="/" component={HomePage} />
        <Route path="/users" component={UsersPage} />
        <Route exact path="/projects" component={ProjectsPage} />
        <Route path="/locations" component={LocationsPage} />
        <Route path="/projects/:project_id" component={ProjectDetails} />
        <Route path="/add_project" component={AddProject} />
        <Route component={PageNotFound} />
      </Switch>
    </>
  );
};

export default App;
