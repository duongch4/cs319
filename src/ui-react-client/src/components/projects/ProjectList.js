import React from 'react';
import PropTypes from 'prop-types';
import ProjectCard from './ProjectCard';
import Loading from '../common/Loading';

const ProjectList = ({ projects }) => {
  var projectList = [];
  projects.forEach((project, index) => {
    projectList.push(
      <div key={projectList.length} className="card">
          <ProjectCard number={index + 1} project={project}
                       canEditProject={true} key={projectList.length}/>
      </div>
    )
  });

  if(projects.length === 0){
    return(<Loading />)
  } 
  return (
      <div>
        {projectList}
      </div>)
};

ProjectList.propTypes = {
  projects: PropTypes.array.isRequired,
};

export default ProjectList
