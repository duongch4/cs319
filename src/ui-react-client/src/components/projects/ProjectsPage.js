import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { loadProjects } from '../../redux/actions/projectsActions';
import { loadLocations } from '../../redux/actions/locationsActions.js';
import ProjectList from './ProjectList';
import './ProjectStyles.css'

const _ProjectsPage = ({
  projects,
  locations,
  loadProjects,
  loadLocations,
}) => {
  useEffect(() => {
    if (projects.length === 0) {
      loadProjects()
      // XXX TODO: need to uncomment this once the full-stack is finished
      // .catch(error => {
      //   alert('Loading projects failed' + error);
      // });
    }

    if (locations.length === 0) {
      // XXX TODO: need to uncomment this once the full-stack is finished
      // loadLocations().catch(error => {
      //   alert('Loading locations failed' + error);
      // });
    }
  }, [projects, locations, loadProjects, loadLocations]);

  return (
    <>
      <h2 className="greenHeader">Manage Projects</h2>
      <ProjectList projects={projects} locations={locations} />
    </>
  );
};

_ProjectsPage.propTypes = {
  projects: PropTypes.array.isRequired,
  locations: PropTypes.array.isRequired,
  loadProjects: PropTypes.func.isRequired,
  loadLocations: PropTypes.func.isRequired,
};

const mapStateToProps = state => {
  return {
    projects: state.projects,
    // XXX TODO: uncomment in the future when we have done locations
      // state.locations.length === 0
      //   ? []
      //   : state.projects.map(project => {
      //       return {
      //         ...project,
      //         location: state.locations.find(
      //           element => element.id === project.locationId,
      //         ).name,
      //       };
      //     }),
    locations: state.locations,
  };
};

const mapDispatchToProps = {
  loadProjects,
  loadLocations,
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(_ProjectsPage);
