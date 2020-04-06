import React, {Component} from 'react';
import { connect } from 'react-redux';
import UserList from './UserList';
import { loadUsers } from '../../redux/actions/usersActions';
import {CLIENT_DEV_ENV} from '../../config/config';
import {UserContext, getUserRoles} from "../common/userContext/UserContext";
import {loadMasterlists} from "../../redux/actions/masterlistsActions";
import ChevronRightIcon from '@material-ui/icons/ChevronRight';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';
import Select from 'react-select';

class UsersPage extends Component {
  constructor(props) {
    super(props);
    this._ismounted = false;
    this.initialState = {
      users: [],
      filters: null,
      masterlist: {},
      sort_by: [{label: "No filter", value: null},  {label: "Lastname: A-Z", value: "name-AZ"}, {label: "Lastname: Z-A", value: "name-ZA"},
        {label: "Utilization: High to Low", value: "util-high"}, {label: "Utilization: Low to High", value: "util-low"},{label: "Locations: A-Z", value: "locations-AZ"},
        {label: "Locations: Z-A", value: "locations-ZA"}],
      loading: true,
      currPage: 1,
      usersAll: [],
      noResultsNextPage: false,
      lastPage: 1,
      doneLoading: false,
      offset: 1,
      url: "?&orderKey=utilization&order=desc",
      reloading: false,
    };
    this.state = this.initialState;
  }

  componentDidMount() {
    this._ismounted = true;
    if (CLIENT_DEV_ENV) {
      this.props.loadMasterlists(["adminUser"]);
      if (this._ismounted) {
        this.setState({
          ...this.state,
          masterlist: this.props.masterlist,
        })
      }
    } else {
      const userRoles = getUserRoles(this.context);
      this.props.loadMasterlists(userRoles)
          .then(() => {
            if (this._ismounted) {
              this.setState({
                ...this.state,
                masterlist: this.props.masterlist,
              })
            }  
          });
      this.props.loadUsers(this.state.url.concat("&page=1"), userRoles)
      .then(()=> {
<<<<<<< HEAD
        this.setState({
          ...this.state,
          users: this.props.users,
          usersAll: [this.props.users],
        }, () => (
          this.state.users.isLastPage ? this.setState({...this.state, loading: false, doneLoading: true}) : this.getAll(userRoles, this.state.currPage, this.state.offset)
        ));
      })
=======
        if (this._ismounted) {
          this.setState({
            ...this.state,
            users: this.props.users,
            usersAll: [this.props.users],
          }, () => (
            this.state.users.length < 50 ? this.setState({...this.state, loading: false, doneLoading: true}) : this.getAll(userRoles, this.state.currPage, this.state.offset)
          ));
        } 
      })
      .catch(error => {
          this.setState({
            ...this.state,
            noResults: true,
            loading: false,
          });
      });
>>>>>>> console-warnings
    }
  }

  componentDidUpdate() {
      if (!this.state.doneLoading && !this.state.loading && (Math.abs(this.state.lastPage - this.state.currPage) < 2)) {
        if (this._ismounted) {
          this.setState({
            ...this.state, 
            loading: true,
          }, () => this.getAll(getUserRoles(this.context), this.state.lastPage, this.state.offset));
        }
      }
  }

  componentWillUnmount() {
    this._ismounted = false;
  }

  restartLoad() {
    const userRoles = getUserRoles(this.context);
    this.props.loadUsers(this.state.url.concat("&page=1"), userRoles)
    .then(()=> {
<<<<<<< HEAD
      this.setState({
        ...this.state,
        users: this.props.users,
        usersAll: [this.props.users],
        reloading: false,
        noResultsNextPage: false,
      }, () => (
        this.state.users.isLastPage ? this.setState({...this.state, loading: false, doneLoading: true}) : this.getAll(userRoles, this.state.currPage, this.state.offset)
      ));
    })
=======
      if (this._ismounted) {
        this.setState({
          ...this.state,
          users: this.props.users,
          usersAll: [this.props.users],
          reloading: false,
          noResultsNextPage: false,
        }, () => (
          this.state.users.length < 50 ? this.setState({...this.state, loading: false, doneLoading: true}) : this.getAll(userRoles, this.state.currPage, this.state.offset)
        ));
      }
    })
    .catch(error => {
        this.setState({
          ...this.state,
          noResults: true,
          loading: false,
          reloading: false,
          noResultsNextPage: false,
        });
    }); 
>>>>>>> console-warnings
  }

  getAll(userRoles, currPage, offset) {
    // only loads 5 pages at a time so that it doesnt take as long
    if (currPage <= (offset * 5)) {
      if ((!this.state.noResultsNextPage || this.state.usersAll[0].length < 50) 
      && (this.state.filters === null) && this.state.loading){
        var newPage = currPage + 1
        var url = this.state.url.concat("&page=").concat(newPage);
        this.props.loadUsers(url, userRoles)
        .then(()=> {
          if (this._ismounted) {
            this.setState({
              ...this.state,
                usersAll: [...this.state.usersAll, this.props.users],
                noResults: false,
                loading: true,
                lastPage: currPage,
                noResultsNextPage: false,
            }, () => this.getAll(userRoles, newPage, offset));
          }})
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
      if (this.state.usersAll[page_index] !== undefined && !this.state.noResultsNextPage) {
          this.setState({
            ...this.state,
            users: this.state.usersAll[page_index],
            currPage: new_page,
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

  sortUsers = (e) => {
    var sortBy = e.value;
    if (sortBy === "name-AZ"){
        this.setState({
          ...this.initialState,
          reloading: true,
          url: "?&orderKey=lastName&order=asc",
        }, () => this.restartLoad());
    } else if (sortBy === "name-ZA") {
      this.setState({
        ...this.initialState,
        reloading: true,
        url: "?&orderKey=lastName&order=desc",
      }, () => this.restartLoad());
    } else if (sortBy === "util-low") {
      this.setState({
        ...this.initialState,
        reloading: true,
        url: "?&orderKey=utilization&order=asc",
      }, () => this.restartLoad());
    } else if (sortBy === "util-high") {
      this.setState({
        ...this.initialState,
        reloading: true,
        url: "?&orderKey=utilization&order=desc",
      }, () => this.restartLoad());
    } else if (sortBy === "locations-AZ") {
      this.setState({
        ...this.initialState,
        reloading: true,
        url: "?&orderKey=province&order=asc",
      }, () => this.restartLoad());     
    } else if (sortBy === "locations-ZA") {
      this.setState({
        ...this.initialState,
        reloading: true,
        url: "?&orderKey=province&order=desc",
      }, () => this.restartLoad());  
    }
};


  render() {
    return (
      <div>
        {(this.props.showUsers) && (
        <div>
          <div className="form-row" style={{ marginTop: "30px"}}>
            {(!this.state.loading) && 
              (<div style={{ position: "absolute", right: "50px" }}>
                  <Select id="sort" className="input-box" options={this.state.sort_by} onChange={this.sortUsers}
                          placeholder='Sort by:'/>
                </div>)}
                {(this.state.loading) && 
                (<div style={{ position: "absolute", right: "50px", width: "300px"}}>
                  <Select id="sort" className="input-box"
                          isDisabled placeholder='Sort by:'/>
                </div>)}
                </div>
        <div className="pagination-controls">
            {(this.state.currPage === 1) && 
            (<ChevronLeftIcon style={{color: "#E8E8E8"}}/>)}

             {(this.state.currPage> 1) && 
            (<ChevronLeftIcon onClick={() => this.toPrevPage()}/>)}

                Page {this.state.currPage}

            {(this.state.usersAll[this.state.currPage - 1] !== undefined) && 
              (!this.state.noResultsNextPage) && (this.state.currPage !== this.state.lastPage) && !this.state.reloading &&
            (<ChevronRightIcon onClick={() => this.toNextPage()}/>)}

            {((this.state.usersAll[this.state.currPage - 1] === undefined)
             || (this.state.currPage === this.state.lastPage) || (this.state.users.length < 50) ||
             (this.state.noResultsNextPage) || this.state.reloading) && 
            (<ChevronRightIcon style={{color: "#E8E8E8"}} />)}
            </div>
            <hr />
          <UserList users={this.state.users}
                    isAssignable={this.props.isAssignable}
                    projectNumber={this.props.projectNumber}
                    openingId={this.props.openingId}
                    createAssignOpenings={(openingId, userId, utilization, user, userRoles) => this.props.createAssignOpenings(openingId, userId, utilization, user, userRoles)} />
          </div>)}
          </div>
    );
  }
}

UsersPage.contextType = UserContext;

const mapStateToProps = state => {
  return {
    masterlist: state.masterlist,
    users: state.users,
  };
};

const mapDispatchToProps = {
  loadMasterlists,
  loadUsers,
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(UsersPage);
