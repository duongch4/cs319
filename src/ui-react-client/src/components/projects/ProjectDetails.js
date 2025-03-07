import React, {Component} from 'react';
import Openings from './Openings'
import './ProjectStyles.css';
import {connect} from 'react-redux';
import UserCard from "../users/UserCard";
import {Button} from "@material-ui/core";
import {Link} from 'react-router-dom';
import {loadSingleProject} from "../../redux/actions/projectProfileActions";
import {formatDate} from "../../util/dateFormatter";
import {CLIENT_DEV_ENV} from '../../config/config';
import ProjectManagerCard from "../users/ProjectManagerCard";
import {UserContext, getUserRoles, isAdminUser} from "../common/userContext/UserContext";
import Loading from '../common/Loading';

class ProjectDetails extends Component {
    state = {
        projectProfile: this.props.projectProfile
    };

    componentDidMount = () => {
        if (CLIENT_DEV_ENV) {
            this.props.loadSingleProject(this.props.match.params.project_id, ['adminUser']);
            var projectProfile = this.props.projectProfile;
            if (projectProfile) {
                this.setState({
                    ...this.state,
                    projectProfile
                })
            }
        } else {
            // checks if there is a newly added opening to the project, if there is, it makes a server call to get
            // the correct openingID for the project
            if (Object.keys(this.props.projectProfile).length > 0 &&
            this.props.projectProfile.projectSummary.projectNumber === this.props.match.params.project_id
            && this.props.projectProfile.openings.findIndex(opening => opening.openingID === 0) !== -1) {
                this.setState({
                    ...this.state,
                    projectProfile: this.props.projectProfile
                })
            } else {
            const userRoles = getUserRoles(this.context);
            this.props.loadSingleProject(this.props.match.params.project_id, userRoles)
                .then(res => {
                    var projectProfile = this.props.projectProfile;
                    if (projectProfile) {
                        this.setState({
                            ...this.state,
                            projectProfile
                        })
                    }
                })
                .catch(err => console.log(err));
            }
        }
    };

    componentDidUpdate = (prevProps) => {
        if(prevProps.projectProfile !== this.props.projectProfile){
            var projectProfile = this.props.projectProfile;
            if (projectProfile) {
                this.setState({
                    ...this.state,
                    projectProfile,
                })
            }
        }
    }

    render() {
        let userRoles = getUserRoles(this.context);
        if (Object.keys(this.state.projectProfile).length !== 0 &&
            this.state.projectProfile.projectSummary.projectNumber === this.props.match.params.project_id) {
            var openingsRender = [];
            var openings = this.state.projectProfile.openings;
            if (openings.length > 0) {
                openings.forEach((opening, index) => {
                    openingsRender.push(<Openings opening={opening}
                                                  index={index} commitment={opening.commitmentMonthlyHours}
                                                  isAssignable={isAdminUser(userRoles)} key={openingsRender.length}/>);
                    if (openings.length - 1 !== index) {
                        openingsRender.push(<hr key={openingsRender.length}/>)
                    }
                })
            } else {
                openingsRender.push(<p className="empty-statements" key={openingsRender.length}>There are currently no openings for this
                    project.</p>);
            }

            var teamMembersRender = [];
            const userSummaries = this.state.projectProfile.usersSummary;
            const projectManager = this.state.projectProfile.projectManager;
            const projectDetails = this.state.projectProfile;
            teamMembersRender.push(<ProjectManagerCard userRoles={userRoles} projectManager={projectManager} key={teamMembersRender.length}/>);
            userSummaries.forEach(userSummary => {
                teamMembersRender.push(
                    <UserCard user={userSummary}
                    canEdit={isAdminUser(userRoles)}
                    key={teamMembersRender.length}
                    canConfirm={isAdminUser(userRoles)}
                    projectDetails={projectDetails}
                    showOpeningInfo={true}
                    canUnassign={isAdminUser(userRoles)}/>)
            });

            if (this.state.projectProfile === null) {
                return (
                    <div className="activity-container">
                        <h1 className="blueHeader">No Project Available</h1>
                    </div>
                )
            }

            var projectStartDate = formatDate(projectDetails.projectSummary.projectStartDate);
            var projectEndDate = formatDate(projectDetails.projectSummary.projectEndDate);

            return (
                <div className="activity-container">
                    <div className="title-bar">
                        <h1 className="blueHeader">{projectDetails.projectSummary.title}</h1>
                        { isAdminUser(userRoles) && (
                            <Link to={'/editproject/' + projectDetails.projectSummary.projectNumber} className="action-link">
                                <Button variant="contained"
                                        style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small"}}
                                        disableElevation>
                                    Edit
                                </Button>
                            </Link>
                        )}
                    </div>
                    <div className="section-container">
                        <p>
                            <b>Location: </b> {projectDetails.projectSummary.location.city}, {projectDetails.projectSummary.location.province}
                        </p>
                        <p><b>Duration: </b> {projectStartDate} - {projectEndDate}</p>
                    </div>
                    <div className="section-container">
                        <h2 className="greenHeader">The Team</h2>
                        {teamMembersRender}
                    </div>
                    <div className="section-container">
                        <h2 className="greenHeader">Openings</h2>
                        {openingsRender}
                    </div>
                </div>
            )
        } else {
            return (
            <div className="activity-container"><Loading /></div>)
        }

    }
}

ProjectDetails.contextType = UserContext;

const mapStateToProps = state => {
    return {
        projectProfile: state.projectProfile,
    }
};

const mapDispatchToProps = {
    loadSingleProject
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(ProjectDetails)
