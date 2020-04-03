import React, {Component} from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
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
    this.initialState = {
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
      lastPage: 0,
      doneLoading: false,
      offset: 1,
      url: "?&orderKey=lastName&order=asc",
    };
    this.state = this.initialState;
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
      this.props.loadUsers(this.state.url.concat("&page=1"), userRoles)
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
        var url = this.state.url.concat("&page=").concat(newPage);
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

  sortUsers = () => {
    if (this.state.sortBy === "name-AZ"){
      this.setState({
        ...this.initialState,
        url: "?&orderKey=lastName&order=asc",
      }, () => this.getAll(getUserRoles(this.context), this.state.lastPage, this.state.offset));
    } else if (this.state.sortBy === "name-ZA") {
      this.setState({
        ...this.initialState,
        url: "?&orderKey=lastName&order=desc",
      });
    } else if (this.state.sortBy === "util-low") {
      this.setState({
        ...this.initialState,
        url: "?&orderKey=utilization&order=asc",
      });
    } else if (this.state.sortBy === "util-high") {
      this.setState({
        ...this.initialState,
        url: "?&orderKey=utilization&order=desc",
      });
    } else if (this.state.sortBy === "locations-AZ") {
      this.setState({
        ...this.initialState,
        url: "?&orderKey=province&order=asc",
      });       
    } else if (this.state.sortBy === "locations-ZA") {
      this.setState({
        ...this.initialState,
        url: "?&orderKey=province&order=desc",
      });  
    } else if (this.state.sortBy === "disciplines-AZ") {
      this.setState({
        ...this.initialState,
        url: "?&orderKey=discipline&order=asc",
      });  
    } else if (this.state.sortBy === "disciplines-ZA") {
      this.setState({
        ...this.initialState,
        url: "?&orderKey=discipline&order=desc",
      });  
    } else if (this.state.sortBy === "yearsOfExp-high") {
      this.setState({
        ...this.initialState,
        url: "?&orderKey=yearsOfExp&order=asc",
      });  
    } else if (this.state.sortBy === "yearsOfExp-low") {
      this.setState({
        ...this.initialState,
        url: "?&orderKey=yearsOfExp&order=desc",
      });  
    }
};


  render() {
    return (
      <div className="activity-container">
        <div className="form-row">
        <h1 className="greenHeader">Users</h1>
        <div style={{ position: "absolute", right: "50px" }}>
                  <Select id="sort" className="input-box" options={this.state.sort_by} onChange={this.sortUsers}
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
          <UserList users={this.state.users} />
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
