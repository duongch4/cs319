import React,{ Component } from 'react';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'
import { createProject } from '../../redux/actions/projectsActions.js';
import { loadMasterlists } from "../../redux/actions/masterlistsActions";
import { connect } from 'react-redux';
import {Button} from "@material-ui/core";

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
    const openings = [...this.state.project.openings, opening]
    this.setState({
      project:{
        ...this.state.project,
        openings
      }
    })
  };

  addProjDetails = (project) => {
    this.setState({
       project: {
         ...this.state.project,
         name: project.name,
         startDate: project.startDate,
         endDate: project.endDate,
         location: project.location
       }
    })
  };

  onSubmit = () => {
     this.props.createProject(this.state.project);
     this.setState({
       project: {
         projID: this.state.project.projID + 1,
         name: "",
         location: {
           city: "",
           province: ""
         },
         startDate: "",
         endDate: "",
         openings: [],
       }})
  };

  render(){
    const openings = [];
    this.state.project.openings.forEach((opening, index) => {
      openings.push(<Openings key={"openings" + index} opening={opening.discipline}
                              commitment={opening.commitment}
                              index={index}/>)
    });
    return (
      <div className="activity-container">
        <h1 className="greenHeader">Create new project</h1>
        <CreateEditProjectDetails locations={this.props.locations}
                          addProjDetails={(project) => this.addProjDetails(project)}/>
        <TeamRequirements disciplines={this.props.disciplines}
                          masterYearsOfExperience={this.props.masterYearsOfExperience}
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
