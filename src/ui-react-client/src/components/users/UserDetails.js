import React, { Component}  from 'react';
import PropTypes from 'prop-types';
import './UserStyles.css';
import { connect } from 'react-redux';
import Openings from '../projects/Openings';
import ProjectCard from '../projects/ProjectCard'
import AvailabilityCard from './AvailabilityCard';
import {Button} from "@material-ui/core";
import { Link } from 'react-router-dom';
import {loadSpecificUser} from "../../redux/actions/userProfileActions";
import {CLIENT_DEV_ENV} from '../../config/config';
import {fetchProfileFromLocalStorage, isProfileLoaded, UserContext} from "../common/userContext/UserContext";

class UserDetails extends Component {
    state = {
        userProfile: {}
    };

    componentDidMount = () => {
      if(CLIENT_DEV_ENV){
            this.props.loadSpecificUser(this.props.match.params.user_id, ['adminUser']);
            this.setState( {
                ...this.state,
                userProfile: this.props.userProfile
            });
        } else {
          let user = this.context;
          let userRoles = user.profile.userRoles;
          if (!isProfileLoaded(user.profile)) {
              let profile = fetchProfileFromLocalStorage();
              user.updateProfile(profile);
              userRoles = profile.userRoles;
          }
          this.props.loadSpecificUser(this.props.match.params.user_id, userRoles)
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
    };

    render() {
        let userDetails = this.state.userProfile;
        if (Object.keys(userDetails).length === 0) {
            return (
                <div className="activity-container">
                    <h1>Loading User Data...</h1>
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
            if(userDetails.currentProjects && userDetails.currentProjects.length > 0){
                userDetails.currentProjects.forEach((project, index) => {
                    let projectRole = userDetails.positions.filter((position => position.projectTitle === project.title));
                    currentProjects.push(
                        <ProjectCard number={index} project={project} canEditProject={false}
                                     onUserCard={true} userRole={projectRole[0]} key={currentProjects.length}/>
                        )
                })
            } else {
                currentProjects.push(<p className="empty-statements" key={currentProjects.length}>There are currently no projects assigned to this resource.</p>)
            }
            let unavailability = [];
            if(userDetails.availability && userDetails.availability.length > 0) {
                userDetails.availability.forEach(currentAvailability => {
                    unavailability.push(<AvailabilityCard availability={currentAvailability} key={unavailability.length}/>)
                })
            } else {
                unavailability.push(<p className="empty-statements" key={unavailability.length}>This resource does not have any unavailabilities.</p>)
            }
            return (<div className="activity-container">
                <div className="title-bar">
                    <h1 className="blueHeader">{userDetails.userSummary.firstName + " " + userDetails.userSummary.lastName}</h1>
                    <Link to={'/edituser/' + userDetails.userSummary.userID} className="action-link">
                        <Button variant="contained"
                                style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small" }}
                                disableElevation>
                            Edit
                        </Button>
                    </Link>
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
