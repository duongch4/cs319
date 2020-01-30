import React from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import ProjectCard from './ProjectCard';

const ProjectList = ({ projects }) => {
  var projectList = []
  projects.forEach((project, index) => {
    projectList.push(<Link to={'/projects/' + project.id}><ProjectCard number={index + 1} project={project} key={projectList.length}/></Link>)
  })
  var project = projects[0];
  var index = 0;
  return (
    <>
    {projectList}
    
    {/* <Link to={'/projects/' + project.id} key={project.id}><ProjectCard number={index + 1} project={project} key={projectList.length}/></Link> */}
    </>
  );
};

ProjectList.propTypes = {
  projects: PropTypes.array.isRequired,
};

export default ProjectList;
