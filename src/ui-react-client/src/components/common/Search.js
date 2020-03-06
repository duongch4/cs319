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
      if (e.target.id === "skills") {
        var skills_arr = [...this.state.search.skills, e.target.value];
          this.setState({
            search: {
              ...this.state.search,
              skills: skills_arr
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
    return (
      <div className="activity-container">
        <div className="form-section">
          <form onSubmit={this.handleSubmit}>
          <input type="text" placeholder="Search.." name="search">
            </input>
            <div className="form-row">
                  <div className="form-section opening">
                      <div className="form-row">
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
                      <Button variant="contained"
                                style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small"}}
                                disableElevation
                                onClick={() => this.onSubmit()}>Search</Button>
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
