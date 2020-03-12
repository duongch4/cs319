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
import DisciplineSearch from "./DisciplineSearch";
import LocationsSearch from "./LocationsSearch";
import AddLocation from './AddLocation';

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
              locations_temp: [],
              location_count: 1,
              discipline_count: 1,
        }
    }

    handler(location) {
        this.setState({
          location_temp: location,
        })
        console.log(location);
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

    addDisciplines = (disciplinesNew) => {
        const name = disciplinesNew.name;
        const newYear = this.state.searchFilter.filter.yearsOfExps;
        const skills = disciplinesNew.skills;

        this.setState({
            ...this.state,
            disciplines_temp: [...this.state.disciplines_temp, {
                    [name]: skills,
            }],
            years_temp: newYear,
        })
    }

    saveFilter = () => {
        var discNew = this.state.disciplines_temp;
        
        var obj = {};
        discNew.forEach((disc, i) => {
            var discKey =  Object.keys(disc);
            var discVal = disc[discKey];
            obj[discKey] = discVal;
        });
 
        this.setState({
            ...this.state,
            searchFilter: {
                ...this.state.searchFilter,
                filter: {
                    ...this.state.searchFilter.filter,
                    locations: this.state.locations_temp,
                    disciplines: obj,
                    yearsOfExp: this.state.years_temp,
                    },
                },
        });
        this.state.searchFilter.filter.locations = this.state.locations_temp;
        this.state.searchFilter.filter.disciplines = obj;
        this.state.searchFilter.filter.yearsOfExps = this.state.years_temp;
    }

    updateLocations = (newLocation) => {
        this.setState({
            ...this.state,
            searchFilter: {
                ...this.state.searchFilter,
                filter: {
                    ...this.state.searchFilter.filter,
                    locations: newLocation.slice(),
                    },
                },
        });
        this.state.searchFilter.filter.locations = newLocation.slice();
    }

    deleteDiscipline = (key) => {
        console.log(key);
        var extra_disciplines = this.state.disciplines_view;
        extra_disciplines.forEach((discipline, index) => {
            if (discipline.key == key) {
                this.state.disciplines_view.splice(index, 1);
                this.setState({
                    ...this.state,
                    disciplines_view: this.state.disciplines_view.splice(index, 1),
                });
            }
        });  
    }

    newDisciplines = () => {
        this.setState({
            ...this.state,
            discipline_count: this.state.discipline_count + 1,
        });
        this.state.discipline_count = this.state.discipline_count + 1;
        var key = "disciplines_" + this.state.discipline_count;
        console.log(key);
        var newDisc = (
            <div className="form-row" key={key}>
            <input className="add" type="button" value="-" onClick={()=> this.deleteDiscipline(key)}/>
             <DisciplineSearch disciplines={this.props.masterlist.disciplines}
                                        masterYearsOfExperience={this.props.masterlist.yearsOfExp}
                                        addDisciplines={this.addDisciplines}/>
            </div>
            );
             this.setState( {
                 ...this.state,
                 disciplines_view: [...this.state.disciplines_view, newDisc]
             })
        }

  render(){
    var disciplines = this.props.masterlist.disciplines;

    var discipline_render = [];
    var all_disciplines_keys = Array.from(Object.keys(disciplines));
    all_disciplines_keys.forEach((discipline, i) => {
        discipline_render.push(<option key={"discipline_" + i} value={discipline}>{discipline}</option>)
    });

    var skills = [];
    var skill_render = [];
    if (Object.keys(this.state.searchFilter.filter.disciplines).length == 0){
      skill_render = <option disabled>Please select a discipline</option>
    } else {
      skills = this.state.skills;
      skills.forEach((skill, i) => {
          skill_render.push(<option key={"skills_" + i} value={skill}>{skill}</option>)
      })
    }
    
    var yearsOfExperience = this.props.masterlist.yearsOfExp;

    var range_render = [];
    yearsOfExperience.forEach((yearsOfExperience, i) => {
        range_render.push(<option key={"yearsOfExperience_" + i} value={yearsOfExperience}>{yearsOfExperience}</option>)
    });

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
                    <input className="add" type="button" value="+" onClick={()=> this.newDisciplines()}/>
                    <DisciplineSearch disciplines={this.props.masterlist.disciplines}
                                        masterYearsOfExperience={this.props.masterlist.yearsOfExp}
                                        addDisciplines={this.addDisciplines}/>
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