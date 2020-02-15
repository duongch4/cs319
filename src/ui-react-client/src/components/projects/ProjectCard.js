import React from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';
import {Link} from "react-router-dom";
import EditIcon from '@material-ui/icons/Edit';

const ProjectCard = (props) => {
  const { number, project, isProjectList} = props;
  //TODO fix weird react object error with dates

  return (
    <div className="card-summary">
        <div className="card-summary-title">
            <h2 className="blueHeader">{number}</h2>
        </div>
        <div className="card-summary-title">
            <h2 className="blueHeader">{project.name}</h2>
            <p><strong>Location:</strong> {project.location.city}, {project.location.province}</p>
            {isProjectList && (
                <Link to={'/editproject/' + project.projID} className="action-link">
                    <EditIcon style={{fontSize: 'small'}}/> Edit
                </Link>
            )}
        </div>
        {/*//<p>Duration: {project.startDate} - {project.endDate}</p>*/}
    </div>
  );
};

ProjectCard.propTypes = {
  project: PropTypes.object.isRequired,
  number: PropTypes.number.isRequired,
  isProjectList: PropTypes.bool.isRequired
};

export default ProjectCard;
