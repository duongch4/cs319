import React, { Component, useEffect }  from 'react';
import PropTypes from 'prop-types';
import './UserStyles.css';
import { connect } from 'react-redux';
import Openings from '../projects/Openings';
import ProjectCard from '../projects/ProjectCard'
import AvailabilityCard from './AvailabilityCard';
import {Button} from "@material-ui/core";
import { Link } from 'react-router-dom';
class UserDetails extends Component {
    state = {
        usersProfile: []
    };
 
    // XXX TODO: These (below) will eventually be sent in from the database XXX

    componentDidMount = () => {
        this.props.loadSpecificUser(this.props.match.params.user_id);
        this.setState({
            usersProfile: this.props.usersProfile
        })
    };

    render(){
        if(this.state.usersProfile.length == 0) {
            return(
                <div className="UserDetails">
                No User Available
                </div>
            )
        }
        const userDetails = this.state.usersProfile[0];
        const disciplines = [];
        userDetails.disciplines.forEach((discipline, index) => {
            disciplines.push(<Openings opening={discipline} index={index} isAssignable={false} key={disciplines.length} />)
        });
        const currentProjects = [];
        if(userDetails.currentProjects){
            userDetails.currentProjects.forEach((project, index) => {
                currentProjects.push(
                    <Link to={'/projects/' + project.projID}>
                        <ProjectCard number={index} project={project} canEditProject={false} key={currentProjects.length}/>
                    </Link>)
            }) 
        }
        const unavailability = [];
        if(userDetails.availability) {
            userDetails.availability.forEach(currentAvailability => {
                unavailability.push(<AvailabilityCard availability={currentAvailability} key={unavailability.length}/>)
            }) 
        }
        return (<div className="activity-container">
                <div className="title-bar">
                    <h1 className="blueHeader">{userDetails.name}</h1>
                    <Button variant="contained"
                            style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small" }}
                            disableElevation>
                        Edit
                    </Button>
                </div>
                <div className="section-container">
                    <p><b>Utilization:</b> {userDetails.utilization}</p>
                    <p><b>Location:</b> {userDetails.location.city}, {userDetails.location.province}</p>
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

// TODO: add availability cards
//     availability: [
//       {
//         reason: "sick",
//         start: Date,
//         end: Date
//       }
//     ],


UserDetails.propTypes = {
    usersProfile: PropTypes.array.isRequired
}

const mapStateToProps = state => {
    return {
        usersProfile: state.usersProfile,
    }
  };

  
export default connect(
    mapStateToProps
  )(UserDetails)
