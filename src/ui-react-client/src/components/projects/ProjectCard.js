import React from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';
import {Link} from "react-router-dom";
import EditIcon from '@material-ui/icons/Edit';
import {formatDate} from "../../util/dateFormatter";

const ProjectCard = (props) => {
  const { number, project, canEditProject, onUserCard, userRole} = props;

  let details = [];
  if (onUserCard) {
      details.push(<p key={details.length}><strong>Position:</strong> {userRole.disciplineName}</p>);
  } else {
      let startDate = formatDate(project.projectStartDate);
      let endDate = formatDate(project.projectEndDate);
      if (project.location) {
          details.push(<p key={details.length}><strong>Location:</strong> {project.location.city}, {project.location.province}</p>);
      }
      details.push(<p key={details.length}><strong>Duration:</strong>{startDate} - {endDate}</p>);
  }

  return (
    <div className="card-summary">
        <div className="card-summary-title">
            <h2 className="blueHeader">{number}</h2>
        </div>
        <div className="card-summary-title">
            <Link to={'/projects/' + project.projectNumber}>
                <h2 className="blueHeader">{project.title}</h2>
            </Link>
            {details}
            {canEditProject && (<Link to={'/editproject/' + project.projectNumber} className="action-link">
                <EditIcon style={{fontSize: 'small'}}/> Edit </Link>)}
        </div>
    </div>
  )
};

ProjectCard.propTypes = {
  project: PropTypes.object.isRequired,
  number: PropTypes.number.isRequired,
  canEditProject: PropTypes.bool,
  onUserCard: PropTypes.bool,
  userRole: PropTypes.object
};

export default ProjectCard
