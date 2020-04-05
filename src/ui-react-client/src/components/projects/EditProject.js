import React, {Component} from 'react';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'
import {updateProject, loadSingleProject, deleteProject} from '../../redux/actions/projectProfileActions.js';
import {loadMasterlists} from "../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import {Button} from "@material-ui/core";
import UserCard from "../users/UserCard";
import {CLIENT_DEV_ENV} from '../../config/config';
import {UserContext, getUserRoles} from "../common/userContext/UserContext";
import '../common/common.css'
import Loading from '../common/Loading';
import LoadingOverlay from 'react-loading-overlay'
import ProjectManagerCard from "../users/ProjectManagerCard";


class EditProject extends Component {
    state = {
        projectProfile: {},
        masterlist: {},
        pending: true,
        error: [],
        sending: false
    };

    componentDidMount() {
        if (CLIENT_DEV_ENV) {
            this.props.loadSingleProject(this.props.match.params.project_number, ['adminUser']);
            this.props.loadMasterlists(['adminUser']);
            this.setState((state, props) => ({
                ...this.state,
                masterlist: props.masterlist,
                projectProfile: props.projectProfile,
                pending: false
            }));
        } else {
            const userRoles = getUserRoles(this.context);
            if (Object.keys(this.props.projectProfile).length > 0 &&
                this.props.projectProfile.projectSummary.projectNumber === this.props.match.params.project_number &&
                Object.keys(this.props.masterlist).length > 0) {
                this.updateMasterlistAndProjectProfile();
            } else if (Object.keys(this.props.projectProfile).length > 0 &&
                this.props.projectProfile.projectSummary.projectNumber === this.props.match.params.project_number &&
                Object.keys(this.props.masterlist).length === 0) {
                this.props.loadMasterlists(userRoles)
                    .then(() => {
                        this.updateMasterlistAndProjectProfile();
                    })
            } else {
                const promise_masterlist = this.props.loadMasterlists(userRoles);
                const promise_singleProject = this.props.loadSingleProject(this.props.match.params.project_number, userRoles);
                Promise.all([promise_masterlist, promise_singleProject])
                    .then(() => {
                        this.updateMasterlistAndProjectProfile();
                    });
            }
        }
    }

    updateMasterlistAndProjectProfile() {
        this.setState({
            ...this.state,
            masterlist: this.props.masterlist,
            projectProfile: this.props.projectProfile,
            pending: false
        })
    }

    compare_dates = (date1,date2) => {
        if (date1<=date2) return true;
        else return false;
     }

    onSubmit = () => {
        if(this.state.error.length !== 0) {
            alert("Cannot Add Project - Please fix the errors in the form before submitting")
        } else {
            const userRoles = getUserRoles(this.context);
            this.props.updateProject(this.state.projectProfile, this.props.history, userRoles);
            this.setState({
                ...this.state,
                error: [],
                sending: true
            })
        }
    };

    onDelete = () => {
        const userRoles = getUserRoles(this.context);
        this.props.deleteProject(this.state.projectProfile.projectSummary.projectNumber, this.props.history, userRoles);
    };

    addOpening = (opening) => {
        const openings = [...this.state.projectProfile.openings, opening];
        this.setState({
            ...this.state,
            projectProfile: {
                ...this.state.projectProfile,
                openings
            }
        })
    };

    removeOpening = (opening) => {
        const openings = this.state.projectProfile.openings.filter(obj => obj !== opening);
        this.setState({
            ...this.state,
            projectProfile: {
                ...this.state.projectProfile,
                openings
            }
        })
    }

    addProjDetails = (project) => {
        let error = []
        if(project.title === "" || project.title === null){
            error = [<p className="errorMessage" key={error.length}>Error: Cannot add a project with no title</p>]
        }
        if(project.projectNumber === "") {
            error = [...error, <p className="errorMessage" key={error.length}>Error: Cannot add a project with no Project Number</p>]
        }
        if (project.projectStartDate !== "" && project.projectEndDate !== "" && !this.compare_dates(project.projectStartDate, project.projectEndDate)){            error = [...error, <p className="errorMessage" key={error.length}>Error: End date cannot be before Start Date</p>]
            error = [...error, <p className="errorMessage" key={this.state.error.length}>Error: End date cannot be before Start Date</p>]
        }
        if(error.length > 0) {
            this.setState({
                ...this.state,
                projectProfile: {
                    ...this.state.projectProfile,
                    projectSummary: {
                        ...this.state.projectProfile.projectSummary,
                        title: project.title,
                        projectNumber: project.projectNumber,
                        projectStartDate: project.projectStartDate,
                        projectEndDate: project.projectEndDate,
                        location: project.location
                    }
                },
                error: error
            })
        } else {
            this.setState({
                ...this.state,
                projectProfile: {
                    ...this.state.projectProfile,
                    projectSummary: {
                        ...this.state.projectProfile.projectSummary,
                        title: project.title,
                        projectNumber: project.projectNumber,
                        projectStartDate: project.projectStartDate,
                        projectEndDate: project.projectEndDate,
                        location: project.location
                    }
                },
                error: []
            })
        }
    };

    render() {
        if (!this.state.pending) {
            let projectProfile = this.state.projectProfile;
            var teamMembersRender = [];
            if (projectProfile.usersSummary.length > 0) {
                projectProfile.usersSummary.forEach(userSummary => {
                    teamMembersRender.push(
                        <UserCard user={userSummary}
                        canEdit={false}
                        key={teamMembersRender.length}
                        showOpeningInfo={true}
                        projectDetails={projectProfile}
                        canUnassignEditProject={true}/>
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
                                                isRemovable={true}
                                                removeOpening={(opening) => this.removeOpening(opening)}
                                                index={index}/>)
                    }
                );
            } else {
                openings.push(
                    <p key={openings.length} className="empty-statements">There are currently no openings for this project.</p>
                );
            }

            return (
                <LoadingOverlay 
                styles={{
                    overlay: (base) => ({
                      ...base,
                      background: 'rgba(169,169,169, 0.5)'
                    })
                  }} 
                  active={this.state.sending} spinner={<div className="spinner"><Loading/><p>Loading...</p></div>}>
                <div className="activity-container">
                    <h1 className="greenHeader">Edit project</h1>
                    <ProjectManagerCard projectNumber={this.state.projectProfile.projectSummary.projectNumber} userRoles={getUserRoles(this.context)} projectManager={projectProfile.projectManager} key={teamMembersRender.length}/>

                    <div className="section-container">
                        <CreateEditProjectDetails locations={this.state.masterlist.locations}
                                                  addProjDetails={(project) => this.addProjDetails(project)}
                                                  currentProject={this.state.projectProfile.projectSummary}/>
                    </div>
                    <div className="section-container">
                        <TeamRequirements disciplines={this.state.masterlist.disciplines}
                                          masterYearsOfExperience={this.state.masterlist.yearsOfExp}
                                          addOpening={(opening) => this.addOpening(opening)}
                                          isUserPage={false}
                                          startDate={this.state.projectProfile.projectSummary.projectStartDate}
                                          endDate={this.state.projectProfile.projectSummary.projectEndDate}/>
                        <div className="errorMessage">{this.state.error}</div>
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
                        <Button variant="contained"
                                style={{backgroundColor: "#EB5757", color: "#ffffff", size: "small"}}
                                disableElevation
                                onClick={() => this.onDelete()}>
                            Delete
                        </Button>
                    </div>
                    </div>
                </LoadingOverlay>
            );
        }
        else {
            return (
            <div className="activity-container">
                <Loading />
            </div>
            )
        }

    }
}

EditProject.contextType = UserContext;

const mapStateToProps = state => {
    return {
        masterlist: state.masterlist,
        projectProfile: state.projectProfile
    };
};

const mapDispatchToProps = {
    loadMasterlists,
    updateProject,
    loadSingleProject,
    deleteProject
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(EditProject);
