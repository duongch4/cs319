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
import UserCard from "../users/UserCard";
import { Link } from 'react-router-dom';

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
    },
    users: []
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

    if(currentProject[0].users) {
        this.setState({
            users: currentProject[0].users
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
    var teamMembersRender = [];
    if (this.state.users.length > 0) {
        this.state.users.forEach(userProfile => {
            teamMembersRender.push(
                <Link to={'/users/' + userProfile.userID}>
                    <UserCard user={userProfile} canEdit={false} key={teamMembersRender.length} />
                </Link>)
        })
    } else {
        teamMembersRender.push(
            <p className="empty-statements">There are currently no resources assigned to this project.</p>
        )
    }

    const openings = [];
      this.state.project.openings.forEach((opening, index) => {
        openings.push(<Openings key = {index} opening={opening.discipline}
                                commitment={opening.commitment}
                                index={index}/>)
    }
  );
      return (
          <div className="activity-container">
            <h1 className="greenHeader">Edit project</h1>
            <div className="section-container">
              <CreateEditProjectDetails locations={this.props.locations}
                                addProjDetails={(project) => this.addProjDetails(project)}
                                currentProject={this.state}/>
            </div>
            <div className="section-container">
              <TeamRequirements disciplines={this.props.disciplines}
                                masterYearsOfExperience={this.props.masterYearsOfExperience}
                                addOpening={(opening) => this.addOpening(opening)}/>
              <hr />
              {openings}
            </div>
            <div className="section-container">
            <h2 className="greenHeader">The Team</h2>
            {teamMembersRender}
            </div>
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
