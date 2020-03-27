import React, {Component} from 'react';
import {loadMasterlists} from "../../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import {CLIENT_DEV_ENV} from '../../../config/config';
import {Button} from "@material-ui/core";
import '../common.css'
import './SearchStyles.css'
import Arrow from '@material-ui/icons/KeyboardArrowDownRounded';
import ExpandLessRoundedIcon from '@material-ui/icons/ExpandLessRounded';
import AddLocation from './AddLocation';
import AddDisciplines from './AddDisciplines';
import YearsSearch from './YearsSearch';
import DatePicker from "react-datepicker";
import InputRange from 'react-input-range';
import 'react-input-range/lib/css/index.css'


class FilterTab extends Component {
    constructor(props) {
        super(props);
        this.reset = false;
        this.initialState = {
            searchFilter: {
                "filter": {
                "utilization": {
                    "min": 0,
                    "max": 200
                },
                "locations": [],
                "disciplines": null,
                "yearsOfExps": [],
                "startDate": new Date(),
                "endDate": new Date(),
            },
            "searchWord": null,
            "orderKey": "utilization",
            "order": "desc",
            "page": 1,
            },
        masterlist: this.props.masterlist,
        showing: false,
        disciplines_temp: null,
        years_temp: [],
        locations_temp: [],
        }
        this.state = this.initialState;
    }
    
    componentDidMount() {
        if (CLIENT_DEV_ENV) {
            this.props.loadMasterlists(["adminUser"])
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

    handleChange = (e) => {
       if (e.target.id === "search") {
        this.setState({
          searchFilter: {
            ...this.state.searchFilter,
            searchWord: e.target.value
            }
        });
      }
    };

    handleChangeStartDate = (date) => {
        this.setState({
            ...this.state,
            searchFilter: {
                ...this.state.searchFilter,
                filter: {
                    ...this.state.searchFilter.filter,
                    "startDate": date
                }
            }
        })
    };

    handleChangeEndDate = (date) => {
        this.setState({
            ...this.state,
            searchFilter: {
                ...this.state.searchFilter,
                filter: {
                    ...this.state.searchFilter.filter,
                    "endDate": date
                }
            }
        })
    };

    performSearch = () => {
        var current_state = JSON.parse(JSON.stringify(this.state.searchFilter));
        this.setState({
            ...this.state,
            showing: false,
         }, ()=>this.props.onDataFetched(current_state));
    };

    saveFilter = () => {
        this.setState({
            ...this.state,
            searchFilter: {
                ...this.state.searchFilter,
                filter: {
                    ...this.state.searchFilter.filter,
                    locations: this.state.locations_temp,
                    disciplines: this.state.disciplines_temp,
                    yearsOfExps: this.state.years_temp,
                }
                }
            }, () =>  this.performSearch());
    }

    updateLocations = (newLocation) => {
        var loc_arr = [];
        newLocation.forEach((location) => {
            if(location.cities.length !== 0){
                location.cities.forEach((city) => {
                        loc_arr.push({locationID: city.id, province: location.province, city: city.city});
                });
                    this.setState({
                        ...this.state,
                        locations_temp: loc_arr,
                    });
            } else {
                this.setState({
                    ...this.state,
                    locations_temp: loc_arr,
                });
            }
        })
    }

    updateDisciplines = (newDiscipline) => {
        // if there are no disciplines yet
        if (this.state.disciplines_temp == null) {
            var name = null;
            var skills = [];
            newDiscipline.forEach((discipline) => {
                name = discipline[1].name;
                skills = discipline[1].skills;
            });
            if (name != null) {
                this.setState({
                    ...this.state,
                    disciplines_temp: {[name]: skills},
                });
            } 
        } else {
            var name = null;
            var skills = [];
            var new_obj = {}
                newDiscipline.forEach((discipline) => {
                    name = discipline[1].name;
                    skills = discipline[1].skills;
                    if (name != null) {
                        new_obj = {...new_obj, [name]: skills}
                    };
                    }
                );
                if (Object.entries(new_obj).length > 0) {
                    this.setState({
                        ...this.state,
                        disciplines_temp: new_obj,
                    });  
                } else {
                    this.setState({
                        ...this.state,
                        disciplines_temp: null,
                    }); 
                } 
    }
}

    updateYears = (years) => {
        this.setState({
            ...this.state,
            years_temp: years.slice(),
        });
    }

    updateUtil = (val) => {
        this.setState({
            ...this.state,
            searchFilter: {
                ...this.state.searchFilter,
                filter: {
                    ...this.state.searchFilter.filter,
                    "utilization": val,
                }
            }
        });
    }

    render(){
        const {showing} = this.state;

        return (
        <div className="form-section">
            <form>
                <div className="form-row">
                    <input className="input-box" type="text" id="search" placeholder="Search" onChange={this.handleChange}/>
                    <Button variant="contained" style={{backgroundColor: "#2c6232", color: "#ffffff", size: "small",  display:(showing ? 'block' : 'none')}} disableElevation onClick={()=> this.saveFilter()}>Apply Filters</Button>
                    <Button variant="contained" style={{backgroundColor: "#2c6232", color: "#ffffff", size: "small",  display:(showing ? 'none' : 'block')}} disableElevation onClick={()=> this.saveFilter()}>Search</Button>
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
                    <div className="form-section opening">
                        <h3 className="darkGreenHeader">Locations</h3>
                        <div className="form-row">
                        <AddLocation locations={this.props.masterlist.locations}
                                        updateLocations={this.updateLocations}/>
                        </div>
                        <h3 className="darkGreenHeader">Disciplines</h3>
                        <div className="form-row">
                            <AddDisciplines disciplines={this.props.masterlist.disciplines}
                                                yearsOfExp={this.props.masterlist.yearsOfExp}
                                                updateDisciplines={this.updateDisciplines}/>
                        </div>
                        <div className="form-row">
                        <div>
                            <h3 className="darkGreenHeader">Years of Experience</h3>
                            <YearsSearch yearsOfExp={this.props.masterlist.yearsOfExp}
                                        updateYears={this.updateYears}/>
                        </div>
                        <div style={{marginLeft: "20px"}}>
                                <h3 className="darkGreenHeader">Availability</h3>
                                <DatePicker className="input-box" id="startDate" selected={this.state.searchFilter.filter.startDate}
                                            onChange={this.handleChangeStartDate}/>
                                <DatePicker className="input-box" id="endDate" selected={this.state.searchFilter.filter.endDate}
                                            onChange={this.handleChangeEndDate}/>
                        </div>
                        </div>
                        <div className="slider" style={{marginBottom: "30px"}}>
                            <h3 className="darkGreenHeader">Utilization</h3>
                            <InputRange maxValue={300} minValue={0} value={this.state.searchFilter.filter.utilization} 
                            onChange={value => this.updateUtil(value)}/>
                        </div>
                    </div>
                </div>
            </form>
        </div>
        );
        }
}

FilterTab.contextType = UserContext;

const mapStateToProps = state => {
  return {
      masterlist: state.masterlist,
  };
};

const mapDispatchToProps = {
  loadMasterlists,
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(FilterTab);