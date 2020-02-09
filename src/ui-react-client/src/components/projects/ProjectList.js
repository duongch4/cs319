import React from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import ProjectCard from './ProjectCard';

const ProjectList = ({ projects }) => {
  var projectList = []
  projects.forEach((project, index) => {
    projectList.push(
      <div key={projectList.length}>
        <Link to={'/projects/' + project.projID}>
          <ProjectCard number={index + 1} project={project} key={projectList.length}/>
        </Link>
        <Link to={'/editproject/' + project.projID} className="btn btn-primary">
          <button>Edit</button>
        </Link>
      </div>
    )
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
