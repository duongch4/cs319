import React, { Component, useEffect }  from 'react';
import PropTypes from 'prop-types';
import Openings from './Openings'
import './ProjectStyles.css';
import { connect } from 'react-redux';

class ProjectDetails extends Component {
    state = {
        openings: [],
        project: this.props.projects.filter(project => project.projID == this.props.match.params.project_id)
    }
 
    // XXX TODO: These (below) will eventually be sent in from the database XXX

    componentDidMount = () => {
        if(this.state.project.length > 0 && this.state.project[0].openings){
            this.setState({
                openings: this.state.project[0].openings
            })
        }
    }

    render(){
        var openingsRender = []
        this.state.openings.forEach((opening, index) => {
            openingsRender.push(<Openings opening={opening.discipline} index={index} commitment={opening.commitment} key={openingsRender.length} />)
            if(this.state.openings.length - 1 != index){
                openingsRender.push(<hr key={openingsRender.length}></hr>)
            }
        })
        if(this.state.project.length === 0) {
            return(
                <div className="ProjectDetails">
                No Project Available
                </div>
            )
        }
        const projectDetails = this.state.project[0]
        return (
            <div className="activity-container">
                <div className="ProjectDetails">
                    <h1 className="blueHeader">{projectDetails.name}</h1>
                    <p>Location: {projectDetails.location.city}, {projectDetails.location.province}</p>
                    <p>Total Hours: </p>
                    <p>Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent ullamcorper aliquam enim in fermentum. Pellentesque placerat augue sit amet leo pretium, eget volutpat turpis fringilla. Integer imperdiet nec augue sed mollis. Phasellus at lectus porttitor, vestibulum nulla sed, tristique nunc.
                    </p>
                    <h2 className="greenHeader">The Team</h2>
                    <h3>PlaceHolder for Profile Brief from Kaye's People pages</h3>
                    <h2 className="greenHeader">Openings</h2>
                    {openingsRender}
                </div>
            </div>
        );
    }
};

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
