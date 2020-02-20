import React from 'react';
import PropTypes from 'prop-types';
import ProjectCard from './ProjectCard';

const ProjectList = ({ projects }) => {
  var projectList = [];
  // console.log(projects);
  projects.forEach((project, index) => {
    // console.log(project);
    projectList.push(
      <div key={projectList.length} className="card">
          <ProjectCard number={index + 1} project={project}
                       canEditProject={true} key={projectList.length}/>
      </div>
    )
  });
  return (
      <div>
        {projectList}
      </div>)
}

ProjectList.propTypes = {
  projects: PropTypes.array.isRequired,
};

export default ProjectList
