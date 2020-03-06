import React, {Component} from 'react';
import {loadMasterlists} from "../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import {CLIENT_DEV_ENV} from '../../config/config';
import {Button} from "@material-ui/core";


class Search extends Component {
    state = {
        search: {
            discipline: null,
            skills: [],
            searchWord: "",
            province: null,
            cities: [],
        },
        masterlist: this.props.masterlist,
        pending: true
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
        var cities_arr = [...this.state.search.cities, e.target.value];
        this.setState({
            search: {
                ...this.state.search,
                cities: cities_arr
            }
        });
      } else if (e.target.id === "province") {
        let newCities = this.props.masterlist.locations[e.target.value];
        this.setState({
            search: {
                ...this.state.search,
                province: e.target.value,
            },
            cities: newCities
        });
    } else if (e.target.id === "skills") {
        var skills_arr = [...this.state.search.skills, e.target.value];
          this.setState({
            search: {
              ...this.state.search,
              skills: skills_arr
            }
         });
      } else if (e.target.id === "search") {
        this.setState({
          search: {
            ...this.state.search,
            searchWord: e.target.value
          }
        });
      } else {
        this.setState({
            search: {
                ...this.state.search,
                discipline: e.target.value
            }
        })
    }
    };

    onSubmit = () => {
      // submit parameters
    };


  render(){
    var disciplines = this.props.masterlist.disciplines;

    var discipline_render = [];
    var all_disciplines_keys = Array.from(Object.keys(disciplines));
    all_disciplines_keys.forEach((discipline, i) => {
        discipline_render.push(<option key={"discipline_" + i} value={discipline}>{discipline}</option>)
    });

    var skills = [];
    var skill_render = [];
    if (this.state.search.discipline === null){
      skill_render = <option disabled>Please select a discipline</option>
    } else {
      skills = disciplines[this.state.search.discipline];
      skills.forEach((skill, i) => {
          skill_render.push(<option key={"skills_" + i} value={skill}>{skill}</option>)
      })
    }

    console.log(this.state.masterlist.locations);

    var provinces = this.props.masterlist.locations; 
    var provinces_render = [];
    var all_provinces_key = Array.from(Object.keys(provinces));
    all_provinces_key.forEach((province, i) => {
      provinces_render.push(<option key={"province_" + i} value={province}>{province}</option>)
    });

    var cities = [];
    var cities_render = [];
    if (this.state.search.province=== null){
      cities_render = <option disabled>Please select a province</option>
    } else {
      cities = provinces[this.state.search.province];
      cities.forEach((city, i) => {
        cities_render.push(<option key={"cities_" + i} value={city}>{city}</option>)
      })
    }

    return (
      <div className="activity-container">
        <div className="form-section">
          <h2 className="greenHeader">Search</h2>
            <form onSubmit={this.handleSubmit}>
              <div className="form-row">
                <input className="input-box" type="text" id="search" placeholder="Search" onChange={this.handleChange}/>
              </div>
              <div id="filters">
              <div className="form-row">
                  <div className="form-section opening">
                  <div className="form-row"> Location
                          <select className="input-box" defaultValue={'DEFAULT'}
                                  id="province" onChange={this.handleChange}>
                              <option value="DEFAULT" disabled>Province</option>
                              {provinces_render}
                          </select>
                          <select className="input-box" defaultValue={'DEFAULT'}
                                  id="city" onChange={this.handleChange}>
                              <option value="DEFAULT" disabled>City</option>
                              {cities_render}
                          </select>
                          </div>
                      <div className="form-row"> Disciplines
                          <select className="input-box" defaultValue={'DEFAULT'}
                                  id="discipline" onChange={this.handleChange}>
                              <option value="DEFAULT" disabled>Discipline</option>
                              {discipline_render}
                          </select>
                          <select className="input-box" defaultValue={'DEFAULT'}
                                  id="skills" onChange={this.handleChange}>
                              <option value="DEFAULT" disabled>Skills</option>
                              {skill_render}
                          </select>
                          </div>
                          </div>
                          </div>
              </div>
                      <Button variant="contained"
                                style={{backgroundColor: "#2c6232", color: "#ffffff", size: "small"}}
                                disableElevation
                                onClick={() => this.onSubmit()}>Apply Filters</Button>
                    </form>
        </div>
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
  loadMasterlists
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(Search);
