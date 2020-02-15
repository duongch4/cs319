import React,{ Component } from 'react';
import { Link } from 'react-router-dom';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'
import { loadDisciplines } from '../../redux/actions/disciplinesActions.js';
import { createProject } from '../../redux/actions/projectsActions.js';
import { loadLocations} from '../../redux/actions/locationsActions.js';
import { loadExperiences } from '../../redux/actions/experienceActions.js';
import { connect } from 'react-redux';

class AddProject extends Component {
  state = {
    project: {
      projID: null,
      name: "",
      location: {
        city: "",
        province: ""
      },
      startDate: "",
      endDate: "",
      openings: [],
    }
  }

  componentDidMount(){
    this.props.loadDisciplines();
    // this.props.disciplines holds the master disciplines Map now
    this.props.loadLocations();
    // this.props.locations hold the master locations now
    this.props.loadExperiences();
    // this.props.masterYearsOfExperience holds the master list of experiences
  }

  addOpening = (opening) => {
    const openings = [...this.state.project.openings, opening]
    this.setState({
      project:{
        ...this.state.project,
        openings
      }
    })
  }

  addProjDetails = (project) => {
    this.setState({
       project: {
         ...this.state.project,
         name: project.name,
         projID: project.projID,
         startDate: project.startDate,
         endDate: project.endDate,
         location: project.location
       }
    })
  }

  onSubmit = () => {
     this.props.createProject(this.state.project)
  }
  render(){
    const openings = []
    this.state.project.openings.forEach((opening, index) => {
      openings.push(<Openings opening={opening.discipline}
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
        <button onClick={() => this.onSubmit()}></button>
      </div>
    );
  }
};

const mapStateToProps = state => {
  return {
    disciplines: state.disciplines,
    locations: state.locations,
    masterYearsOfExperience: state.masterYearsOfExperience,
  };
};

const mapDispatchToProps = {
  loadDisciplines,
  loadLocations,
  createProject,
  loadExperiences,
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(AddProject);
