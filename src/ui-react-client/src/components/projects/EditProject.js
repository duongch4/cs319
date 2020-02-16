import React,{ Component } from 'react';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'
import { updateProject, loadProjects } from '../../redux/actions/projectsActions.js';
import { loadDisciplines } from '../../redux/actions/disciplinesActions.js';
import { loadLocations} from '../../redux/actions/locationsActions.js';
import { loadExperiences } from '../../redux/actions/experienceActions.js';
import { connect } from 'react-redux';

class EditProject extends Component {
  state = {
      // TODO: Depending how you implement this, you may or may not need the following lines:
    // project: {
    //   projID: null,
    //   name: "",
    //   location: {
    //     city: "",
    //     province: ""
    //   },
    //   startDate: "",
    //   endDate: "",
    //   openings: [],      
    // }
  }

  componentDidMount(){
    this.props.loadDisciplines();
    // this.props.disciplines holds the master disciplines Map now
    this.props.loadLocations();
    // this.props.locations hold the master locations now
    this.props.loadExperiences();
    // this.props.masterYearsOfExperience holds the master list of experiences
    this.props.loadProjects()
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
  
  render(){
      return (
      <div>
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
