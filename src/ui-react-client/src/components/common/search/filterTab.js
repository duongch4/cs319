import React, {Component} from 'react';
import {loadMasterlists} from "../../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import {CLIENT_DEV_ENV} from '../../../config/config';
import {Button} from "@material-ui/core";
import '../common.css'
import AddIcon from '@material-ui/icons/Add';
import Fab from '@material-ui/core/Fab';
import SearchIcon from '@material-ui/icons/SearchRounded';
import Arrow from '@material-ui/icons/KeyboardArrowDownRounded';
import ExpandLessRoundedIcon from '@material-ui/icons/ExpandLessRounded';
import {performUserSearch} from "../../../redux/actions/searchActions";

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
          disciplines: {

          },
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
      showing: false
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
      if (e.target.id === "city") {
        this.setState({
          searchFilter: {
            ...this.state.searchFilter,
            filter: {
              ...this.state.searchFilter.filter,
              locations:
              [
                {
                  province: this.state.currProvince,
                  city: e.target.value,
                }
              ]
            },
          },  
        });
      } else if (e.target.id === "province") {
        let newCities = this.props.masterlist.locations[e.target.value];
        this.setState({
          searchFilter: {
            ...this.state.searchFilter,
            filter: {
              ...this.state.searchFilter.filter,
              locations:
              [
                {
                  province: e.target.value,
                }
              ]
            },
          },
            cities: newCities,
            currProvince: e.target.value,
        });
    } else if (e.target.id === "skills") {
          this.setState({
            searchFilter: {
              ...this.state.searchFilter,
              filter: {
                ...this.state.searchFilter.filter,
                disciplines:
                {
                  ...this.state.searchFilter.filter.disciplines,
                  [this.state.currentDiscipline]: [e.target.value],
                }
              },
            }
         });
      } else if (e.target.id === "search") {
        this.setState({
          searchFilter: {
            ...this.state.searchFilter,
          filter: {
            ...this.state.searchFilter.filter,
            searchWord: e.target.value
          }
        }
        });
      } else if (e.target.id === "yearsOfExp") {
        this.setState({
          searchFilter: {
            ...this.state.searchFilter,
          filter: {
            ...this.state.searchFilter.filter,
            yearsOfExps: [e.target.value]
          }
        }
        });
      }else {
        this.setState({
          searchFilter: {
            ...this.state.searchFilter,
          filter: {
            ...this.state.searchFilter.filter,
            disciplines:
            {
              ...this.state.searchFilter.filter.disciplines,
              [e.target.value]: [],
            }
          },
        },
          skills: this.props.masterlist.disciplines[e.target.value],
          currentDiscipline: e.target.value,
        })
    }
    };

    onSubmit = () => {
      var results = this.props.performUserSearch(this.state.searchFilter);
      console.log(results);
    };

    saveFilter = () => {
      // adds sticker tiles to top
    };

    showFilter = () => {
        // this.refs[filters].style.display = "visible";
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

    var provinces = this.props.masterlist.locations; 

    var provinces_render = [];
    var all_provinces_key = Array.from(Object.keys(provinces));
    all_provinces_key.forEach((province, i) => {
      provinces_render.push(<option key={"province_" + i} value={province}>{province}</option>)
    });

    var cities = [];
    var cities_render = [];
    if (this.state.searchFilter.filter.locations.province === null){
      cities_render = <option disabled>Please select a province</option>
    } else {
      cities = this.state.cities;
      cities.forEach((city, i) => {
        cities_render.push(<option key={"cities_" + i} value={city}>{city}</option>)
      })
    }
    var yearsOfExperience = this.props.masterlist.yearsOfExp;

    var range_render = [];
    yearsOfExperience.forEach((yearsOfExperience, i) => {
        range_render.push(<option key={"yearsOfExperience_" + i} value={yearsOfExperience}>{yearsOfExperience}</option>)
    });
    const { showing } = this.state;

    return (
    <div className="form-section">
        <form onSubmit={this.handleSubmit}>
            <div className="form-row">
                <input className="input-box" type="text" id="search" placeholder="Search" onChange={this.handleChange}/>
                <SearchIcon style={{backgroundColor: "#87c34b", color: "white", borderRadius: "3px"}} size={"large"} onClick={()=> this.onSubmit()} />
            </div>
            <div id="filter-closed" className="card-summary" style={ {backgroundColor: "#87c34b", color: "white", paddingLeft: "30px", paddingRight: "30px",display:  (showing ? 'none' : 'block') }}>
                <div style={{padding: "10px"}} >
                <h2  style={{color: "white"}} >Add Filters</h2>
                <Arrow  onClick={()=> this.setState({ showing: !showing })}>toggle </Arrow>
                </div>
            </div>
            <div id="filters" style={ {backgroundColor: "#87c34b", paddingLeft: "30px", paddingRight: "30px", display:  (showing ? 'block' : 'none')}}>
                <div className="form-row">
                    <div className="form-section opening">
                        <div className="form-row"> 
                          <h2  style={{color: "white"}} >Add Filters</h2>
                          <ExpandLessRoundedIcon  onClick={()=> this.setState({ showing: !showing })}>toggle </ExpandLessRoundedIcon>
                          </div>
                          <div className="form-row">
                            <Fab style={{ backgroundColor: "#87c34b", boxShadow: "none"}} size={ "small"} color="primary" aria-label="add">
                                <AddIcon style={{float: "right"}}/>
                            </Fab> 
                            <div style={{color: "white", padding: "5px"}}>
                                Location
                            </div>
                            <select className="input-box" defaultValue={ 'DEFAULT'} style={{padding: "5px"}} id="province" onChange={this.handleChange}>
                                <option value="DEFAULT" disabled>Province</option>
                                {provinces_render}
                            </select>
                            <select className="input-box" defaultValue={ 'DEFAULT'} style={{padding: "5px"}} id="city" onChange={this.handleChange}>
                                <option value="DEFAULT" disabled>City</option>
                                {cities_render}
                            </select>
                        </div>
                        <div className="form-row">
                            <Fab style={{ backgroundColor: "#87c34b", boxShadow: "none"}} size={ "small"} color="primary" aria-label="add">
                                <AddIcon />
                            </Fab> 
                            <div style={{color: "white", padding: "5px"}}>
                                Discipline
                            </div>
                            <select className="input-box" defaultValue={ 'DEFAULT'} id="discipline" onChange={this.handleChange}>
                                <option value="DEFAULT" disabled>Discipline</option>
                                {discipline_render}
                            </select>
                            <select className="input-box" defaultValue={ 'DEFAULT'} id="skills" onChange={this.handleChange}>
                                <option value="DEFAULT" disabled>Skills</option>
                                {skill_render}
                            </select>
                        </div>
                    </div>
                </div>
                <label className="form-row" htmlFor="yearsOfExp">
                    <p className="form-label">
                    <Fab style={{ backgroundColor: "#87c34b", boxShadow: "none"}} size={ "small"} color="primary" aria-label="add">
                      <AddIcon />
                    </Fab></p>
                    <div style={{color: "white", padding: "5px"}}>
                                Years of Experience
                            </div>
                    <select className="input-box" defaultValue={ 'DEFAULT'} id="yearsOfExp" onChange={this.handleChange}>
                        <option value="DEFAULT" disabled>Select a range</option>
                        {range_render}
                    </select>
                </label>
                <div style={{padding: "20px"}}>
                <Button variant="contained" style={{backgroundColor: "#2c6232", color: "#ffffff", size: "small"}} disableElevation onClick={()=> this.saveFilter()}>Apply Filters</Button>
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