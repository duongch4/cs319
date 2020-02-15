import React, { Component, useEffect }  from 'react';
import PropTypes from 'prop-types';
import './UserStyles.css';
import { connect } from 'react-redux';
import Openings from '../projects/Openings';
import { loadSpecificUser } from '../../redux/actions/userProfileActions';
import ProjectCard from '../projects/ProjectCard'
import AvailabilityCard from './AvailabilityCard';
class UserDetails extends Component {
    state = {
        usersProfile: []
    }
 
    // XXX TODO: These (below) will eventually be sent in from the database XXX

    componentDidMount = () => {
        this.props.loadSpecificUser(this.props.match.params.user_id)
        this.setState({
            usersProfile: this.props.usersProfile
        })
    }

    render(){
        if(this.state.usersProfile.length == 0) {
            return(
                <div className="UserDetails">
                No User Available
                </div>
            )
        }
        const userDetails = this.state.usersProfile[0]
        const disciplines = []
        userDetails.disciplines.forEach((discipline, index) => {
            disciplines.push(<Openings opening={discipline} index={index}  key={disciplines.length} />)
        })
        const currentProjects = []
        if(userDetails.currentProjects){
            userDetails.currentProjects.forEach((project, index) => {
                currentProjects.push(<ProjectCard number={index} project={project} isProjectList={false} key={currentProjects.length}/>)
            }) 
        }
        const unavailability = []
        if(userDetails.availability) {
            userDetails.availability.forEach(currentAvailability => {
                unavailability.push(<AvailabilityCard availability={currentAvailability} key={unavailability.length}/>)
            }) 
        }
        return (
            <div className="activity-container">
                <div className="UserDetails">
                    <h1 className="blueHeader">{userDetails.name}</h1>
                    <p>Utilization: {userDetails.utilization}</p>
                    <p>Location: {userDetails.location.city}, {userDetails.location.province}</p>
                    <h2 className="greenHeader">Discipline & Skills</h2>
                    {disciplines}
                    <h2 className="greenHeader">Current Projects</h2>
                    {currentProjects}
                    <h2 className="greenHeader">Unavailability</h2>
                    {unavailability}
                </div>
            </div>
        );
    }
};

// TODO: add availability cards
//     availability: [
//       {
//         reason: "sick",
//         start: Date,
//         end: Date
//       }
//     ],


UserDetails.propTypes = {
    usersProfile: PropTypes.array.isRequired,
};

const mapStateToProps = state => {
    return {
        usersProfile: state.usersProfile,
    };
  };

const mapDispatchToProps = {
    loadSpecificUser,
  };
  
export default connect(
    mapStateToProps,
    mapDispatchToProps,
  )(UserDetails);
