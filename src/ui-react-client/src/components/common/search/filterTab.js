import React, {Component} from 'react';
import {loadMasterlists} from "../../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import {CLIENT_DEV_ENV} from '../../../config/config';
import {Button} from "@material-ui/core";
import '../common.css'
import './SearchStyles.css'
import AddIcon from '@material-ui/icons/Add';
import Fab from '@material-ui/core/Fab';
import SearchIcon from '@material-ui/icons/SearchRounded';
import Arrow from '@material-ui/icons/KeyboardArrowDownRounded';
import ExpandLessRoundedIcon from '@material-ui/icons/ExpandLessRounded';
import {performUserSearch} from "../../../redux/actions/searchActions";
import FilterStickers from "./FilterStickers";
import CloseIcon from '@material-ui/icons/Close';
import DisciplineSearch from "./DisciplineSearch";
import LocationsSearch from "./LocationsSearch";

class FilterTab extends Component {
    state = {
      searchFilter: {
        filter: {
          utilization: {
            min: 0,
            max: 100
          },
          locations: [
            {
              province: null,
              city: null,
            }
          ],
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
      cities: [],
      skills:[],
      currProvince: "",
      currentDiscipline: "",
      users: this.props.users,
      pending: true,
      showing: false,
      showSticker: true,
      stickerHTML: null,
    };
  
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
    //   var results = this.props.performUserSearch(this.state.searchFilter);
      console.log(this.state.searchFilter);
    };

    addDisciplines = (disciplinesNew) => {
        const name = disciplinesNew.name;
        const years = disciplinesNew.yearsOfExp;
        const skills = disciplinesNew.skills;
        console.log(disciplinesNew);
        this.setState({
            ...this.state,
            searchFilter: {
                ...this.setState.searchFilter,
                filter: {
                    ...this.state.searchFilter.filter,
                    disciplines: this.state.searchFilter.filter.disciplines[name] = skills,
                    yearsOfExps: this.state.searchFilter.filter.yearsOfExps.push(years),
                }
            }
        })
        console.log(this.state.searchFilter.filter);
    }

    addLocations = (locationsNew) => {
        const newObj = {province: locationsNew[0].province}
        console.log(locationsNew);
        this.setState({
            ...this.state,
            searchFilter: {
                ...this.setState.searchFilter,
                filter: {
                    ...this.state.searchFilter.filter,
                    locations: this.state.searchFilter.filter.locations.push(newObj),
                }
            }
        })
    }

    saveFilter = () => {
        var filters = [];
        var results = this.state.searchFilter; 
        var province = results.filter.locations[0].province;
        var city = results.filter.locations[0].city; 

        if (province != null) {
            filters.push(province);
        }
    }
    
    addOpening = (disciplines) => {
        // const openings = [...this.state.projectProfile.openings, opening]
        // this.setState({
        //     projectProfile: {
        //         ...this.state.projectProfile,
        //         openings
        //     }
        // })
    };

    closeFilter = () => {

    }
    
    showFilter = () => {
        // this.refs[filters].style.display = "visible";
    }

    Stickers() {
        const filtersArr = this.state.savedFilters;
        if (filtersArr.length != 0){
            return (
                <FilterStickers filters={this.state.savedFilters}/>
            )
        }
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
    const html_sticker = this.state.stickerHTML;
    console.log(html_sticker);

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
                <div className="form-row">
                </div>
                <div className="form-section opening">
                        <LocationsSearch provinces={this.props.masterlist.locations}
                                        addLocations={this.addLocations}/>
                        <DisciplineSearch disciplines={this.props.masterlist.disciplines}
                                          masterYearsOfExperience={this.props.masterlist.yearsOfExp}
                                          addDisciplines={this.addDisciplines}/>
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