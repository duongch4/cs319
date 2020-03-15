import React, {Component} from 'react';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'
import {createProject} from '../../redux/actions/projectProfileActions.js';
import {loadMasterlists} from "../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import {Button} from "@material-ui/core";
import {CLIENT_DEV_ENV} from '../../config/config';

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
                userID: 2,
                firstName: "Charles",
                lastName: "Bartowski"
            },
            usersSummary: [],
            openings: [],
        },
        masterlist: this.props.masterlist,
        pending: true
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
            this.props.loadMasterlists()
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
                    title: project.title,
                    projectStartDate: project.projectStartDate,
                    projectEndDate: project.projectEndDate,
                    location: project.location,
                    projectNumber: project.projectNumber
                }
            }
        })
    };

    onSubmit = () => {
        let newProject = this.state.projectProfile;
        this.props.createProject(newProject, this.props.history);
    };

    render() {
        if (this.state.pending) {
            return (
                <div className="activity-container">
                    <h1>Loading form...</h1>
                </div>
            )
        } else {
            const openings = [];
            this.state.projectProfile.openings.forEach((opening, index) => {
                openings.push(<Openings key={"openings" + index} opening={opening}
                                        commitment={opening.commitmentMonthlyHours}
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
                                          addOpening={(opening) => this.addOpening(opening)}/>
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
