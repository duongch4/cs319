import React from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';

const ProjectCard = (props) => {
    console.log(props)
    // console.log(project)
    const { number, project } = props;
  return (
    <div className="projectCard">    
        <h1>{number}. {project.title}</h1>
        <p>Location: {project.locationName}</p>
        <p>Duration: {project.createdAt} - {project.updatedAt}</p>
    </div>
  );
};

ProjectCard.propTypes = {
  project: PropTypes.object.isRequired,
  number: PropTypes.number.isRequired,
};

export default ProjectCard;
