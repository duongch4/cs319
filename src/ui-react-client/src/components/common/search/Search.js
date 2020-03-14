import React, {Component} from 'react';
import {loadMasterlists} from "../../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import FilterTab from "./filterTab";
import SearchResults from "./SearchResults";
import initialState from '../../../redux/reducers/_initialState';

class Search extends Component {
  constructor(props) {
    super(props);
    this.state = {
      data: [],
    };
    this.handleResultChange = this.handleResultChange.bind(this);
  }

  handleResultChange(data) {
    this.setState({
      data,
    });
  }

  render() {
    return (
      <div className="activity-container">
        <FilterTab onDataFetched={this.handleResultChange} />
        {(this.state.data.length != 0) &&
        <SearchResults data={initialState.users} />}
      </div>
    )
  }
}

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
