import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { loadProjects } from '../../redux/actions/projectsActions';
import ProjectList from './ProjectList';
import './ProjectStyles.css'
import Fab from '@material-ui/core/Fab';
import AddIcon from '@material-ui/icons/Add';
import { Link } from 'react-router-dom'
import {CLIENT_DEV_ENV} from '../../config/config';

const _ProjectsPage = ({
  projects,
  loadProjects,
}) => {
  useEffect(() => {
    if (projects.length === 0) {
      if (CLIENT_DEV_ENV) {
        loadProjects()
      } else {
        loadProjects()
        .catch(error => {
          alert('Loading projects failed' + error);
        });
      }
      
    }
  }, [projects, loadProjects]);
  return (
    
    <div className="activity-container">
        <div className="title-bar">
          <h1 className="greenHeader">Manage Projects</h1>
          <div className="fab-container">
            <Fab
                style={{ backgroundColor: "#87c34b", boxShadow: "none"}}
                size={"small"}
                color="primary" aria-label="add" component={Link} to="/add_project">
             <AddIcon />
            </Fab>
          </div>
        </div>
        <ProjectList projects={projects}/>
    </div>
  );
};

_ProjectsPage.propTypes = {
  projects: PropTypes.array.isRequired,
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
