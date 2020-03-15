import React, {Component} from 'react';
import {loadMasterlists} from "../../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import {CLIENT_DEV_ENV} from '../../../config/config';
import {Button} from "@material-ui/core";
import '../common.css'
import './SearchStyles.css'
import SearchIcon from '@material-ui/icons/SearchRounded';
import Arrow from '@material-ui/icons/KeyboardArrowDownRounded';
import ExpandLessRoundedIcon from '@material-ui/icons/ExpandLessRounded';
import {performUserSearch} from "../../../redux/actions/searchActions";
import AddLocation from './AddLocation';
import AddDisciplines from './AddDisciplines';
import YearsSearch from './YearsSearch';

class FilterTab extends Component {
    constructor(props) {
        super(props);
        this.state = {
                searchFilter: {
                  filter: {
                    utilization: {
                      min: 0,
                      max: 100
                    },
                    locations: [],
                    disciplines: {},
                    yearsOfExps: [],
                    startDate: null,
                    endDate: null,
                },
                orderKey: "utilization",
                order: "desc",
                page: 1,
              },
              masterlist: this.props.masterlist,
              skills:[],
              users: this.props.users,
              pending: true,
              showing: false,
              locations_view: [],
              disciplines_view: [],
              disciplines_temp: [],
              years_temp: [],
              years_view: [],
              locations_temp: [],
              location_count: 1,
              discipline_count: 1,
        }
    }
    
    componentDidMount() {
        if (CLIENT_DEV_ENV) {
            this.props.loadMasterlists()
            this.setState({
                ...this.state,
                masterlist: this.props.masterlist,
                pending: false
            })
        } else {
            this.props.loadMasterlists()
            .then(() => {
                this.setState({
                    ...this.state,
                    masterlist: this.props.masterlist,
                    pending: false
                })
            })
        }
    }

    handleChange = (e) => {
       if (e.target.id === "search") {
        this.setState({
          searchFilter: {
            ...this.state.searchFilter,
          filter: {
            ...this.state.searchFilter.filter,
            searchWord: e.target.value
          }
        }
        });
      }
    };

    onSubmit = () => {
        //not connected yet... fake data being passed out
       this.props.onDataFetched(this.state.searchFilter);
    };

    saveFilter = () => {
        this.setState({
            ...this.state,
            searchFilter: {
                ...this.state.searchFilter,
                filter: {
                    ...this.state.searchFilter.filter,
                    locations: this.state.locations_temp,
                    },
                },
        });
        this.state.searchFilter.filter.locations = this.state.locations_temp;
    }

    updateLocations = (newLocation) => {
        this.setState({
            ...this.state,
            locations_temp: newLocation.slice(),
        });
        this.state.locations_temp = newLocation.slice();
    }

    updateDisciplines = (newDiscipline) => {
        this.setState({
            ...this.state,
            disciplines_temp: newDiscipline.slice(),
        });
        this.state.disciplines_temp = newDiscipline.slice();
    }

    updateYears = (years) => {
        console.log(years);
        this.setState({
            ...this.state,
            years_temp: years.slice(),
        });
        this.state.years_temp = years.slice();
    }

  render(){
    console.log(this.state);

    const {showing} = this.state;

    return (
    <div className="form-section">
        <form onSubmit={this.handleSubmit}>
            <div className="form-row">
                <input className="input-box" type="text" id="search" placeholder="Search" onChange={this.handleChange}/>
                <SearchIcon style={{backgroundColor: "#87c34b", color: "white", borderRadius: "3px"}} size={"large"} onClick={()=> this.onSubmit()} />
            </div>
            <div id="filter-closed" style={ {backgroundColor: "#87c34b", color: "white", paddingLeft: "30px", paddingRight: "30px",display:  (showing ? 'none' : 'block')}}>
                <div style={{padding: "10px"}} >
                <h2  style={{color: "white"}} >Add Filters
                <Arrow  style={{float:"right"}} size={"large"} onClick={()=> this.setState({ showing: !showing })}>toggle </Arrow>
                </h2>
                </div>
            </div>
            <div id="filters" style={ {backgroundColor: "#87c34b", paddingLeft: "30px", paddingRight: "30px", display:  (showing ? 'block' : 'none')}}>
                <div style={{padding: "10px"}}> 
                    <h2  style={{color: "white"}} >Add Filters
                        <ExpandLessRoundedIcon style={{float:"right"}} onClick={()=> this.setState({ showing: !showing })}>toggle </ExpandLessRoundedIcon>
                    </h2>
                </div>
                <div className="form-row" id="stickers">
                    {this.state.stickerHTML}
                </div>
                <div className="form-section opening">
                   <div className="form-row">
                   <AddLocation locations={this.props.masterlist.locations}
                                updateLocations={this.updateLocations}/>
                   </div>
                   {this.state.locations_view}
                    <div className="form-row">
                    <AddDisciplines disciplines={this.props.masterlist.disciplines}
                                        yearsOfExp={this.props.masterlist.yearsOfExp}
                                        updateDisciplines={this.updateDisciplines}/>
                    </div>
                    {this.state.disciplines_view}
                    <div className="form-row">
                    <YearsSearch yearsOfExp={this.props.masterlist.yearsOfExp}
                                updateYears={this.updateYears}/>
                    </div>
                    {this.state.disciplines_view}
                <div style={{padding: "20px"}}>
                <Button variant="contained" style={{backgroundColor: "#2c6232", color: "#ffffff", size: "small"}} disableElevation onClick={()=> this.saveFilter()}>Apply Filters</Button>
                </div>
            </div>
            </div>
        </form>
    </div>
    );
  }
};

const mapStateToProps = state => {
  return {
      masterlist: state.masterlist,
  };
};

const mapDispatchToProps = {
  loadMasterlists,
  performUserSearch,
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(FilterTab);