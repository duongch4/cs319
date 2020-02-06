import React from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';

const ProjectCard = (props) => {
  const { number, project } = props;
  return (
    <div className="projectCard">    
        <h1>{number}. {project.name}</h1>
        <p>Location: {project.location.city}, {project.location.province}</p>
        <p>Duration: {project.startDate} - {project.endDate}</p>
    </div>
  );
};

ProjectCard.propTypes = {
  project: PropTypes.object.isRequired,
  number: PropTypes.number.isRequired,
};

export default ProjectCard;
