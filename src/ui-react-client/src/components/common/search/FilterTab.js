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
        this.initialState = {
            searchFilter: {
                "filter": {
                "utilization": {
                    "min": 0,
                    "max": 100
                },
                "locations": [],
                "disciplines": null,
                "yearsOfExps": [],
                "startDate": null,
                "endDate": null,
            },
            "searchWord": null,
            "orderKey": "utilization",
            "order": "desc",
            "page": 1,
            },
        stickers: {
            locations: [],
            disciplines: null,
            yearsOfExps: [],
        },
        masterlist: this.props.masterlist,
        pending: true,
        showing: false,
        disciplines_temp: null,
        years_temp: [],
        locations_temp: [],
        sticker_view: [],
        }
        this.state = this.initialState;
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

    performSearch = () => {
        var current_state = JSON.parse(JSON.stringify(this.state.searchFilter));
        this.setState({
            ...this.state,
            showing: false,
         }, ()=>this.props.onDataFetched(current_state));
    };

    getFilters =() => {
        var locations_good = [];
        var disciplines_good = {};
        var years_good = [];

        this.state.stickers.locations.forEach((location) => {
            locations_good = [...locations_good, {province: location.province, city: location.city}];
        });
        
        if(this.state.stickers.disciplines){
            Object.entries(this.state.stickers.disciplines).forEach((discipline) => {
                disciplines_good = Object.assign({}, disciplines_good, {[discipline[1].name]: discipline[1].skills})
            });
        } else {
            disciplines_good = null;
        }
        
        this.state.stickers.yearsOfExps.forEach((year) => {
            years_good = [...years_good, year];
        });

        this.setState({
            ...this.state,
            searchFilter: {
                ...this.state.searchFilter,
                filter: {
                    locations: locations_good,
                    disciplines: disciplines_good,
                    yearsOfExps: years_good,
                }
            }
        }, () => this.performSearch())
    }

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
                <FilterStickers key={location.locationID} label={location.city + ", " + location.province}
                                                keyId={location.locationID}
                                                type={"location"}
                                                deleteFilter = {this.deleteFilter}/>);
                
                stickers = [...stickers, newSticker];
                });
        };
       
        if (disciplines !== null) {
            Object.entries(disciplines).forEach((discipline) => {
                newSticker = (
                <FilterStickers key={discipline[0]} label={discipline[1].name + ": " + discipline[1].skills}
                                                type={"discipline"}
                                                keyId={discipline[0]}
                                                deleteFilter = {this.deleteFilter}/>);
                
                stickers = [...stickers, newSticker];
                });
        }

        if (years.length !== 0) {
            years.forEach((year) => {
                newSticker = (
                    <FilterStickers key={year} label={year}
                                    type={"year"}
                                    keyId={year}
                                    deleteFilter = {this.deleteFilter}/>);
                    stickers = [...stickers, newSticker];
            })
        };

        this.setState({
            ...this.state,
            sticker_view: stickers,
        });
    }

    deleteFilter = (type, keyId) => {
        var view_mock = this.state.sticker_view.slice();

        if (type === "location") {
            var location_mock = this.state.stickers.locations.slice();
            this.state.stickers.locations.forEach((location, index) => {
            if (location.locationID === keyId) {
                location_mock.splice(index,1);
                this.state.sticker_view.forEach((curr, index1) => {
                    if ((parseInt(curr.key)) === keyId) {
                        view_mock.splice(index1, 1);
                    }
                });
                this.setState({
                    ...this.state,
                    stickers: {
                        ...this.state.stickers,
                        locations: location_mock,
                    },
                    sticker_view: view_mock,
                    });
                }
            });
        } else if (type === "discipline") {
            var discipline_mock = this.state.stickers.disciplines;
            Object.entries(this.state.stickers.disciplines).forEach((discipline, index) => {
            if (discipline[0] === keyId) {
                delete discipline_mock[keyId];
                this.state.sticker_view.forEach((curr, index1) => {
                    if (curr.key === keyId) {
                        view_mock.splice(index1, 1);
                    }
                });
                if (Object.keys(discipline_mock).length === 0){
                    discipline_mock = null;
                }
                this.setState({
                    ...this.state,
                    stickers: {
                        ...this.state.stickers,
                        disciplines: discipline_mock,
                    },
                    sticker_view: view_mock,
                    });
                }
            });
        } else {
            var years_mock = this.state.stickers.yearsOfExps.slice();
            this.state.stickers.yearsOfExps.forEach((year, index) => {
                if (year === keyId) {
                    years_mock.splice(index,1);
                    this.state.sticker_view.forEach((curr, index1) => {
                        if (curr.key === keyId) {
                            view_mock.splice(index1, 1);
                        }
                    });
                    this.setState({
                        ...this.state,
                        stickers: {
                            ...this.state.stickers,
                            yearsOfExps: years_mock,
                        },
                        sticker_view: view_mock,
                        });
                    }
                });
        }
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
            }
        })
    }

    updateDisciplines = (newDiscipline) => {
        var discipline_obj = {};
        newDiscipline.forEach((discipline) => {
            discipline_obj[discipline[0]] = discipline[1];
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
                    <SearchIcon style={{backgroundColor: "#87c34b", color: "white", borderRadius: "3px"}} size={"large"} onClick={()=> this.getFilters()} />
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
                        <h3 className="darkGreenHeader">Disciplines</h3>
                        <div className="form-row">
                            <AddDisciplines disciplines={this.props.masterlist.disciplines}
                                                yearsOfExp={this.props.masterlist.yearsOfExp}
                                                updateDisciplines={this.updateDisciplines}/>
                        </div>
                        <h3 className="darkGreenHeader">Years of Experience</h3>
                        <div className="form-row">
                            <YearsSearch yearsOfExp={this.props.masterlist.yearsOfExp}
                                        updateYears={this.updateYears}/>
                        </div>
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