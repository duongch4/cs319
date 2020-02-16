import React, { Component, useEffect }  from 'react';
import PropTypes from 'prop-types';
import Openings from './Openings'
import './ProjectStyles.css';
import { connect } from 'react-redux';
import UserCard from "../users/UserCard";
import { Button } from "@material-ui/core";
import { Link } from 'react-router-dom';

class ProjectDetails extends Component {
    state = {
        openings: [],
        project: this.props.projects.filter(project => project.projID == this.props.match.params.project_id),
        users: []
    }
 
    // XXX TODO: These (below) will eventually be sent in from the database XXX

    componentDidMount = () => {
        if(this.state.project.length > 0 && this.state.project[0].openings){
            this.setState({
                openings: this.state.project[0].openings
            })
        }

        if(this.state.project.length > 0 && this.state.project[0].users) {
            this.setState({
                users: this.state.project[0].users
            })
        }
    }

    render(){
        var openingsRender = [];
        if (this.state.openings.length > 0) {
            this.state.openings.forEach((opening, index) => {
                openingsRender.push(<Openings opening={opening.discipline}
                                              index={index} commitment={opening.commitment}
                                              isAssignable={true} key={openingsRender.length} />);
                if(this.state.openings.length - 1 != index){
                    openingsRender.push(<hr key={openingsRender.length}></hr>)
                }
            });
        } else {
            openingsRender.push(<p className="empty-statements">There are currently no openings for this project.</p>);
        }

        var teamMembersRender = [];
        if (this.state.users.length > 0) {
            this.state.users.forEach(userProfile => {
                teamMembersRender.push(
                    <Link to={'/users/' + userProfile.userID}>
                        <UserCard user={userProfile} canEdit={false} key={teamMembersRender.length} />
                    </Link>)
            });
        } else {
            teamMembersRender.push(
                <p className="empty-statements">There are currently no resources assigned to this project.</p>
            )
        }

        if(this.state.project.length === 0){
            return(
                <div className="ProjectDetails">
                No Project Available
                </div>
            )
        }
        const projectDetails = this.state.project[0];
        return (
            <div className="activity-container">
                <div className="title-bar">
                    <h1 className="blueHeader">{projectDetails.name}</h1>
                    <Button variant="contained"
                            style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small" }}
                            disableElevation>
                        Edit
                    </Button>
                </div>
                <div className="section-container">
                    <p><b>Location:</b> {projectDetails.location.city}, {projectDetails.location.province}</p>
                    <p><b>Total Hours:</b> </p>
                    <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent ullamcorper aliquam enim in fermentum. Pellentesque placerat augue sit amet leo pretium, eget volutpat turpis fringilla. Integer imperdiet nec augue sed mollis. Phasellus at lectus porttitor, vestibulum nulla sed, tristique nunc.
                    </p>
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
        );
    }
}

ProjectDetails.propTypes = {
    projects: PropTypes.array.isRequired,
};

const mapStateToProps = state => {
    return {
      projects: state.projects,
      locations: state.locations,
    };
  };

const mapDispatchToProps = {
  };
  
export default connect(
    mapStateToProps,
    mapDispatchToProps,
  )(ProjectDetails);
