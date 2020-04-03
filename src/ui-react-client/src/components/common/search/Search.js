import React, {Component} from 'react';
import {loadMasterlists} from "../../../redux/actions/masterlistsActions";
import {clearSearchResults} from "../../../redux/actions/searchActions";
import { loadUsers } from '../../../redux/actions/usersActions';
import {connect} from 'react-redux';
import FilterTab from "./FilterTab";
import SearchResults from "./SearchResults";
import {CLIENT_DEV_ENV} from '../../../config/config';
import Select from 'react-select';
import Loading from '../Loading';
import { UserContext, getUserRoles } from "../userContext/UserContext";
import UserList from '../../users/UserList';
import ChevronRightIcon from '@material-ui/icons/ChevronRight';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';
import {Button} from "@material-ui/core";

class Search extends Component {
  constructor(props) {
    super(props);
    this.state = {
      users: [],
      filters: null,
      masterlist: {},
      sort_by: [{label: "No filter", value: null},  {label: "Lastname: A-Z", value: "name-AZ"}, {label: "Lastname: Z-A", value: "name-ZA"},
        {label: "Utilization: High to Low", value: "util-high"}, {label: "Utilization: Low to High", value: "util-low"},{label: "Locations: A-Z", value: "locations-AZ"},
        {label: "Locations: Z-A", value: "locations-ZA"}, {label: "Disciplines: A-Z", value: "disciplines-AZ"},
        {label: "Disciplines: Z-A", value: "disciplines-ZA"}, {label: "Years of Experience: High to Low", value: "yearsOfExp-high"},
        {label: "Years of Experience: Low to High", value: "yearsOfExp-low"}],
      sort: null,
      search: false,
      loading: true,
      currPage: 1,
      usersAll: [],
      noResultsNextPage: false,
      lastPage: 1,
      doneLoading: false,
      offset: 1,
    };
    this.handleResultChange = this.handleResultChange.bind(this);
  }

  componentDidMount() {
    if (CLIENT_DEV_ENV) {
      this.props.loadMasterlists(["adminUser"]);
      this.setState({
        ...this.state,
        masterlist: this.props.masterlist,
      })
    } else {
      const userRoles = getUserRoles(this.context);
      this.props.loadMasterlists(userRoles)
          .then(() => {
            this.setState({
              ...this.state,
              masterlist: this.props.masterlist,
            })
          });
      this.props.loadUsers("?&orderKey=lastName&order=asc&page=1", userRoles)
      .then(()=> {
        this.setState({
          ...this.state,
          users: this.props.users,
          usersAll: [this.props.users],
        }, () => (
          this.state.users.length < 50 ? this.setState({...this.state, loading: false, doneLoading: true}) : this.getAll(userRoles, this.state.currPage, this.state.offset)
        ));
      })
      .catch(error => {
        this.setState({
          ...this.state,
          noResults: true,
          loading: false,
        });
      });
    }
  }

  componentWillUnmount() {
    this.props.clearSearchResults();
  }

  componentDidUpdate() {
    if (!this.state.doneLoading && !this.state.loading && (Math.abs(this.state.lastPage - this.state.currPage) < 5)) {
      this.setState({
        ...this.state, 
        loading: true,
      }, () => this.getAll(getUserRoles(this.context), this.state.lastPage, this.state.offset));
    }
  }

  getAll(userRoles, currPage, offset) {
    // only loads 10 pages at a time so that it doesnt take as long
    if (currPage <= (offset * 10)) {
      if ((!this.state.noResultsNextPage || this.state.usersAll[0].length < 50) 
      && (this.state.filters === null) && this.state.loading){
        var newPage = currPage + 1
        var url = "?&orderKey=lastName&order=asc&page=".concat(newPage);
        this.props.loadUsers(url, userRoles)
        .then(()=> {
          this.setState({
              usersAll: [...this.state.usersAll, this.props.users],
              noResults: false,
              loading: true,
              lastPage: currPage,
          }, () => this.getAll(userRoles, newPage, offset));
        })
        .catch(error => {
          this.setState({
            ...this.state,
            noResultsNextPage: false,
            loading: false,
            lastPage: currPage,
            doneLoading: true,
            });
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
      var page_index = this.state.currPage;
      if (this.state.usersAll[page_index] != undefined) {
        this.setState({
            ...this.state,
            users: this.state.usersAll[page_index],
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
      users: this.state.usersAll[page_index],
      currPage: new_page,
    })
  }

  handleResultChange(filter) {
    this.setState({
      ...this.state,
      filters: filter,
      search: true,
      loading: true,
      page: 1,
    });
  }

  onFilterChange = (e) => {
    this.setState({
      ...this.state,
      sort: e.value,
    });
  }

  stopLoading = () => {
    this.setState({
      ...this.state,
      loading: false,
    });
  }

  showAllUsers = () => {
    this.setState({
      ...this.state,
      filters: null,
      currPage: 1, 
      noResultsNextPage: false,
      doneLoading: false,
      search: false,
    }, () => console.log(this.state));
  }

  render() {
    if(Object.keys(this.state.masterlist).length === 0 ){
      return (
          <div className="activity-container">
            <Loading />
          </div>
      )
    } else if (this.state.filters == null && (this.state.users).length !== 0) {
      return (
      <div className="activity-container">
        <h1 className="greenHeader">Users</h1>
        <FilterTab onDataFetched={this.handleResultChange}
                      masterlist={this.state.masterlist} />
        <div className="form-row">
          <h2 className="darkGreenHeader">All Users</h2>
          <div style={{ position: "absolute", right: "50px" }}>
                  <Select id="sort" className="input-box" options={this.state.sort_by} onChange={this.onFilterChange}
                          placeholder='Sort by:'/>
          </div>
        </div>
        <div className="pagination-controls">
            {(this.state.currPage == 1) && 
            (<ChevronLeftIcon style={{color: "#E8E8E8"}}/>)}

             {(this.state.currPage> 1) && 
            (<ChevronLeftIcon onClick={() => this.toPrevPage()}/>)}

                Page {this.state.currPage}

            {(this.state.usersAll[this.state.currPage - 1] !== undefined) && 
              (!this.state.noResultsNextPage) && (this.state.currPage != this.state.lastPage) &&
            (<ChevronRightIcon onClick={() => this.toNextPage()}/>)}

            {((this.state.usersAll[this.state.currPage - 1] === undefined)
             || (this.state.currPage == this.state.lastPage) || (this.state.users.length < 50) ||
             (this.state.noResultsNextPage)) && 
            (<ChevronRightIcon style={{color: "#E8E8E8"}} />)}
            </div>
            <hr />
        <UserList users={this.state.users} />
     </div>)
    } else {
      const {showing} = (this.state.filters != null);
      const userRoles = getUserRoles(this.context);
      return (
        <div className="activity-container">
           <div className="form-row">
        <h1 className="greenHeader">Users</h1>
        <Button variant="contained" style={{backgroundColor: "#2c6232", color: "#ffffff", position: "absolute", right: "50px", size: "small",  display:(showing ? 'none' : 'block')}} disableElevation 
        onClick={()=> this.showAllUsers()}>Show all users</Button>
        </div>
          <FilterTab onDataFetched={this.handleResultChange}
                      masterlist={this.state.masterlist} />
          {(this.state.filters != null) && (this.state.search) &&
            (<div>
              <div className="form-row">
                <h2 className="darkGreenHeader">Search Results</h2>
                {(this.state.loading) &&
                <Loading/>
                }
                <div style={{ position: "absolute", right: "50px" }}>
                  <Select id="sort" className="input-box" options={this.state.sort_by} onChange={this.onFilterChange}
                          placeholder='Sort by:'/>
                </div>
              </div>
              <SearchResults data={this.state.filters}
                             sortBy={this.state.sort}
                             stopLoading={this.stopLoading}
                             pageLeft={this.pageLeft}
                             pageRight={this.pageRight}
                             master={this.props.masterlist}
                             isAssignable={this.props.isAssignable}
                             projectNumber={this.props.projectNumber}
                             openingId={this.props.openingId}
                             createAssignOpenings={(openingId, userId, utilization, user, userRoles) => this.props.createAssignOpenings(openingId, userId, utilization, user, userRoles)}/>
            </div>)}
          </div>
      )}
  }
}

Search.contextType = UserContext;


const mapStateToProps = state => {
  return {
    masterlist: state.masterlist,
    users: state.users,
  };
};

const mapDispatchToProps = {
  loadMasterlists,
  clearSearchResults,
  loadUsers,
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Search);
