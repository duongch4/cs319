import React from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import ProjectCard from './ProjectCard';
import Button from '@material-ui/core/Button';

const ProjectList = ({ projects }) => {
  var projectList = []
  projects.forEach((project, index) => {
    projectList.push(
      <div key={projectList.length} className="card">
        <Link to={'/projects/' + project.projID}>
          <ProjectCard number={index + 1} project={project} isProjectList={true} key={projectList.length}/>
        </Link>
      </div>
    )
  })
  return (
    <div>
    {projectList}
    </div>
  );
};

ProjectList.propTypes = {
  projects: PropTypes.array.isRequired,
};

export default ProjectList;
