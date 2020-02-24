import React, {Component} from 'react';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'
import {updateProject, loadSingleProject} from '../../redux/actions/projectProfileActions.js';
import {loadMasterlists} from "../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import {Button} from "@material-ui/core";
import UserCard from "../users/UserCard";


class EditProject extends Component {
    state = {
        projectProfile: {},
        masterlist: {},
        pending: true
    };

    componentDidMount() {
        var promise_masterlist = this.props.loadMasterlists();
        var promise_singleProject = this.props.loadSingleProject(this.props.match.params.project_number);
        Promise.all([promise_masterlist, promise_singleProject])
           .then(() => {
               this.setState(
                   {
                       ...this.state,
                       masterlist: this.props.masterlist,
                       projectProfile: this.props.projectProfile,
                       pending: false
                   })});
    }

    onSubmit = () => {
        this.props.updateProject(this.state.projectProfile);
    };

    addOpening = (opening) => {
        const openings = [...this.state.projectProfile.openings, opening];
        this.setState({
            projectProfile: {
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
                    title: project.projectSummary.title,
                    projectNumber: project.projectSummary.projectNumber,
                    projectStartDate: project.projectSummary.projectStartDate,
                    projectEndDate: project.projectSummary.projectEndDate,
                    location: project.projectSummary.location
                }
            }
        })
    };

    render() {
        if (!this.state.pending) {
            let projectProfile = this.state.projectProfile;
            var teamMembersRender = [];
            if (projectProfile.usersSummary.length > 0) {
                projectProfile.usersSummary.forEach(userSummary => {
                    teamMembersRender.push(
                        <UserCard user={userSummary} canEdit={false} key={teamMembersRender.length}/>
                    )
                })
            } else {
                teamMembersRender.push(
                    <p key={teamMembersRender.length} className="empty-statements">There are currently no resources assigned to this project.</p>
                )
            }
    
            const openings = [];
            if (projectProfile.openings.length > 0) {
                projectProfile.openings.forEach((opening, index) => {
                        openings.push(<Openings key={index} opening={opening}
                                                commitment={opening.commitmentMonthlyHours}
                                                index={index}/>)
                    }
                );
            } else {
                openings.push(
                    <p key={openings.length} className="empty-statements">There are currently no openings for this project.</p>
                );
            }

            return (
                <div className="activity-container">
                    <h1 className="greenHeader">Edit project</h1>
                    <div className="section-container">
                        <CreateEditProjectDetails locations={this.state.masterlist.locations}
                                                  addProjDetails={(project) => this.addProjDetails(project)}
                                                  currentProject={this.state.projectProfile.projectSummary}/>
                    </div>
                    <div className="section-container">
                        <TeamRequirements disciplines={this.state.masterlist.disciplines}
                                          masterYearsOfExperience={this.state.masterlist.yearsOfExp}
                                          addOpening={(opening) => this.addOpening(opening)}/>
                        <hr/>
                        {openings}
                    </div>
                    <div className="section-container">
                        <h2 className="darkGreenHeader">The Team</h2>
                        {teamMembersRender}
                    </div>
                    <div className="section-container">
                        <Button variant="contained"
                                style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small"}}
                                disableElevation
                                onClick={() => this.onSubmit()}>
                            Save
                        </Button>
                    </div>
                </div>
            );
        }
        else {
            return <div>Loading</div>
        }
 
    }
}

const mapStateToProps = state => {
    return {
        masterlist: state.masterlist,
        projectProfile: state.projectProfile
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
