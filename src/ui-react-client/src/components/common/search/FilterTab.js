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
import FilterStickers from './FilterSticker';


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
                disciplines: null,
                yearsOfExps: [],
                startDate: null,
                endDate: null,
            },
            searchWord: null,
            orderKey: "utilization",
            order: "desc",
            page: 1,
            },
        stickers: {
            locations: [],
            disciplines: null,
            yearsOfExps: [],
        },
        masterlist: this.props.masterlist,
        skills:[],
        users: this.props.users,
        pending: true,
        showing: false,
        locations_view: [],
        disciplines_view: [],
        disciplines_temp: null,
        years_temp: [],
        years_view: [],
        locations_temp: [],
        location_count: 1,
        discipline_count: 1,
        sticker_view: [],
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
            searchWord: e.target.value
            }
        });
      }
    };

    onSubmit = () => {
        // TODO: figure out why the formatting isn't correct
       var results = this.props.performUserSearch(this.state.searchFilter);
    };

    saveFilter = () => {
        this.setState({
            ...this.state,
            stickers: {
                locations: this.state.locations_temp,
                disciplines: this.state.disciplines_temp,
                yearsOfExps: this.state.years_temp,
                },
            }, () => this.addSticker());
    }

    addSticker = () => {
        var locations = this.state.stickers.locations;
        var disciplines = this.state.stickers.disciplines;
        var years = this.state.stickers.yearsOfExps;
        var newSticker = null;
        var stickers = this.state.sticker_view.slice();
        if (locations.length !== 0) {
            locations.forEach((location) => {
                newSticker = (
                <FilterStickers label={location.city + ", " + location.province}
                                                keyId={location.locationID}
                                                deleteFilter = {this.deleteFilter()}/>);
                
                stickers = [...stickers, newSticker];
                });
        };
       
        if (disciplines != null) {
            Object.entries(disciplines).forEach((discipline) => {
                newSticker = (
                <FilterStickers label={discipline[0] + ": " + discipline[1]}
                                                keyId={discipline[0]}
                                                deleteFilter = {this.deleteFilter()}/>);
                
                stickers = [...stickers, newSticker];
                });
        }

        if (years.length != 0) {
            var count = 0;
            years.forEach((year) => {
                newSticker = (
                    <FilterStickers label={year}
                                    keyId={year + "_" + count}
                                    deleteFilter = {this.deleteFilter()}/>);
                    count++;
                    stickers = [...stickers, newSticker];
            })
        };

            this.setState({
                ...this.state,
                sticker_view: stickers,
            }, () => this.render());

        }

    deleteFilter = () => {

    }

    updateLocations = (newLocation) => {
        var loc_arr = [];
        newLocation.forEach((location) => {
            if(location.cities.length !== 0){
                location.cities.forEach((city) => {
                    loc_arr.push({locationID: city.id, province: location.province, city: city.city});
                    this.setState({
                        ...this.state,
                        locations_temp: loc_arr,
                    });
                });
            }
        })
    }

    updateDisciplines = (newDiscipline) => {
        var discipline_obj = {};
        newDiscipline.forEach((discipline) => {
            discipline_obj[discipline.name] = discipline.skills;
        });
        this.setState({
            ...this.state,
            disciplines_temp: discipline_obj,
        });
    }

    updateYears = (years) => {
        this.setState({
            ...this.state,
            years_temp: years.slice(),
        });
    }

    render(){
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
                        {this.state.sticker_view}
                    </div>
                    <div className="form-section opening">
                        <h3 className="darkGreenHeader">Locations</h3>
                        <div className="form-row">
                        <AddLocation locations={this.props.masterlist.locations}
                                        updateLocations={this.updateLocations}/>
                        </div>
                        {this.state.locations_view}
                        <h3 className="darkGreenHeader">Disciplines</h3>
                        <div className="form-row">
                            <AddDisciplines disciplines={this.props.masterlist.disciplines}
                                                yearsOfExp={this.props.masterlist.yearsOfExp}
                                                updateDisciplines={this.updateDisciplines}/>
                        </div>
                        {this.state.disciplines_view}
                        <h3 className="darkGreenHeader">Years of Experience</h3>
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