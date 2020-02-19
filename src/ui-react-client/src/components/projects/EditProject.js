import React,{ Component } from 'react';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'
import { updateProject, loadSingleProject} from '../../redux/actions/projectsActions.js';
import {loadMasterlists} from "../../redux/actions/masterlistsActions";
import { connect } from 'react-redux';
import { Button } from "@material-ui/core";
import UserCard from "../users/UserCard";


class EditProject extends Component {
  state = {
    projectProfile: {
        projectSummary: {
            title: "",
            location: {
                province: "",
                city: ""
            },
            projectStartDate: "",
            projectEndDate: "",
            projectNumber: ""
        },
        projectManager: {
            userID: 0,
            firstName: "",
            lastName: ""
        },
        usersSummary: [],
        openings:[]
    }
  };

  componentDidMount(){
      this.props.loadMasterlists();
      // this.props.masterlists holds the master list of experiences, disciplines and locations
      this.props.loadSingleProject(this.props.match.params.project_id);
      var currentProject = this.props.projectProfile;

    if(currentProject){
        this.setState({
            projectProfile: currentProject
            // state now holds the current project
        })
    }
  }

  onSubmit = () => {
     this.props.updateProject(this.state.projectProfile)
  }

    addOpening = (opening) => {
      const openings = [...this.state.projectProfile.openings, opening];
      this.setState({
        projectProfile:{
          ...this.state.projectProfile,
          openings
        }
      })
    }

    addProjDetails = (project) => {
      this.setState({
         projectProfile: {
           ...this.state.projectProfile,
           projectSummary: {
               ...this.state.projectProfile.projectSummary,
               title: project.projectSummary.title,
               projectNumber: project.projectSummary.projectNumber,
               projectStartDate: project.projectSummary.projectStartDate,
               projectEndDate: project.projectSummary.projectEndDate,
               location: project.projectSummary.location
           }
         }
      })
    }
  render(){
    var teamMembersRender = [];
    console.log(this.props);
    if (this.props.projectProfile.usersSummary.length > 0) {
        this.props.projectProfile.usersSummary.forEach(userSummary => {
            teamMembersRender.push(
                <UserCard user={userSummary} canEdit={false} key={teamMembersRender.length} />
                )
        })
    } else {
        teamMembersRender.push(
            <p className="empty-statements">There are currently no resources assigned to this project.</p>
        )
    }

    const openings = [];
      this.props.projectProfile.openings.forEach((opening, index) => {
        openings.push(<Openings key = {index} opening={opening}
                                commitment={opening.commitmentMonthlyHours}
                                index={index}/>)
    }
  );
      console.log(this.props);
      return (
          <div className="activity-container">
            <h1 className="greenHeader">Edit project</h1>
            <div className="section-container">
              <CreateEditProjectDetails locations={this.props.masterlist.locations}
                                addProjDetails={(project) => this.addProjDetails(project)}
                                currentProject={this.props.projectProfile.projectSummary}/>
            </div>
            <div className="section-container">
              <TeamRequirements disciplines={this.props.masterlist.disciplines}
                                masterYearsOfExperience={this.props.masterlist.yearsOfExp}
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
    console.log(state);
  return {
    masterlist: state.masterlist,
    projectProfile: state.projects.projectProfile
  };
};

const mapDispatchToProps = {
  loadMasterlists,
  updateProject,
  loadSingleProject
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(EditProject);
