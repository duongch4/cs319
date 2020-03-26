import React, {useContext, useEffect } from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { loadProjects } from '../../redux/actions/projectsActions';
import ProjectList from './ProjectList';
import './ProjectStyles.css'
import Fab from '@material-ui/core/Fab';
import AddIcon from '@material-ui/icons/Add';
import { Link } from 'react-router-dom'
import {CLIENT_DEV_ENV} from '../../config/config';
import {fetchProfileFromLocalStorage, isProfileLoaded, UserContext} from "../common/userContext/UserContext";

const _ProjectsPage = (props) => {
  const user = useContext(UserContext);
  let userRoles = user.profile.userRoles;
  if (!isProfileLoaded(user.profile)) {
    let profile = fetchProfileFromLocalStorage();
    userRoles = profile.userRoles;
  }
  useEffect(() => {
    if (props.projects.length === 0) {
      if (CLIENT_DEV_ENV) {
        props.loadProjects(["adminUser"]);
      } else {
        props.loadProjects(userRoles)
            .catch(error => {
              alert('Loading projects failed' + error);
            });
      }
    }
  }, [props.projects, props.loadProjects, userRoles]);
  return (
    <div className="activity-container">
        <div className="title-bar">
          <h1 className="greenHeader">Manage Projects</h1>
          <div className="fab-container">
            <Link to={{
              pathname: "/add_project",
              state: {
                profile: props.profile
              }
            }}>
            <Fab
                style={{ backgroundColor: "#87c34b", boxShadow: "none"}}
                size={"small"}
                color="primary" aria-label="add">
             <AddIcon />
            </Fab>
            </Link>
          </div>
        </div>
        <ProjectList projects={props.projects}/>
    </div>
  );
};

_ProjectsPage.propTypes = {
  props: PropTypes.object.isRequired,
};

const mapStateToProps = state => {
  return {
    projects: state.projects,
  };
};

const mapDispatchToProps = {
  loadProjects
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(_ProjectsPage);
