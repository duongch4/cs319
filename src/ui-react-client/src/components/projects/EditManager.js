import React, {Component} from 'react';
import {connect} from 'react-redux';
import { updateProject } from '../../redux/actions/projectProfileActions';
import {loadMasterlists} from "../../redux/actions/masterlistsActions";
import Search from '../common/search/Search.js';
import Loading from '../common/Loading';
import {UserContext, getUserRoles} from "../common/userContext/UserContext";

class EditManager extends Component {
  state = {
        projectProfile: {
            projectManager: {},
        },
        pending: true,
  };

  componentDidMount() {
      this.setState({
        ...this.state,
        projectProfile: this.props.projectProfile,
        projectID: this.props.match.params.projectNumber,
        pending: false,
    }) 
  }

  render() {
    if (!this.state.pending) {
      if(Object.keys(this.state.projectProfile).length === 0){
        this.props.history.goBack();
        return <div></div>;
      }
    let project_title = this.state.projectProfile.projectSummary ? this.state.projectProfile.projectSummary.title : "";

    const project_number = this.state.projectProfile.projectSummary.projectNumber;
   
    return (
      <div>
        <div className="activity-container">
          <div className="title-bar">
            <h2 className="blueHeader">{project_title}</h2>
          </div>
          <div className="title-bar">
            <h1 className="greenHeader">{"Assign: Manager"}</h1>
          </div>
        </div>
        <Search onDataFetched={this.handleResultChange}
                isAssignable={true}
                openingId={this.state.projectID}
                projectNumber={project_number}
                createAssignOpenings={(_, userId, __, user, ___) => {
                    var projectProfile ={
                        ...this.state.projectProfile,
                        projectManager:{
                            userID: userId,
                            firstName: user.firstName,
                            lastName: user.lastName,
                        }
                    }
                    this.props.updateProject(projectProfile, this.props.history, getUserRoles(this.context))}
                }/>
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

EditManager.contextType = UserContext;

const mapStateToProps = state => {
  return {
    projectProfile: state.projectProfile,
  };
};

const mapDispatchToProps = {
  loadMasterlists,
  updateProject
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(EditManager);
