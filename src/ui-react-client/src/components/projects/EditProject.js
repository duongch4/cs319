import React,{ Component } from 'react';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'
import { updateProject, loadProjects } from '../../redux/actions/projectsActions.js';
import { loadDisciplines } from '../../redux/actions/disciplinesActions.js';
import { loadLocations} from '../../redux/actions/locationsActions.js';
import { loadExperiences } from '../../redux/actions/experienceActions.js';
import { connect } from 'react-redux';
import { Button } from "@material-ui/core";

class EditProject extends Component {
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
    this.props.loadProjects();
    var currentProject = this.props.projects.filter(project => project.projID == this.props.match.params.project_id)
    if(currentProject){
        this.setState({
            project: currentProject[0]
            // state now holds the current project
        })
    }
  }

  onSubmit = () => {
     this.props.updateProject(this.state.project)
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
  render(){
    const openings = [];
      this.state.project.openings.forEach((opening, index) => {
        openings.push(<Openings opening={opening.discipline}
                                commitment={opening.commitment}
                                index={index}/>)
    });
      return (
          <div className="activity-container">
            <h1 className="greenHeader">Edit project</h1>
            <CreateEditProjectDetails locations={this.props.locations}
                              addProjDetails={(project) => this.addProjDetails(project)}
                              currentProject={this.state}/>
            <TeamRequirements disciplines={this.props.disciplines}
                              masterYearsOfExperience={this.props.masterYearsOfExperience}
                              addOpening={(opening) => this.addOpening(opening)}/>
            {openings}
            <Button variant="contained"
                    style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small" }}
                    disableElevation
                    onClick={() => this.onSubmit()}>
              Save
            </Button>
          </div>
    );
  }
}

const mapStateToProps = state => {
  return {
    disciplines: state.disciplines,
    locations: state.locations,
    masterYearsOfExperience: state.masterYearsOfExperience,
    projects: state.projects
  };
};

const mapDispatchToProps = {
  loadDisciplines,
  loadLocations,
  updateProject,
  loadExperiences,
  loadProjects
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(EditProject);
