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

class UserDetails extends Component {
    state = {
        userProfile: {
            userSummary: {
                userID: 0,
                firstName: "",
                lastName: "",
                location: {
                    city: "",
                    state: ""
                },
                utilization: 0,
                resourceDiscipline: {
                    discipline: "",
                    yearsOfExp: ""
                },
                isConfirmed: false
            },
            currentProjects:[],
            availability: [],
            disciplines: []
        }
    };

    componentDidMount() {
        if (Object.keys(this.props.userProfile).length === 0) {
            this.props.loadSpecificUser(this.props.match.params.user_id)
                .then(
                    this.setState({ userProfile: this.props.userProfile })
                )
        }
    };

    render(){
        let userDetails = this.state.userProfile;
        let disciplines = [];
        if (userDetails.disciplines) {
            userDetails.disciplines.forEach((discipline, index) => {
                disciplines.push(<Openings opening={discipline} index={index} isAssignable={false} key={disciplines.length} />)
            });
        }

        const currentProjects = [];
        if(userDetails.currentProjects){
            userDetails.currentProjects.forEach((project, index) => {
                currentProjects.push(
                    <Link to={'/projects/' + project.projectNumber}>
                        <ProjectCard number={index} project={project} canEditProject={false} key={currentProjects.length}/>
                    </Link>)
            }) 
        }
        let unavailability = [];
        if(userDetails.availability) {
            userDetails.availability.forEach(currentAvailability => {
                unavailability.push(<AvailabilityCard availability={currentAvailability} key={unavailability.length}/>)
            }) 
        }
        return (<div className="activity-container">
                <div className="title-bar">
                    <h1 className="blueHeader">{userDetails.userSummary.firstName + " " + userDetails.userSummary.lastName}</h1>
                    <Button variant="contained"
                            style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small" }}
                            disableElevation>
                        Edit
                    </Button>
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

UserDetails.propTypes = {
    userProfile: PropTypes.object.isRequired
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
