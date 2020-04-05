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
import PropTypes from 'prop-types';

class ProjectsPage extends Component {
  constructor(props) {
    super(props);
    this._ismounted = false;
  }

    state = {
      filter: "?searchWord=&orderKey=startDate&order=asc&page=1",
      projects: [],
      searchWord: null,
      searchPressed: false,
      sort_arr: [{label: "No filter", value: null}, {label: "Title", value: "title"}, {label: "Province", value: "province"},
                {label: "City", value: "city"}, {label: "Start date", value: "startDate"},
                {label: "End date", value: "endDate"}],
      sort: null,
      loading: true,
      noResults: false,
      projectsAll: [],
      noResultsNextPage: false,
      currPage: 1,
      offset: 1,
      lastPage: 1,
      doneLoading: false,
    };

  componentDidMount() {
    this._ismounted = true;
      if (CLIENT_DEV_ENV) {
        this.props.loadProjects(this.state.filter, ["adminUser"]);
        if (this._ismounted) {
          this.setState({
            ...this.state,
            projects: this.props.projects,
            searchPressed: false,
            loading: false,
          });
        }
      } else {
        const userRoles = getUserRoles(this.context);
        this.props.loadProjects(this.state.filter, userRoles).then(() => {
          if (this._ismounted) {
            this.setState({
              ...this.state,
              projects: this.props.projects,
              searchPressed: false,
              noResults: false,
              loading: true,
              lastPage: 1,
              projectsAll: [this.props.projects],
              doneLoading: false,
            }, ()=> (
              this.state.projects.length < 50 ? this.setState({...this.state, loading: false, doneLoading: true}) : this.getAll(userRoles, this.state.currPage, this.state.offset)
            ))
          }   
        }).catch(err => {
          this.setState({
            ...this.state,
            noResults: true,
            searchPressed: false,
            loading: false,
          });
        });
    }
  };

  componentDidUpdate() {
    if (this.state.searchPressed) {
      this.resetSearch();
    } else if (!this.state.doneLoading && !this.state.loading && Math.abs(this.state.lastPage - this.state.currPage) < 2) {
      // once the user 1 page away from the last loaded page, it will load 2 more
      this.loadMore();
    }
  }

  componentWillUnmount() {
    this._ismounted = false;
  }

  loadMore = () => {
    var lastPage = this.state.lastPage;
    if (this._ismounted) {
      this.setState({
        ...this.state, 
        loading: true,
      }, () => this.getAll(getUserRoles(this.context), lastPage, this.state.offset))  
    }
  }

  resetSearch = () => {
    const userRoles = getUserRoles(this.context);
    this.props.loadProjects(this.state.filter, userRoles).then(() => {
      if (this._ismounted) {
        this.setState({
          ...this.state,
          projects: this.props.projects,
          searchPressed: false,
          noResults: false,
          loading: true,
          lastPage: 1,
          projectsAll: [this.props.projects],
          doneLoading: false,
        }, ()=> (
          this.state.projects.length < 50 ? this.setState({...this.state, loading: false, doneLoading: true}) : this.getAll(userRoles, this.state.currPage, this.state.offset)
        ))
      }      
    }).catch(err => {
      this.setState({
        ...this.state,
        noResults: true,
        searchPressed: false,
        loading: false,
      });
    });
  }

  getAll(userRoles, currPage, offset) {
    // only loads 2 pages at a time so that it doesnt take as long
    if (currPage <= (offset * 3)) {
      if (!this.state.noResultsNextPage && this.state.projectsAll[0].length === 50 && !this.state.searchPressed) {
        var newPage = currPage + 1
        var filter = this.getFilterWithPage(newPage);
        this.props.loadProjects(filter, userRoles)
        .then(() => {
            var projects = (this.props.projects).slice()
            if (this._ismounted) {
              this.setState({
                ...this.state,
                projectsAll: [...this.state.projectsAll, projects],
                noResults: false,
                loading: true,
                lastPage: currPage,
            }, () => this.getAll(userRoles, newPage, offset))
            }       
        }).catch(err => {
            this.setState({
                ...this.state,
                noResultsNextPage: false,
                loading: false,
                lastPage: currPage,
                doneLoading: true,
            });
        });
        // if the last page of projects has less than 50 projects, that means it's at the end 
        // and there are no more projects to load
      } else if (this.state.projectsAll[this.state.projectsAll.length - 1].length < 50) {
          this.setState({
            ...this.state,
            noResultsNextPage: false,
            loading: false,
            lastPage: currPage,
            doneLoading: true,
        });
      }
    } else {
      // stops loading after it loads 10 pages
        this.setState({
          ...this.state,
          noResultsNextPage: false,
          loading: false,
          offset: this.state.offset + 1,
          lastPage: currPage,
      });
      }
    }

    toNextPage = () => {
      var new_page = this.state.currPage + 1;
      var page_index = new_page - 1;
      if (this.state.projectsAll[page_index] !== undefined) {
        this.setState({
            ...this.state,
            projects: this.state.projectsAll[page_index],
            currPage: new_page,
            noResultsNextPage: false
        })
      } else {
        this.setState({
          ...this.state,
          noResultsNextPage: true,
      })
      } 
    }

    toPrevPage = () => {
      var new_page = this.state.currPage - 1;
      var page_index = new_page - 1;
      this.setState({
        ...this.state,
        projects: this.state.projectsAll[page_index],
        currPage: new_page,
      })
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
      if(this.state.sort === null) {
        sort = "startDate";
      } else {
        sort = this.state.sort;
      }

      if (this.state.searchWord === null) {
        searchWord = "";
      } else {
        searchWord = this.state.searchWord;
      }
  
      this.setState({
        ...this.state,
        filter: "?searchWord=".concat(searchWord) + "&orderKey=".concat(sort) + "&order=asc&page=".concat(this.state.currPage),
        searchPressed: true,
        loading: true,
        noResults: false,
        currPage: 1,
      }, () => this.setState({...this.state,loading:false}));
    }
  }

  getFilterWithPage(currPage) {
      var sort = "";
      var searchWord = "";
      
      if(this.state.sort === null) {
        sort = "startDate";
      } else {
        sort = this.state.sort;
      }

      if (this.state.searchWord === null) {
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
              <div className="pagination-controls">
              {(this.state.currPage === 1) && 
              (<ChevronLeftIcon style={{color: "#E8E8E8"}}/>)}

              {(this.state.currPage> 1) && 
              (<ChevronLeftIcon onClick={() => this.toPrevPage()}/>)}

                  Page {this.state.currPage}

              {(this.state.projectsAll[this.state.currPage - 1] !== undefined) && 
                (!this.state.noResultsNextPage) && (this.state.currPage !== this.state.lastPage) &&
              (<ChevronRightIcon onClick={() => this.toNextPage()}/>)}

              {((this.state.projectsAll[this.state.currPage - 1] === undefined)
              || (this.state.currPage === this.state.lastPage) || (this.state.projects.length < 50) ||
              (this.state.noResultsNextPage)) && 
              (<ChevronRightIcon style={{color: "#E8E8E8"}} />)}
              </div>
              {(this.state.projects.length === 0) &&
              <div>
                <Loading/>
              </div>}
            {(this.state.projects.length > 0) &&
            <ProjectList projects={this.state.projects}/>}
          </div>
          {(this.state.noResults) && 
          <div className="darkGreenHeader">There are no projects that match your search</div>}
      </div>
    );
    }
  }
 
ProjectsPage.contextType = UserContext;

ProjectsPage.propTypes = {
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
