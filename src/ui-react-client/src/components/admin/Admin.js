import React, { Component}  from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import {CLIENT_DEV_ENV} from '../../config/config';
import {loadMasterlists, createDiscpline, createSkill, createProvince,createCity} from '../../redux/actions/masterlistsActions';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';

class Admin extends Component {
    state = {
        discipline: {
            name: "", 
            id: null
        },
        skill: {
            disciplineID: "", 
            name: "", 
            skillID: null
        },
        location: {
            city: "", 
            province: "", 
            id: null
        },
        masterlist: {}
    };

    static getDerivedStateFromProps(props, state){
        if(CLIENT_DEV_ENV){
            props.loadMasterlists();
            return{
                masterlist: props.masterlist,
            }
        } else {
            props.loadMasterlists()
            .then(() => {
                return{
                    masterlist: props.masterlist
                }
            })
        }
    }

    componentDidMount(){
        this.setState({
            ...this.state,
            skill: {
                ...this.state.skill,
                disciplineID: Object.keys(this.state.masterlist.disciplines)[0]
            },
            location: {
                ...this.state.location,
                province: Object.keys(this.state.masterlist.locations)[0]
            }
        })
        
    }

    handleSkillDropdownChange = (e) => {
        this.setState({
            ...this.state,
            skill: {
                ...this.state.skill,
                disciplineID: e.target.value 
                // TODO: This needs to be changed to the ID of the discpline, but we currently do not have that
            }
        })
    }

    handleCityDropdownChange = (e) => {
        this.setState({
            ...this.state,
            location: {
                ...this.state.location,
                province: e.target.value
            }
        })
    }

    handleChange = (e) => {
        this.setState({
            ...this.state,
            [e.target.name]: {
                ...this.state[e.target.name],
                name: e.target.value
            }
        })
    }

    handleLocationChange = (e) => {
        this.setState({
            ...this.state,
            location: {
                ...this.state.location,
                [e.target.name]: e.target.value
            }
        })
    }

    onSubmit = (e) => {
        e.preventDefault();
        switch(e.target.name) {
            case "discipline":
                return this.props.createDiscpline(this.state.discipline);
            case "skill":
                return this.props.createSkill(this.state.skill);
            case "province":
                return this.props.createProvince(this.state.location);
            case "city":
                return this.props.createCity(this.state.location);
            default:
                console.log("ERR")
        }
    }

    render() {
        let disciplinesObj = this.state.masterlist.disciplines
        let skills = []
        let disciplines = Object.keys(disciplinesObj)
        for(var skillArr of Object.values(disciplinesObj)){
            skillArr.forEach(elem => {
                skills.push(elem)
            })
        }
        let provinces = Object.keys(this.state.masterlist.locations)
        let cities = []
        for(var cityArr of Object.values(this.state.masterlist.locations)){
            cityArr.forEach(elem => {
                cities.push(elem)
            })
        }

        let disciplineList = listGen(disciplines)
        let skillList = listGen(skills)
        let provinceList = listGen(provinces)
        let cityList = listGen(cities)
        
        let disciplineDropDown = []
        disciplines.forEach(elem => {
            disciplineDropDown.push(<option value={elem} key={disciplineDropDown.length}>{elem}</option>
            )
        })
        
        let provinceDropDown = []
        provinces.forEach(elem => {
            provinceDropDown.push(<option value={elem} key={provinceDropDown.length}>{elem}</option>
            )
        })

        return (
            <div className="activity-container">
                <h1 className="greenHeader">Admin</h1>
                <div>
                    <h2>Disciplines</h2>
                    {disciplineList}
                    <form name="discipline" onSubmit={this.onSubmit}>
                    <input type="text" onChange={this.handleChange} name="discipline"/>
                    </form>
                    <button name="discipline" id="discipline" onClick={this.onSubmit}>Add Discipline</button>
                </div>
                <div>
                    <h2>Skills</h2>
                    {skillList}
                    <select className="input-box" id="discipline" onChange={this.handleSkillDropdownChange}>
                        <option value="DEFAULT" disabled>Discipline</option>
                        {disciplineDropDown}
                    </select>
                    <form name="skill" onSubmit={this.onSubmit}>
                        <input type="text" onChange={this.handleChange} name="skill"/>
                    </form>
                    <button id="skill" name="skill" onClick={this.onSubmit}>Add Skill</button>
                </div>
                <div>
                    <h2>Province</h2>
                    {provinceList}
                    <form name="province" onSubmit={this.onSubmit}>
                        <input type="text" onChange={this.handleLocationChange} name="province"/>
                    </form>
                    <button id="province" name="province" onClick={this.onSubmit}>Add Province</button>
                </div>
                <div>
                    <h2>Cities</h2>
                    {cityList}
                    <select className="input-box" id="discipline" onChange={this.handleCityDropdownChange}>
                        <option value="DEFAULT" disabled>Province</option>
                        {provinceDropDown}
                    </select>
                    <form name="city" onSubmit={this.onSubmit}>
                        <input type="text" onChange={this.handleLocationChange} name="city"/>
                    </form>
                    <button id="city" name="city" onClick={this.onSubmit}>Add City</button>
                </div>
            </div>
        )
    }
}

function listGen (inputList) {
    let list = []
    inputList.forEach(elem =>{
        list.push(<div key={list.length}>
            <List>
                <ListItem button>
                <ListItemText primary={elem} />
                </ListItem>
            </List>
        </div>)
    })
    return list
}

Admin.propTypes = {
    masterlist: PropTypes.object.isRequired
};

const mapStateToProps = state => {
    return {
        masterlist: state.masterlist,
    };
};

const mapDispatchToProps = {
    loadMasterlists,
    createDiscpline,
    createSkill,
    createProvince,
    createCity
};
  
export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Admin)
