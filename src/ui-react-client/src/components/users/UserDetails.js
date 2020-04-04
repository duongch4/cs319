import React, { Component } from 'react';
import PropTypes from 'prop-types';
import './UserStyles.css';
import { connect } from 'react-redux';
import Openings from '../projects/Openings';
import ProjectCard from '../projects/ProjectCard'
import AvailabilityCard from './AvailabilityCard';
import { Button } from "@material-ui/core";
import { Link } from 'react-router-dom';
import { loadSpecificUser } from "../../redux/actions/userProfileActions";
import { CLIENT_DEV_ENV } from '../../config/config';
import { UserContext, getUserRoles } from "../common/userContext/UserContext";
import Loading from '../common/Loading';

class UserDetails extends Component {
    state = {
        userProfile: {}
    };

    componentDidMount = () => {
        if (CLIENT_DEV_ENV) {
            this.props.loadSpecificUser(this.props.match ? this.props.match.params.user_id : this.props.id, ['adminUser']);
            this.setState({
                ...this.state,
                userProfile: this.props.userProfile
            });
        } else {
            // we want to check if there is a role being passed in the props first and prioritize using that role
            // otherwise, we will try to fetch the role from the User Context Provider
            let userRoles;
            if (this.props.roles) {
                userRoles = this.props.roles
            } else {
                userRoles = getUserRoles(this.context);
            }
            let id = this.props.match ? this.props.match.params.user_id : this.props.id;
            if (Object.keys(this.props.userProfile).length > 0 &&
                this.props.userProfile.userSummary.userID === id) {
                this.setState({
                    ...this.state,
                    userProfile: this.props.userProfile
                })
            } else {
                this.props.loadSpecificUser(id, userRoles)
                    .then(() => {
                        var userProfile = this.props.userProfile;
                        if (userProfile) {
                            this.setState({
                                ...this.state,
                                userProfile: userProfile
                            })
                        }
                    })
            }
        }
    }

    render() {
        let userRoles;
        if (this.props.roles) {
            userRoles = this.props.roles
        } else {
            userRoles = getUserRoles(this.context);
        }
        let currUserID = (this.props.id || this.context.profile.userID)
        let userDetails = this.state.userProfile;
        if (Object.keys(userDetails).length === 0) {
            return (
                <div className="activity-container">
                    <Loading />
                </div>
            )
        } else {
            let disciplines = [];
            if (userDetails.disciplines) {
                userDetails.disciplines.forEach((discipline, index) => {
                    disciplines.push(<Openings opening={discipline} index={index} isAssignable={false} key={disciplines.length} />)
                });
            }

            const currentProjects = [];
            let counter = 1;
            if (userDetails.currentProjects && userDetails.currentProjects.length > 0) {
                // we want to get only the unique project titles from the list of projects.
                let uniqueProjects = [];
                userDetails.currentProjects.map(project => {
                    if (uniqueProjects.length === 0 ||
                        uniqueProjects.find(proj => proj.projectNumber === project.projectNumber) === undefined) {
                        uniqueProjects.push(project)
                    }
                });
                uniqueProjects.forEach(project => {
                    let projectRoles = userDetails.positions.filter((position => position.projectTitle === project.title));
                    // we find all the positions related to the project and create a card for each one
                    projectRoles.forEach(role => {
                        currentProjects.push(
                            <ProjectCard number={counter} project={project} canEditProject={false}
                                         onUserCard={true} userRole={role} key={currentProjects.length} />
                        )
                        counter++
                    })
                });
            } else {
                currentProjects.push(<p className="empty-statements" key={currentProjects.length}>There are currently no projects assigned to this resource.</p>)
            }
            let unavailability = [];
            if (userDetails.availability && userDetails.availability.length > 0) {
                userDetails.availability.forEach(currentAvailability => {
                    unavailability.push(<AvailabilityCard edit={false} availability={currentAvailability} key={unavailability.length} />)
                })
            } else {
                unavailability.push(<p className="empty-statements" key={unavailability.length}>This resource does not have any unavailabilities.</p>)
            }
            return (
                <div className="activity-container">
                    {
                        this.props.showGreeting && (
                            <h1>Welcome, {userDetails.userSummary.firstName}!</h1>
                        )}
                    <div className="title-bar">
                        <h1 className="blueHeader">{userDetails.userSummary.firstName + " " + userDetails.userSummary.lastName}</h1>
                        {(userRoles.includes("adminUser") || userDetails.userSummary.userID === currUserID) && (
                            <Link to={'/edituser/' + userDetails.userSummary.userID} className="action-link">
                                <Button variant="contained"
                                        style={{ backgroundColor: "#87c34b", color: "#ffffff", size: "small" }}
                                        disableElevation>
                                    Edit
                                </Button>
                            </Link>
                        )}
                    </div>
                    <div className="section-container">
                        <p><b>Utilization:</b> {userDetails.userSummary.utilization}</p>
                        <p><b>Location:</b> {userDetails.userSummary.location.city}, {userDetails.userSummary.location.province}</p>
                    </div>
                    <div className="section-container">
                        <h2 className="greenHeader">Discipline & Skills</h2>
                        {disciplines}
                    </div>
                    <div className="section-container">
                        <h2 className="greenHeader">Current Projects</h2>
                        {currentProjects}
                    </div>
                    <div className="section-container">
                        <h2 className="greenHeader">Unavailability</h2>
                        {unavailability}
                    </div>
                </div>)
        }
    }
}

UserDetails.contextType = UserContext;

UserDetails.propTypes = {
    userProfile: PropTypes.object
};

const mapStateToProps = state => {
    return {
        userProfile: state.userProfile,
    }
};

const mapDispatchToProps = {
    loadSpecificUser,
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(UserDetails)
