import React from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';
import {Link} from "react-router-dom";
import EditIcon from '@material-ui/icons/Edit';
import {formatDate} from "../../util/dateFormatter";

const ProjectCard = (props) => {
  const { number, project, canEditProject} = props;
  var startDate = formatDate(project.projectStartDate);
  var endDate = formatDate(project.projectEndDate);

  return (
    <div className="card-summary">
        <div className="card-summary-title">
            <h2 className="blueHeader">{number}</h2>
        </div>
        <div className="card-summary-title">
            <Link to={'/projects/' + project.projectNumber}>
                <h2 className="blueHeader">{project.title}</h2>
            </Link>
            <p><strong>Location:</strong> {project.location.city}, {project.location.province}</p>
            <p><strong>Duration:</strong>{startDate} - {endDate}</p>
            {canEditProject && (<Link to={'/editproject/' + project.projectNumber} className="action-link">
                <EditIcon style={{fontSize: 'small'}}/> Edit </Link>)}
        </div>
    </div>
  )
};

ProjectCard.propTypes = {
  project: PropTypes.object.isRequired,
  number: PropTypes.number.isRequired,
  canEditProject: PropTypes.bool
};

export default ProjectCard
