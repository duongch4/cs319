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
import ChevronRightIcon from '@material-ui/icons/ChevronRight';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';

class ProjectsPage extends Component {
    state = {
      filter: "?searchWord=&orderKey=startDate&order=asc&page=",
      projects: [],
      searchWord: null,
      searchPressed: false,
      sort_arr: [{label: "No filter", value: null}, {label: "Title", value: "title"}, {label: "Province", value: "province"},
                {label: "City", value: "city"}, {label: "Start date", value: "startDate"},
                {label: "End date", value: "endDate"}],
      sort: null,
      loading: false,
      noResults: false,
      projectsAll: [],
      noResultsNextPage: false,
      currPage: 1,
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
        this.props.loadProjects(this.state.filter.concat(this.state.currPage), userRoles).then(() => {
          this.setState({
            ...this.state,
            projects: this.props.projects,
            searchPressed: false,
            noResults: false,
          }, ()=> this.getAll(userRoles, this.state.currPage));
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

getAll(userRoles, currPage) {
  if (!this.state.noResultsNextPage || this.state.projectsAll[0].length < 50) {
      var newPage = currPage + 1
      var filter = this.getFilterWithPage(newPage);
      this.props.loadProjects(filter, userRoles)
      .then(() => {
          console.log(this.state);
          var projects = (this.props.projects).slice()
          this.setState({
              ...this.state,
              projectsAll: [...this.state.projectsAll, projects],
              noResults: false,
              loading: true,
          }, () => this.getAll(userRoles, newPage))
      }).catch(err => {
          this.setState({
              ...this.state,
              noResultsNextPage: true,
              loading: false,
          }, () => console.log(this.state));
      });
  }
}

toNextPage = () => {
  // var new_page = this.state.currPage + 1;
  // var page_index = this.state.currPage;
  // this.setState({
  //     ...this.state,
  //     userSummaries: this.state.userSummariesAll[page_index],
  //     currPage: new_page,
  // })
}

toPrevPage = () => {
  // var new_page = this.state.currPage - 1;
  // var page_index = new_page - 1;
  // this.setState({
  //     ...this.state,
  //     userSummaries: this.state.userSummariesAll[page_index],
  //     currPage: new_page,
  // })
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
    console.log("?searchWord=".concat(searchWord) + "&orderKey=".concat(sort) + "&order=asc&page=".concat(this.state.currPage));
 
     this.setState({
       ...this.state,
       filter: "?searchWord=".concat(searchWord) + "&orderKey=".concat(sort) + "&order=asc&page=".concat(this.state.currPage),
       searchPressed: true,
       loading: true,
       noResults: false,
     });
   }
 }

getFilterWithPage(currPage) {
    var sort = "";
    var searchWord = "";
    
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
    var filter = "?searchWord=".concat(searchWord) + "&orderKey=".concat(sort) + "&order=asc&page=".concat(currPage);
    return filter;
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
        <div>
          {(this.state.loading) && 
          <Loading/>}
          <div>
            {(this.state.currPage == 1) && 
            (<ChevronLeftIcon style={{color: "#E8E8E8"}}/>)}
            {(this.state.currPage> 1) && 
            (<ChevronLeftIcon onClick={() => this.toPrevPage()}/>)}
                Page {this.state.currPage}
            {(this.state.noResultsNextPage) && 
            (<ChevronRightIcon onClick={() => this.toNextPage()}/>)}
            {(!this.state.noResultsNextPage || (this.state.projects).length < 50) && 
            (<ChevronRightIcon style={{color: "#E8E8E8"}} />)}
          </div>
          <ProjectList projects={this.props.projects}/>
        </div>
        {(this.state.noResults) && 
        <div className="darkGreenHeader">There are no projects that match your search</div>}
    </div>
  );
  }
}
 
ProjectsPage.contextType = UserContext;


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
