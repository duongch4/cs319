import React,{ Component } from 'react';
import { loadMasterlists } from "../../redux/actions/masterlistsActions";
import {updateSpecificUser, loadSpecificUser} from "../../redux/actions/userProfileActions";
import {connect} from "react-redux";
import TeamRequirements from "../projects/TeamRequirements";
import Openings from "../projects/Openings";
import EditUserDetails from "./EditUserDetails";
import { Button } from "@material-ui/core";

class EditUser extends Component {
    state = {
        userProfile: {},
        pending: true,
        masterlist: {}
    };

    componentDidMount() {
        this.props.loadMasterlists()
            .then(() => {
                this.setState({
                    ...this.state,
                    masterlist: this.props.masterlist
                })
            });
        this.props.loadSpecificUser(this.props.match.params.user_id)
            .then(() => {
                this.setState({
                    ...this.state,
                    userProfile: this.props.userProfile,
                    pending: false
                })
            });
    }

    onSubmit = () => {
        this.props.updateSpecificUser(this.state.userProfile)
    };

    addDisciplines = (opening) => {
        let discipline = {
            discipline: opening.discipline,
            yearsOfExp: opening.yearsOfExp,
            skills: opening.skills
        };
        const disciplines = [...this.state.userProfile.disciplines, discipline];
        this.setState({
            userProfile: {
                ...this.state.userProfile,
                disciplines: disciplines
            }
        })
    };

    addUserDetails = (userProfile) => {
        this.setState({
            userProfile: {
                ...this.state.userProfile,
                userSummary: {
                    ...this.state.userProfile.userSummary,
                    firstName: userProfile.firstName,
                    lastName: userProfile.lastName,
                    location: {
                        ...this.state.userProfile.location,
                        city: userProfile.location.city,
                        province: userProfile.location.province
                    }
                }
            }
        })
    };

    render() {
        if (this.state.pending) {
            return (<div className="activity-container">
                <h1>Loading user data...</h1>
            </div>);
        } else {
            let disciplines = [];
            if (this.props.userProfile) {
                this.state.userProfile.disciplines.forEach((discipline, index) => {
                    disciplines.push(<Openings opening={discipline}
                                               index={index}
                                               key={disciplines.length} />)
                });
            }
            return (
                <div className="activity-container">
                    <h1 className="greenHeader">Edit user</h1>
                    <div className="section-container">
                        <EditUserDetails userProfile={this.state.userProfile.userSummary}
                                         addUserDetails={(userProfile) => this.addUserDetails(userProfile)}
                                         locations={this.props.masterlist.locations}/>
                    </div>
                    <div className="section-container">
                        <TeamRequirements disciplines={this.props.masterlist.disciplines}
                                          masterYearsOfExperience={this.props.masterlist.yearsOfExp}
                                          addOpening={(opening) => this.addDisciplines(opening)}
                                          isUserPage={true}/>
                        <hr />
                        {disciplines}
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
}

const mapStateToProps = state => {
    return {
        masterlist: state.masterlist,
        userProfile: state.userProfile
    };
};

const mapDispatchToProps = {
    loadMasterlists,
    loadSpecificUser,
    updateSpecificUser,
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(EditUser);
