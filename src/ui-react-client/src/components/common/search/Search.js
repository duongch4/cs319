import React, {Component} from 'react';
import {loadMasterlists} from "../../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import FilterTab from "./FilterTab";
import SearchResults from "./SearchResults";
import {CLIENT_DEV_ENV} from '../../../config/config';

class Search extends Component {
  constructor(props) {
    super(props);
    this.state = {
      filters: null,
      masterlist: {},
    };
    this.handleResultChange = this.handleResultChange.bind(this);
  }

  componentDidMount() {
    if (CLIENT_DEV_ENV) {
        this.props.loadMasterlists()
        this.setState({
            ...this.state,
            masterlist: this.props.masterlist,
        })
    } else {
        this.props.loadMasterlists()
        .then(() => {
            this.setState({
                ...this.state,
                masterlist: this.props.masterlist,
            })
        })
    }
}

  handleResultChange(filter) {
    console.log(filter);
    this.setState({
      ...this.state,
     filters: filter,
    });
  }

  render() {
    if(Object.keys(this.state.masterlist).length === 0 ){
      return <div>loading</div>
    } else {
    return (
      <div className="activity-container">
        <FilterTab onDataFetched={this.handleResultChange} 
                  masterlist={this.state.masterlist}/>
        {(this.state.filters != null) &&
        <SearchResults data={this.state.filters} />}
      </div>
    )
  }
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
