import React, {Component} from 'react';
import {loadMasterlists} from "../../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import FilterTab from "./FilterTab";
import SearchResults from "./SearchResults";
import {CLIENT_DEV_ENV} from '../../../config/config';
import Select from 'react-select';

class Search extends Component {
  constructor(props) {
    super(props);
    this.state = {
      filters: null,
      masterlist: {},
      sort_by: [{label: "No filter", value:"no-filter"}, {label: "Utilization: High to Low", value: "util-high"}, 
                {label: "Utilization: Low to High", value: "util-low"},{label: "Locations", value: "locations"}, 
                {label: "Disciplines", value: "disciplines"}, {label: "Years of Exerpience", value: "yearsOfExp"}],
      sort_by_keys: ["utilization-high", "utilization-low", "location", "disciplines", "yearsOfExp"],
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
        <div>
        <div className="form-row">
        <h3 className="darkGreenHeader">Results</h3>
        <div style={{position: "absolute", right: "50px"}}>
        <Select id="sort" key={this.state.sort_by_keys} className="input-box" options={this.state.sort_by}
          placeholder='Sort by:'/>
          </div>
        </div>
        <SearchResults data={this.state.filters} />
        </div>
        }
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
