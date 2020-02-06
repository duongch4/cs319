import React from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import ProjectCard from './ProjectCard';

const ProjectList = ({ projects }) => {
  var projectList = []
  projects.forEach((project, index) => {
    projectList.push(<Link to={'/projects/' + project.projID} key={projectList.length}><ProjectCard number={index + 1} project={project} key={projectList.length}/></Link>)
  })
  return (
    <>
    {projectList}
    </>
  );
};

ProjectList.propTypes = {
  projects: PropTypes.array.isRequired,
};

export default ProjectList;
