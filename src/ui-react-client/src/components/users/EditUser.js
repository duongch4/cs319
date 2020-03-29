import React,{ Component } from 'react';
import { loadMasterlists } from "../../redux/actions/masterlistsActions";
import {updateSpecificUser, loadSpecificUser} from "../../redux/actions/userProfileActions";
import {connect} from "react-redux";
import TeamRequirements from "../projects/TeamRequirements";
import Openings from "../projects/Openings";
import EditUserDetails from "./EditUserDetails";
import { Button } from "@material-ui/core";
import {CLIENT_DEV_ENV} from '../../config/config';
import AvailabilityForm from './AvailabilityForm';
import AvailabilityCard from './AvailabilityCard';
import {UserContext, getUserRoles} from "../common/userContext/UserContext";
import Loading from '../common/Loading';
import LoadingOverlay from 'react-loading-overlay'
import ClipLoader from 'react-spinners/ClipLoader'

class EditUser extends Component {
    state = {
        userProfile: {},
        pending: true,
        masterlist: {},
        error: "",
        sending: false
    };

    componentDidMount() {
        if(CLIENT_DEV_ENV){
            this.props.loadMasterlists(['adminUser'])
            this.props.loadSpecificUser(this.props.match.params.user_id, ['adminUser'])
            this.setState((state, props) => ({
                ...this.state,
                masterlist: props.masterlist,
                userProfile: props.userProfile,
                pending: false,
                sending: false
            }))
        } else {
            const userRoles = getUserRoles(this.context);
            const loadMasterlistsPromise = this.props.loadMasterlists(userRoles);
            const loadSpecificUserPromise = this.props.loadSpecificUser(this.props.match.params.user_id, userRoles);
            Promise.all([loadMasterlistsPromise, loadSpecificUserPromise]).then(() => {
                this.setState({
                    ...this.state,
                    masterlist: this.props.masterlist,
                    userProfile: this.props.userProfile,
                    pending: false,
                    sending: false
                })
            });
        }
    }

    onSubmit = () => {
        const userRoles = getUserRoles(this.context);
        const userSummary = this.state.userProfile.userSummary;
        if(userSummary.firstName === "" || userSummary.lastName === ""){
            this.setState({
                ...this.state,
                error: "Unable to Save - User's Name is invalid",
            })
        } else {
            this.props.updateSpecificUser(this.state.userProfile, this.props.history, userRoles)
            this.setState({
                ...this.state,
                error: "",
                sending: true
            })
        }
       
    };

    addDisciplines = (opening) => {
        if(this.state.userProfile.disciplines && this.state.userProfile.disciplines.length === 5){
            this.setState({
                ...this.state,
                error: "Error: Cannot add more than 5 Disciplines"
            })
        } else {
            let discipline = {
                disciplineID: this.state.masterlist.disciplines[opening.discipline].disciplineID,
                discipline: opening.discipline,
                yearsOfExp: opening.yearsOfExp,
                skills: opening.skills
            };
            const disciplines = [...this.state.userProfile.disciplines, discipline];
            this.setState({
                ...this.state,
                userProfile: {
                    ...this.state.userProfile,
                    disciplines: disciplines
                },
                error: ""
            })
        }
    };

    removeDiscipline = (opening) => {
        let disciplineIDToRemove = this.state.masterlist.disciplines[opening.discipline].disciplineID;
        const disciplines = this.state.userProfile.disciplines.filter(discipline => discipline.disciplineID !== disciplineIDToRemove);
        this.setState({
            ...this.state,
            userProfile: {
                ...this.state.userProfile,
                disciplines: disciplines
            }
        })
    }

    addUserDetails = (userProfile) => {
        this.setState({
            ...this.state,
            userProfile: {
                ...this.state.userProfile,
                userSummary: {
                    ...this.state.userProfile.userSummary,
                    firstName: userProfile.firstName,
                    lastName: userProfile.lastName,
                    location: {
                        ...this.state.userProfile.location,
                        locationID: userProfile.location.locationID,
                        city: userProfile.location.city,
                        province: userProfile.location.province
                    }
                }
            }
        })
    };

    addAvailability = (date) => {
        let newDate = {
            fromDate: date.startDate,
            toDate: date.endDate,
            reason: date.reason
        }
        let availability = this.state.userProfile.availability ? [...this.state.userProfile.availability, newDate] : [newDate]
        this.setState({
            ...this.state,
            userProfile:{
                ...this.state.userProfile,
                availability, 
            }
        })
    }

    removeAvailability = (availability) => {
        let result = [];
        let first = true;
        this.state.userProfile.availability.forEach(avail => {
            if(availability.fromDate === avail.fromDate 
                && availability.toDate === avail.toDate 
                && availability.reason === avail.reason && first)
            {
                first = false;
            } else {
                result.push(avail)
            }
        })
        this.setState({
            ...this.state,
            userProfile: {
                ...this.state.userProfile,
                availability: result
            }
        })
    }

    render() {
        if (this.state.pending) {
            return (<div className="activity-container"><Loading /></div>)
        } else {
            let disciplines = [];
            if (this.state.userProfile) {
                this.state.userProfile.disciplines.forEach((discipline, index) => {
                    disciplines.push(<Openings opening={discipline}
                                               index={index}
                                               isRemovable={true}
                                               removeOpening={(opening) => this.removeDiscipline(opening)}
                                               key={disciplines.length} />)
                });
            }

            let unavailability = [];
            if(this.state.userProfile.availability && this.state.userProfile.availability.length > 0) {
                this.state.userProfile.availability.forEach(currentAvailability => {
                    unavailability.push(<AvailabilityCard availability={currentAvailability} key={unavailability.length}  removeAvailability={this.removeAvailability}/>)
                })
            } else {
                unavailability.push(<p className="empty-statements" key={unavailability.length}>This resource does not have any unavailabilities.</p>)
            }
            
            return (
                <div className="activity-container">
                <LoadingOverlay active={this.state.sending} spinner={<ClipLoader />}>
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
                                          isUserPage={true}
                                          startDate={null}
                                          endDate={null}/>
                        <p className="errorMessage">{this.state.error}</p>  
                        {disciplines} 
                        <hr />                       
                    </div>
                    <h2 className="darkGreenHeader">Unavailability</h2>
                    <div className="section-container">
                        <AvailabilityForm addAvailability={this.addAvailability}/>
                    </div>
                        {unavailability}
                    <Button variant="contained"
                            style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small" }}
                            disableElevation
                            onClick={() => this.onSubmit()}>
                        Save
                    </Button>
                    </LoadingOverlay>
                </div>
            );
        }
    }
}

EditUser.contextType = UserContext;

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
