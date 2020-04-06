import React, { Component } from 'react';
import './UserStyles.css';
import EditIcon from "@material-ui/icons/Edit";
import {Link} from "react-router-dom";
import {Button} from "@material-ui/core";
import FiberManualRecordIcon from '@material-ui/icons/FiberManualRecord';
import {confirmAssignOpenings} from '../../redux/actions/forecastingActions';
import {unassignOpenings} from '../../redux/actions/forecastingActions';
import {loadSpecificUser} from '../../redux/actions/userProfileActions';
import {UserContext, getUserRoles} from "../common/userContext/UserContext";
import {connect} from "react-redux";
import ClearIcon from '@material-ui/icons/Clear';
import {
    LOW_UTILIZATION,
    MEDIUM_UTILIZATION,
    HIGH_UTILIZATION,
    LOW_UTILIZATION_COLOUR,
    MEDIUM_UTILIZATION_COLOUR,
    HIGH_UTILIZATION_COLOUR,
    OVER_UTILIZATION_COLOUR
} from '../../config/config';

class UserCard extends Component {

    onConfirm = (user) => {
        const userRoles = getUserRoles(this.context);
        let userSummaryDisciplineName = user.resourceDiscipline.discipline;
        let projectTitle = this.props.projectDetails.projectSummary.title;

        //because userSummary doesn't include positionID, need to triage to get it from the usersProfile
        this.props.loadSpecificUser(user.userID, userRoles).then(() => {
            let targetUsersPositionsInProject = this.props.userProfile.positions.filter(opening => opening.projectTitle === projectTitle);

            targetUsersPositionsInProject.forEach((position, index) => {
                let userProfileDisciplineName = position.disciplineName;
                //ideally we would compare disciplineID here instead of name, but positions
                //in userProfile only have name, not ID
                if (userProfileDisciplineName === userSummaryDisciplineName){
                    this.props.confirmAssignOpenings(position.positionID, user.userID, user.utilization, userRoles, userSummaryDisciplineName);
                }
            });
        });
    }

    onUnassign = (user) => {
        const userRoles = getUserRoles(this.context);
        let userSummaryDisciplineName = user.resourceDiscipline.discipline;
        let projectTitle = this.props.projectDetails.projectSummary.title;

        //because userSummary doesn't include positionID, need to triage to get it from the usersProfile
        this.props.loadSpecificUser(user.userID, userRoles).then(() => {
            let targetUsersPositionsInProject = this.props.userProfile.positions.filter(opening => {
                return opening.projectTitle === projectTitle
            });

            targetUsersPositionsInProject.forEach((position, index) => {
                let userProfileDisciplineName = position.disciplineName;
                //ideally we would compare disciplineID here instead of name, but positions
                //in userProfile only have name, not ID
                if (userProfileDisciplineName === userSummaryDisciplineName){
                    this.props.unassignOpenings(position.positionID, user.userID, user.utilization, userRoles, userSummaryDisciplineName);
                }
            });
        });
    }

    render(){
        const {user, canEdit, canConfirm, showOpeningInfo, canUnassign} = this.props;
        let isUserConfirmed = user.isConfirmed;

        let colour = ""
        if(user.utilization <= LOW_UTILIZATION){
            colour = LOW_UTILIZATION_COLOUR
        } else if(user.utilization <= MEDIUM_UTILIZATION){
            colour = MEDIUM_UTILIZATION_COLOUR
        } else if(user.utilization <= HIGH_UTILIZATION){
            colour = HIGH_UTILIZATION_COLOUR
        } else {
            colour = OVER_UTILIZATION_COLOUR
        }
        return(
            <div className="card-summary">
                <div className="card-summary-title">
                    <div className="confirmAssign">
                        <Link to={'/users/' + user.userID}>
                            <h2 className="blueHeader">{user.firstName + " " + user.lastName}</h2>
                        </Link>

                        {canConfirm && !isUserConfirmed && (
                            <Button onClick={() => this.onConfirm(user)} className="action-link">
                                <FiberManualRecordIcon style={{fontSize: 'small', color: 'red'}}/> Confirm
                            </Button>
                        )}
                    </div>
                    { showOpeningInfo && (
                        <p><b>Discipline:</b> {user.resourceDiscipline.discipline + " | "}
                        <b>Years of Experience:</b> {user.resourceDiscipline.yearsOfExp + " years"}</p>
                    )}
                    <p><b>Location:</b> {user.location.city}, {user.location.province}</p>

                    <div className="form-row">
                    {canUnassign && (
                     <Button onClick={() => this.onUnassign(user)}>
                      <div className="action-link">
                        <ClearIcon style={{fontSize: 'small', color: '#87C34B'}}/> Unassign
                      </div>
                      </Button>
                    )}

                        { canEdit && (
                            <Link to={'/edituser/' + user.userID} className="action-link">
                                <Button>
                                    <div className="action-link">
                                        <EditIcon style={{fontSize: 'small'}}/> Manage User
                                    </div>
                                </Button>
                            </Link>
                        )}
                    </div>
                </div>
                <div className="card-summary-title utilization">
                    <p style={{color: colour}}>{user.utilization}%</p>
                </div>
            </div>
        )
    }
}
UserCard.contextType = UserContext;

const mapStateToProps = state => {
    return {
        userProfile: state.userProfile,
        projectProfile: state.projectProfile,
    };
};

const mapDispatchToProps = {
    confirmAssignOpenings,
    loadSpecificUser,
    unassignOpenings
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(UserCard);
