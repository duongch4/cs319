import React, {Component} from 'react';
import PropTypes from 'prop-types';
import '../../projects/ProjectStyles.css';
import {connect} from 'react-redux';
import { updateSpecificUser } from '../../../redux/actions/userProfileActions';
import { updateProject } from '../../../redux/actions/projectProfileActions';
import {createAssignOpenings} from '../../../redux/actions/forecastingActions';
import {Button} from "@material-ui/core";
import Fab from '@material-ui/core/Fab';
import AddIcon from '@material-ui/icons/Add';
import { Link } from 'react-router-dom';
import Search from '../search/Search.js';
import UserCard from '../../users/UserCard.js';
import {CLIENT_DEV_ENV} from '../../../config/config';
import {loadMasterlists} from "../../../redux/actions/masterlistsActions";
import Loading from '../Loading';
import {UserContext, getUserRoles} from "../userContext/UserContext";

class Forecasting extends Component {
    state = {
        users: null,
        openingId: null,
        projectProfile: {
            projectSummary: {},
            projectManager: {},
            usersSummary: [],
            openings: []
        },
        pending: true,
        masterlist: {},
        prevProps: null,
    };

    componentDidMount() {
        this.setState({
            ...this.state,
            projectProfile: this.props.projectProfile,
            openingId: this.props.match.params.positionID,
            users: this.props.users,
            pending: false
        })
    }
    componentDidUpdate(prevProps){
        this.state.prevProps = prevProps;
    }

    render() {
        if (!this.state.pending) {
            if(Object.keys(this.state.projectProfile).length === 0){
                this.props.history.goBack();
                return <div></div>;
            }
            let project_title = this.state.projectProfile.projectSummary ? this.state.projectProfile.projectSummary.title : "";
            let users = this.state.users;

            let openings = this.state.projectProfile.openings;
            let selectedOpening = {};

            openings.forEach((opening, index) => {
                if (opening.positionID == this.state.openingId) {
                    selectedOpening = opening;
                }
            });

            let discipline_name = selectedOpening.discipline;
            const project_number = this.state.projectProfile.projectSummary.projectNumber;
            const userRoles = getUserRoles(this.context);

            return (
                <div>
                    <div className="activity-container-no-padding">
                        <div className="title-bar">
                            <h2 className="blueHeader">Project: {project_title}</h2>
                        </div>
                        <div className="title-bar">
                            <h1 className="greenHeader">{"Assign: " + discipline_name}</h1>
                        </div>
                    </div>
                    <Search onDataFetched={this.handleResultChange}
                            masterlist={this.state.masterlist}
                            isAssignable={true}
                            openingId={this.state.openingId}
                            projectNumber={project_number}
                            createAssignOpenings={(openingId, userId, utilization, user, userRoles) => this.props.createAssignOpenings(openingId, userId, utilization, user, userRoles)}/>
                </div>
            )
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

Forecasting.contextType = UserContext;

const mapStateToProps = state => {
    return {
        users: state.users,
        projectProfile: state.projectProfile,
        openingId: state.openingId,
        pending: state.pending,
    };
};

const mapDispatchToProps = {
    createAssignOpenings,
    loadMasterlists
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Forecasting);
