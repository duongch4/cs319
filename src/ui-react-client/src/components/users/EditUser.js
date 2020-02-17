import React,{ Component } from 'react';
import {loadDisciplines} from "../../redux/actions/disciplinesActions";
import {loadExperiences} from "../../redux/actions/experienceActions";
import {updateSpecificUser, loadSpecificUser} from "../../redux/actions/userProfileActions";
import {connect} from "react-redux";
import TeamRequirements from "../projects/TeamRequirements";
import Openings from "../projects/Openings";
import EditUserDetails from "./EditUserDetails";

class EditUser extends Component {
    state = {
        usersProfile: {
            userID: null,
            name: "",
            discipline: {},
            position: "",
            utilization: 0,
            location: {city: "", province: ""},
            currentProjects: [],
            availability: [],
            disciplines:[]
        }
    };

    componentDidMount() {
        this.props.loadDisciplines();
        // this.props.disciplines holds the master disciplines Map now
        this.props.loadExperiences();
        // this.props.masterYearsOfExperience holds the master list of experiences
        this.props.loadSpecificUser();
        var currentUser = this.props.usersProfile.filter(userProfile => userProfile.userID == this.props.match.params.user_id);
        if (currentUser) {
            this.setState({
                usersProfile: currentUser[0]
            })
        }
    }

    onSubmit = () => {
        this.props.updateSpecificUser(this.state.usersProfile)
    };

    addDisciplines = (opening) => {
        const disciplines = [...this.state.usersProfile.disciplines, opening.discipline];
        this.setState({
            usersProfile:{
                ...this.state.usersProfile,
                disciplines
            }
        })
    };

    addUserDetails = (userProfile) => {
        this.setState({
            usersProfile: {
                ...this.state.usersProfile,
                name: userProfile.name,
                location: userProfile.location
            }
        })
    }

    render() {
        const disciplines = [];
        if (this.state.usersProfile) {
            this.state.usersProfile.disciplines.forEach((discipline, index) => {
                disciplines.push(<Openings opening={discipline}
                                           index={index}
                                           key={disciplines.length} />)
            });
        }

        return (
            <div className="activity-container">
                <h1 className="greenHeader">Edit user</h1>
                <EditUserDetails userProfile={this.state.usersProfile}
                                 addUserDetails={(userProfile) => this.addUserDetails(userProfile)}
                                 locations={this.props.locations}/>
                <TeamRequirements disciplines={this.props.disciplines}
                                  masterYearsOfExperience={this.props.masterYearsOfExperience}
                                  addOpening={(opening) => this.addDisciplines(opening)}
                                  isUserPage={true}/>
                {disciplines}
                <button onClick={() => this.onSubmit()}>Save</button>
            </div>
        );
    }
}

const mapStateToProps = state => {
    return {
        disciplines: state.disciplines,
        locations: state.locations,
        masterYearsOfExperience: state.masterYearsOfExperience,
        usersProfile: state.usersProfile
    };
};

const mapDispatchToProps = {
    loadDisciplines,
    loadExperiences,
    updateSpecificUser,
    loadSpecificUser
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(EditUser);
