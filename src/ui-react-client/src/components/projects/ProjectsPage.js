import React, { Component } from 'react';
import { connect } from 'react-redux';
import Select from 'react-select';
import { loadProjects } from '../../redux/actions/projectsActions';
import ProjectList from './ProjectList';
import './ProjectStyles.css'
import Fab from '@material-ui/core/Fab';
import AddIcon from '@material-ui/icons/Add';
import { Link } from 'react-router-dom'
import {CLIENT_DEV_ENV} from '../../config/config';
import {UserContext, getUserRoles} from "../common/userContext/UserContext";
import {Button} from "@material-ui/core";
import Loading from '../common/Loading';

class ProjectsPage extends Component {
    state = {
      filter: "",
      projects: [],
      searchWord: null,
      searchPressed: false,
      sort_arr: [{label: "No filter", value: null}, {label: "Title", value: "title"}, {label: "Province", value: "province"},
                {label: "City", value: "city"}, {label: "Start date", value: "startDate"},
                {label: "End date", value: "endDate"}],
      sort: null,
      loading: false,
      noResults: false,
    };

  componentDidMount() {
      if (CLIENT_DEV_ENV) {
        this.props.loadProjects(this.state.filter, ["adminUser"]);
        this.setState({
          ...this.state,
          projects: this.props.projects,
          searchPressed: false,
          loading: false,
        });
      } else {
        const userRoles = getUserRoles(this.context);
        this.props.loadProjects(this.state.filter, userRoles).then(() => {
          this.setState({
            ...this.state,
            projects: this.props.projects,
            searchPressed: false,
            noResults: false,
          }, () => this.setState({...this.state, loading: false}));
        }).catch(err => {
          this.setState({
            ...this.state,
            noResults: true,
            searchPressed: false,
          }, () => this.setState({...this.state, loading: false}));
        });
    }
  };

   componentDidUpdate() {
    if (this.state.searchPressed) {
       this.componentDidMount();
    }
}

  handleChange = (e) => {
    if (e.target.id === "search") {
     this.setState({
         ...this.state,
         searchWord: e.target.value,
         searchPressed: false,
         });
   }
 };

 onFilterChange = (e) => {
  this.setState({
    ...this.state,
    sort: e.value,
    searchPressed: false,
  });
}

 performSearch = () => {
   if (this.state.sort != null || this.state.searchWord != null) {
    var sort = null;
    var searchWord = null;
    if(this.state.sort == null) {
      sort = "startDate";
    } else {
      sort = this.state.sort;
    }

    if (this.state.searchWord == null) {
      searchWord = "";
    } else {
      searchWord = this.state.searchWord;
    }
 
     this.setState({
       ...this.state,
       filter: "?searchWord=".concat(searchWord) + "&orderKey=".concat(sort) + "&order=asc&page=1",
       searchPressed: true,
       loading: true,
       noResults: false,
     });
   }
 }

render() {
  return (
    <div className="activity-container">
        <div className="form-row">
            <input className="input-box" type="text" id="search" placeholder="Search" style={{height: "25px"}}onChange={this.handleChange}/>
                <Select id="sort" className="input-box" options={this.state.sort_arr} onChange={this.onFilterChange}
                     placeholder='Sort by:'/>
            <Button variant="contained" style={{backgroundColor: "#2c6232", color: "#ffffff", size: "small"}} disableElevation onClick={() => this.performSearch()}>Search</Button>
        </div>
        <div className="title-bar">
          <h1 className="greenHeader">Manage Projects</h1>
          <div className="fab-container">
            <Link to={{
              pathname: "/add_project",
              state: {
                profile: this.props.profile
              }
            }}>
            <Fab
                style={{ backgroundColor: "#87c34b", boxShadow: "none"}}
                size={"small"}
                color="primary" aria-label="add">
             <AddIcon />
            </Fab>
            </Link>
          </div>
        </div>
        {(this.state.loading) && 
        <Loading/>}
        {!(this.state.loading) && !(this.state.noResults) &&
        <ProjectList projects={this.props.projects}/>}
        {(this.state.noResults) && 
        <div className="darkGreenHeader">There are no projects that match your search</div>}
    </div>
  );
  }
}
 
ProjectsPage.contextType = UserContext;

_ProjectsPage.propTypes = {
  props: PropTypes.object,
};

const mapStateToProps = state => {
  return {
    projects: state.projects,
  };
};

const mapDispatchToProps = {
  loadProjects
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(ProjectsPage);
