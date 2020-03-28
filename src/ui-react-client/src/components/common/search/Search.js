import React, { Component } from 'react';
import { loadMasterlists } from "../../../redux/actions/masterlistsActions";
import { connect } from 'react-redux';
import FilterTab from "./FilterTab";
import SearchResults from "./SearchResults";
import { CLIENT_DEV_ENV } from '../../../config/config';
import Select from 'react-select';
import Loading from '../Loading';
import { UserContext, getUserRoles } from "../userContext/UserContext";

class Search extends Component {
  constructor(props) {
    super(props);
    this.state = {
      filters: null,
      masterlist: {},
      sort_by: [{ label: "No filter", value: "no-filter" }, { label: "Utilization: High to Low", value: "util-high" },
      { label: "Utilization: Low to High", value: "util-low" }, { label: "Locations", value: "locations" },
      { label: "Disciplines", value: "disciplines" }, { label: "Years of Exerpience", value: "yearsOfExp" }],
      sort_by_keys: ["utilization-high", "utilization-low", "location", "disciplines", "yearsOfExp"],
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
        })
    }
  }

  handleResultChange(filter) {
    this.setState({
      ...this.state,
      filters: filter,
    });
  }

  render() {
    if (Object.keys(this.state.masterlist).length === 0) {
      return (
        <div className="activity-container">
          <Loading />
        </div>
      )
    } else {
      const userRoles = getUserRoles(this.context);
      return (
        <div className="activity-container">
          <FilterTab onDataFetched={this.handleResultChange}
            masterlist={this.state.masterlist} />
          {(this.state.filters != null) &&
            <div>
              <div className="form-row">
                <h3 className="darkGreenHeader">Results</h3>
                <div style={{ position: "absolute", right: "50px" }}>
                  <Select id="sort" key={this.state.sort_by_keys} className="input-box" options={this.state.sort_by}
                    placeholder='Sort by:' />
                </div>
              </div>
              <SearchResults data={this.state.filters}
                isAssignable={this.props.isAssignable}
                projectNumber={this.props.projectNumber}
                openingId={this.props.openingId}
                createAssignOpenings={(openingId, userId, utilization, user, userRoles) => this.props.createAssignOpenings(openingId, userId, utilization, user, userRoles)}/>
            </div>
          }
        </div>
      )
    }
  }
}

Search.contextType = UserContext;

const mapStateToProps = state => {
  return {
    masterlist: state.masterlist,
  };
};

const mapDispatchToProps = {
  loadMasterlists
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(Search);
