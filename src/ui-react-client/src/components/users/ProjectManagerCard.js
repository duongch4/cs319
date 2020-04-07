import React from 'react';
import {Link} from 'react-router-dom';
import PropTypes from 'prop-types';
import {Button} from "@material-ui/core";

const ProjectManagerCard = ({projectNumber, projectManager, userRoles}) => {
    return (
        <div className="card-summary">
            <div className="card-summary-title">
                <Link to={'/users/' + projectManager.userID}>
                    <h2 className="blueHeader">{projectManager.firstName + " " + projectManager.lastName}</h2>
                </Link>
                <p><b>Project Manager</b></p>
            </div>
            {userRoles.indexOf('adminUser') !== -1 && 
            <div className="manager-button">
                <Link to={"/editmanager/" + projectNumber} className="action-link">
                    <Button variant="contained"
                            style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small"}}
                            disableElevation>
                        Edit Manager
                    </Button>
                </Link>
             </div>
            }
        </div>
    )
};

ProjectManagerCard.propTypes = {
    projectManager: PropTypes.object.isRequired,
};

export default ProjectManagerCard
