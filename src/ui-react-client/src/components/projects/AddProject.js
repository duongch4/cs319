import React,{ Component } from 'react';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'
import { createProject } from '../../redux/actions/projectProfileActions.js';
import { loadMasterlists } from "../../redux/actions/masterlistsActions";
import { connect } from 'react-redux';
import { Button } from "@material-ui/core";

class AddProject extends Component {
  state = {
    projectProfile: {
      projectSummary: {
        title: "",
        location: {
          city: "",
          province: ""
        },
        projectStartDate: "",
        projectEndDate: "",
        projectNumber: ""
      },
      projectManager: {
        userID: null,
        firstName: "",
        lastName: ""
      },
      usersSummary: [],
      openings: [],
    }
  };

  componentDidMount(){
    this.props.loadMasterlists()
    //
  }

  addOpening = (opening) => {
    const openings = [...this.state.projectProfile.openings, opening]
    this.setState({
      projectProfile:{
        ...this.state.projectProfile,
        openings
      }
    })
  };

  addProjDetails = (project) => {
    this.setState({
       projectProfile: {
         ...this.state.projectProfile,
         projectSummary: {
           ...this.state.projectProfile.projectSummary,
           title: project.title,
           projectStartDate: project.projectStartDate,
           projectEndDate: project.projectEndDate,
           location: project.location
         }
       }
    })
  };

  onSubmit = () => {
     this.props.createProject(this.state.project);
     this.setState({
       projectProfile: {
         projectSummary: {
           title: "",
           location: {
             city: "",
             province: ""
           },
           projectStartDate: "",
           projectEndDate: "",
           projectNumber: ""
         },
         projectManager: {
           userID: null,
           firstName: "",
           lastName: ""
         },
         usersSummary: [],
         openings: [],
       }
     })
  };

  render(){
    const openings = [];
    this.state.projectProfile.openings.forEach((opening, index) => {
      openings.push(<Openings key={"openings" + index} opening={opening.discipline}
                              commitment={opening.commitmentMonthlyHours}
                              index={index}/>)
    });
    return (
      <div className="activity-container">
        <h1 className="greenHeader">Create new project</h1>
        <CreateEditProjectDetails locations={this.props.masterlist.locations}
                          addProjDetails={(project) => this.addProjDetails(project)}/>
        <TeamRequirements disciplines={this.props.masterlist.disciplines}
                          masterYearsOfExperience={this.props.masterlist.yearsOfExp}
                          addOpening={(opening) => this.addOpening(opening)}/>
        {openings}
        <Button variant="contained"
                style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small" }}
                disableElevation
                onClick={() => this.onSubmit()}>Save</Button>
      </div>
    );
  }
};

const mapStateToProps = state => {
  return {
    masterlist: state.masterlist,
  };
};

const mapDispatchToProps = {
  createProject,
  loadMasterlists
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(AddProject);
