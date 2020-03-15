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

class ProjectDetails extends Component {
    state = {
        projectProfile: this.props.projectProfile
    };

    componentDidMount = () => {
        if (CLIENT_DEV_ENV) {
            this.props.loadSingleProject(this.props.match.params.project_id);
            var projectProfile = this.props.projectProfile;
            if (projectProfile) {
                this.setState({
                    projectProfile
                })
            }
        } else {
            this.props.loadSingleProject(this.props.match.params.project_id)
                .then(res => {
                    var projectProfile = this.props.projectProfile;
                    if (projectProfile) {
                        this.setState({
                            projectProfile
                        })
                    }
                })
                .catch(err => console.log(err));
        }
    };

    render() {
        if (Object.keys(this.state.projectProfile).length !== 0) {
            var openingsRender = [];
            var openings = this.state.projectProfile.openings;
            if (openings.length > 0) {
                openings.forEach((opening, index) => {
                    openingsRender.push(<Openings opening={opening}
                                                  index={index} commitment={opening.commitmentMonthlyHours}
                                                  isAssignable={true} key={openingsRender.length}/>);
                    if (openings.length - 1 !== index) {
                        openingsRender.push(<hr key={openingsRender.length}/>)
                    }
                })
            } else {
                openingsRender.push(<p className="empty-statements" key={openingsRender.length}>There are currently no openings for this
                    project.</p>);
            }

            var teamMembersRender = [];
            var userSummaries = this.state.projectProfile.usersSummary;
            if (userSummaries.length > 0) {
                userSummaries.forEach(userSummary => {
                    teamMembersRender.push(
                        <UserCard user={userSummary} canEdit={false} key={teamMembersRender.length}/>)
                })
            } else {
                teamMembersRender.push(
                    <p className="empty-statements" key={teamMembersRender.length}>There are currently no resources assigned to this project.</p>
                )
            }

            if (this.state.projectProfile === null) {
                return (
                    <div className="activity-container">
                        <h1 className="blueHeader">No Project Available</h1>
                    </div>
                )
            }
            const projectDetails = this.state.projectProfile;
            var projectStartDate = formatDate(projectDetails.projectSummary.projectStartDate);
            var projectEndDate = formatDate(projectDetails.projectSummary.projectEndDate);

            return (
                <div className="activity-container">
                    <div className="title-bar">
                        <h1 className="blueHeader">{projectDetails.projectSummary.title}</h1>
                        <Link to={'/editproject/' + projectDetails.projectSummary.projectNumber} className="action-link">
                            <Button variant="contained"
                                    style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small"}}
                                    disableElevation>
                                Edit
                            </Button>
                        </Link>
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
            return <div>Loading</div>
        }

    }
}

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
