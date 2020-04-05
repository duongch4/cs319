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
import {Button} from "@material-ui/core";
import UsersPage from '../../users/UsersPage';

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
      showUsers: true,
      clearFilters: false,
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
    }
  }

  componentWillUnmount() {
    this.props.clearSearchResults();
  }

  handleResultChange(filter) {
    // if search filter is the same as previous search filter, it will not reload 
    // because the results are the same
    if (JSON.stringify(filter) !== JSON.stringify(this.state.filters)) {
      this.setState({
        ...this.state,
        filters: filter,
        search: true,
        loading: true,
        showUsers: false,
      });
    }
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
      showUsers: true,
      filters: null,
      clearFilters: true,
    }, () => this.setState({...this.state, clearFilters: false}))
  }

  render() {
    if(Object.keys(this.state.masterlist).length === 0 ){
      return (
          <div className="activity-container">
            <Loading />
          </div>
      )
    } else if (this.state.showUsers) {
      return (
      <div className="activity-container">
        <h1 className="greenHeader">All Users</h1>
        <FilterTab onDataFetched={this.handleResultChange}
                      masterlist={this.state.masterlist}
                      clear={this.state.clearFilters}/>
        <UsersPage data={this.state.filters}
                  showUsers={this.state.showUsers}
                  isAssignable={this.props.isAssignable}
                  projectNumber={this.props.projectNumber}
                  openingId={this.props.openingId}
                  createAssignOpenings={(openingId, userId, utilization, user, userRoles) => this.props.createAssignOpenings(openingId, userId, utilization, user, userRoles)}/>
        </div>)
      } else {
      const {showing} = (this.state.filters != null);
      return (
        <div className="activity-container">
        <div className="form-row">
        <h1 className="greenHeader">Users</h1>
        {(!this.state.loading) && (!this.state.showUsers) &&
       (<Button variant="contained" style={{backgroundColor: "#2c6232", color: "#ffffff", position: "absolute", right: "50px", size: "small",  display:(showing ? 'none' : 'block')}} disableElevation 
        onClick={()=> this.showAllUsers()}>Show all users</Button>)}
        </div>
          <FilterTab onDataFetched={this.handleResultChange}
                      masterlist={this.state.masterlist}
                      clear={this.state.clearFilters} />
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
