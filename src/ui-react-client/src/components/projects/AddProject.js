import React, {Component} from 'react';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'
import {createProject} from '../../redux/actions/projectProfileActions.js';
import {loadMasterlists} from "../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import {Button} from "@material-ui/core";
import {CLIENT_DEV_ENV} from '../../config/config';
import {isProfileLoaded, UserContext} from "../common/userContext/UserContext";

class AddProject extends Component {
    state = {
        projectProfile: {
            projectSummary: {
                title: "",
                location: {
                    locationID: 0,
                    city: "",
                    province: ""
                },
                projectStartDate: "",
                projectEndDate: "",
                projectNumber: ""
            },
            projectManager: {
                // these fields are from Azure ADs profile object which keeps track of the current user
                userID: this.props.location.state.profile.id,
                firstName: this.props.location.state.profile.givenName,
                lastName: this.props.location.state.profile.surname
            },
            usersSummary: [],
            openings: [],
        },
        masterlist: this.props.masterlist,
        pending: true,
        error: []
    };

    componentDidMount() {
        if (CLIENT_DEV_ENV) {
            this.props.loadMasterlists();
            this.setState({
                ...this.state,
                masterlist: this.props.masterlist,
                pending: false
            })
        } else {
            let user = this.context;
            if (!isProfileLoaded(user.profile)) {
                user.fetchProfile();
            } else {
                this.props.loadMasterlists(user.profile.userRoles)
                    .then(() => {
                        this.setState({
                            ...this.state,
                            masterlist: this.props.masterlist,
                            pending: false
                        })
                    })
            }
        }
        
    }

    addOpening = (opening) => {
        const openings = [...this.state.projectProfile.openings, opening];
        this.setState({
            projectProfile: {
                ...this.state.projectProfile,
                openings
            },
        })
    };

    removeOpening = (opening) => {
        const openings = this.state.projectProfile.openings.filter(obj => obj !== opening);
        this.setState({
            projectProfile: {
                ...this.state.projectProfile,
                openings
            }
        })
    }

    compare_dates = (date1,date2) => {
        if (date1<=date2) return true;
        else return false;
     }

    addProjDetails = (project) => {
        let error = []
        if(project.title === "" || project.title === null){
            error = [<p className="errorMessage" key={error.length}>Error ADD: Cannot add a project with no title</p>]
        }
        if(project.projectNumber === "") {
            error = [...error, <p className="errorMessage" key={error.length}>Error ADD: Cannot add a project with no Project Number</p>]
        }
        if (!this.compare_dates(project.projectStartDate, project.projectEndDate)){
            error = [...error, <p className="errorMessage" key={error.length}>Error ADD: End date cannot be before Start Date</p>]
        }
        if (project.location.province === "DEFAULT" || project.location.city === "DEFAULT") {
            error = [...error, <p className="errorMessage" key={error.length}>Error ADD: Location is not valid</p>]
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

    onSubmit = (userRole) => {
        if(this.state.error.length !== 0) {
            alert("Cannot Add Project - Please fix the errors in the form before submitting")
        } else {
            let newProject = this.state.projectProfile;
            let user = this.context;
            if (!isProfileLoaded(user.profile)) {
                user.fetchProfile();
            } else {
                this.props.createProject(newProject, this.props.history, user.profile.userRoles);
                this.setState({
                    ...this.state,
                    error: []
                })
            }
        }
    };

    render() {
        if (this.state.pending) {
            return (
                <div className="activity-container">
                    <h1>Loading form...</h1>
                </div>
            )
        }else {
            const openings = [];
            this.state.projectProfile.openings.forEach((opening, index) => {
                openings.push(<Openings key={"openings" + index} opening={opening}
                                        commitment={opening.commitmentMonthlyHours}
                                        isRemovable={true} removeOpening={(opening) => this.removeOpening(opening)}
                                        index={index}/>)
            });
            return (
                <div className="activity-container">
                    <h1 className="greenHeader">Create new project</h1>
                    <CreateEditProjectDetails locations={this.props.masterlist.locations}
                                              addProjDetails={(project) => this.addProjDetails(project)}/>
                    <div className="section-container">
                        <TeamRequirements disciplines={this.props.masterlist.disciplines}
                                          masterYearsOfExperience={this.props.masterlist.yearsOfExp}
                                          addOpening={(opening) => this.addOpening(opening)}
                                          isUserPage={false}/>
                        {this.state.error}
                        <hr/>
                       
                        {openings}
                    </div>
                    <div className="section-container">
                        <Button variant="contained"
                                style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small"}}
                                disableElevation
                                onClick={() => this.onSubmit()}>Save</Button>
                    </div>
                </div>
            )
        }
    }
}

AddProject.contextType = UserContext;

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
