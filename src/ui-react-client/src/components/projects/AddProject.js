import React, {Component} from 'react';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'
import {createProject} from '../../redux/actions/projectProfileActions.js';
import {loadMasterlists} from "../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import {Button} from "@material-ui/core";
import {CLIENT_DEV_ENV} from '../../config/config';
import Loading from '../common/Loading';
import {UserContext, getUserRoles} from "../common/userContext/UserContext";
import LoadingOverlay from 'react-loading-overlay'

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
                projectStartDate: new Date(),
                projectEndDate: new Date(),
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
        error: [],
        sending: false
    };

    componentDidMount() {
        if (CLIENT_DEV_ENV) {
            this.props.loadMasterlists(["adminUser"]);
            this.setState({
                ...this.state,
                masterlist: this.props.masterlist,
                pending: false
            })
        } else {
            const userRoles = getUserRoles(this.context);
            this.props.loadMasterlists(userRoles)
                .then(() => {
                    this.setState({
                        ...this.state,
                        masterlist: this.props.masterlist,
                        pending: false
                    })
                })
        }
    }

    addOpening = (opening) => {
        const openings = [...this.state.projectProfile.openings, opening];
        this.setState({
            ...this.state,
            projectProfile: {
                ...this.state.projectProfile,
                openings
            },
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

    compare_dates = (date1,date2) => {
        if (date1<=date2) return true;
        else return false;
     }

    addProjDetails = (project) => {
        let error = []
        if(project.title === "" || project.title === null){
            error = [<p className="errorMessage" key={error.length}>Error: Cannot add a project with no title</p>]
        }
        if(project.projectNumber === "") {
            error = [...error, <p className="errorMessage" key={error.length}>Error: Cannot add a project with no Project Number</p>]
        }
        if (!this.compare_dates(project.projectStartDate, project.projectEndDate)){
            error = [...error, <p className="errorMessage" key={error.length}>Error: End date cannot be before Start Date</p>]
        }
        if (project.location.province === "DEFAULT" || project.location.city === "DEFAULT") {
            error = [...error, <p className="errorMessage" key={error.length}>Error: Location is not valid</p>]
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

    onSubmit = () => {
        if(this.state.error.length !== 0) {
            alert("Cannot Add Project - Please fix the errors in the form before submitting")
        } else {
            let newProject = this.state.projectProfile;
            const userRoles = getUserRoles(this.context);
            this.props.createProject(newProject, this.props.history, userRoles);
            this.setState({
                ...this.state,
                error: [],
                sending: true,
            })
        }
    };

    render() {
        if (this.state.pending) {
            return (
                <div className="activity-container">
                    <Loading />
                </div>
            )
        } else {
            const openings = [];
            this.state.projectProfile.openings.forEach((opening, index) => {
                openings.push(<Openings key={"openings" + index} opening={opening}
                                        commitment={opening.commitmentMonthlyHours}
                                        isRemovable={true} removeOpening={(opening) => this.removeOpening(opening)}
                                        index={index}/>)
            });
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
                    <h1 className="greenHeader">Create new project</h1>
                    <CreateEditProjectDetails locations={this.props.masterlist.locations}
                                              addProjDetails={(project) => this.addProjDetails(project)}/>
                    <div className="section-container">
                        <TeamRequirements disciplines={this.props.masterlist.disciplines}
                                          masterYearsOfExperience={this.props.masterlist.yearsOfExp}
                                          addOpening={(opening) => this.addOpening(opening)}
                                          isUserPage={false}
                                          startDate={this.state.projectProfile.projectSummary.projectStartDate}
                                          endDate={this.state.projectProfile.projectSummary.projectEndDate}/>
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
                </LoadingOverlay>
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
