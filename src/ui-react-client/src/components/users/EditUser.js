import React,{ Component } from 'react';
import Openings from './Openings'
import TeamRequirements from './TeamRequirements'
import { loadDisciplines } from '../../redux/actions/disciplinesActions.js';
import { loadLocations} from '../../redux/actions/locationsActions.js';
import { loadExperiences } from '../../redux/actions/experienceActions.js';
import { connect } from 'react-redux';

class EditProject extends Component {
  state = {
      openings: []
  }

  componentDidMount(){
    this.props.loadDisciplines();
    // this.props.disciplines holds the master disciplines Map now
    this.props.loadLocations();
    // this.props.locations hold the master locations now
    this.props.loadExperiences();
    // this.props.masterYearsOfExperience holds the master list of experiences
    this.props.loadProjects()
    var currentUser = this.props.users.filter(user => user.userID == this.props.match.params.user_id)
    if(currentProject){
        this.setState({
            user: currentUser[0]
            // state now holds the current project
        })
    }
  }

  onSubmit = () => {
      // TODO: The updateUser needs to be fed through redux which isn't done yet
    //  this.props.updateUser(this.state.project)
  }
  
  addOpening = (opening) => {
    const openings = [...this.state.openings, opening]
    this.setState({
      project:{
        ...this.state.project,
        openings
      }
    })
  }


  render(){
      return (
      <>
      <TeamRequirements 
        disciplines={this.props.disciplines}
        masterYearsOfExperience={this.props.masterYearsOfExperience}
        addOpening={(opening) => this.addOpening(opening)} />
      </>
    );
  }
};

const mapStateToProps = state => {
  return {
    disciplines: state.disciplines,
    locations: state.locations,
    masterYearsOfExperience: state.masterYearsOfExperience,
    users: state.users
  };
};

const mapDispatchToProps = {
  loadDisciplines,
  loadLocations,
  loadExperiences,
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(EditProject);
