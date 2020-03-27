import React from 'react';
import {Link} from 'react-router-dom';
import PropTypes from 'prop-types';

const ProjectManagerCard = ({projectManager}) => {
    return (
        <div className="card-summary">
            <div className="card-summary-title">
                <Link to={'/users/' + projectManager.userID}>
                    <h2 className="blueHeader">{projectManager.firstName + " " + projectManager.lastName}</h2>
                </Link>
                <p><b>Project Manager</b></p>
            </div>
        </div>
    )
};

ProjectManagerCard.propTypes = {
    projectManager: PropTypes.object.isRequired,
};

export default ProjectManagerCard
