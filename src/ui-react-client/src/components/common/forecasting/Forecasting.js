import React, {Component} from 'react';
import '../../projects/ProjectStyles.css';
import {connect} from 'react-redux';
import {createAssignOpenings} from '../../../redux/actions/forecastingActions';
import Search from '../search/Search.js';
import {loadMasterlists} from "../../../redux/actions/masterlistsActions";
import Loading from '../Loading';
import {UserContext} from "../userContext/UserContext";

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
        if(prevProps.users !== this.props.users){
            this.setState({
                ...this.state,
                users: this.props.users
            })
        }
        if(prevProps.projectProfile !== this.props.projectProfile){
            this.setState({
                ...this.state,
                projectProfile: this.props.projectProfile
            })
        }
        if(prevProps.openingId !== this.props.openingId){
            this.setState({
                ...this.state,
                openingId: this.props.openingId
            })
        }
    }

    render() {
        if (!this.state.pending) {
            if(Object.keys(this.state.projectProfile).length === 0){
                this.props.history.goBack();
                return <div></div>;
            }
            let project_title = this.state.projectProfile.projectSummary ? this.state.projectProfile.projectSummary.title : "";

            let openings = this.state.projectProfile.openings;
            let selectedOpening = {};

            openings.forEach((opening, index) => {
                if (opening.positionID === parseInt(this.state.openingId)) {
                    selectedOpening = opening;
                }
            });

            let discipline_name = selectedOpening.discipline;
            const project_number = this.state.projectProfile.projectSummary.projectNumber;
            const projectSummary = this.state.projectProfile.projectSummary;

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
                            createAssignOpenings={(openingId, userId, utilization, user, userRoles) => this.props.createAssignOpenings(selectedOpening, userId, utilization, user, userRoles, projectSummary, this.props.history)}/>
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
