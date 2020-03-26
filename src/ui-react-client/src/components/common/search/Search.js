import React, {Component} from 'react';
import {loadMasterlists} from "../../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import FilterTab from "./FilterTab";
import SearchResults from "./SearchResults";
import {CLIENT_DEV_ENV} from '../../../config/config';
import Select from 'react-select';
import Loader from 'react-loader-spinner';
import ChevronRightIcon from '@material-ui/icons/ChevronRight';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';

class Search extends Component {
  constructor(props) {
    super(props);
    this.state = {
      filters: null,
      masterlist: {},
      sort_by: [{label: "No filter", value: null},  {label: "Lastname: A-Z", value: "name-AZ"}, {label: "Lastname: Z-A", value: "name-ZA"},
                {label: "Utilization: High to Low", value: "util-high"}, {label: "Utilization: Low to High", value: "util-low"},{label: "Locations: A-Z", value: "locations-AZ"},
                {label: "Locations: Z-A", value: "locations-ZA"}, {label: "Disciplines: A-Z", value: "disciplines-AZ"},
                {label: "Disciplines: Z-A", value: "disciplines-ZA"}, {label: "Years of Experience: High to Low", value: "yearsOfExp-high"},
                {label: "Years of Experience: Low to High", value: "yearsOfExp-low"}],
      sort_by_keys: ["utilization-high", "utilization-low", "location", "disciplines", "yearsOfExp"],
      sort: null,
      search: false,
      loading: true,
      page: 1,
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

  pageLeft = () => {
    if (this.state.page > 1) {
      this.setState({
        ...this.state,
        filters: {
          ...this.state.filters,
          page: this.state.page -= 1,
        },
        loading: true,
      });
    }
  }

  pageRight = () => {
      this.setState({
        ...this.state,
        filters: {
          ...this.state.filters,
          page: this.state.page += 1,
        },
        loading: true,
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
        {(this.state.filters != null) && (this.state.search) &&
        <div>
        <div className="form-row">
        <h3 className="darkGreenHeader">Results</h3>
        { (this.state.loading) &&
        <Loader type="Grid" color="#2C6232" height={40} width={40}/>
        }
        <div style={{position: "absolute", right: "50px"}}>
        <Select id="sort" key={this.state.sort_by_keys} className="input-box" options={this.state.sort_by} onChange={this.onFilterChange}
          placeholder='Sort by:'/>
          </div>
        </div>
        {(!this.state.loading) &&
        (<div style={{}}>
          <ChevronLeftIcon onClick={this.pageLeft}/>
          Page {this.state.page}
          <ChevronRightIcon onClick={this.pageRight}/>
          </div>)
        }
        <SearchResults data={this.state.filters}
                        sortBy={this.state.sort}
                        stopLoading={this.stopLoading} 
                        page={this.state.page}/>
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
