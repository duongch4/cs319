import React, { Component}  from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import {CLIENT_DEV_ENV} from '../../config/config';
import {loadMasterlists, createDiscpline, createSkill, createProvince,createCity} from '../../redux/actions/masterlistsActions';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import EditIcon from '@material-ui/icons/Edit';

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
        selectedprovince: "",
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
                province: Object.keys(this.state.masterlist.locations)[0],
            },
            selectedprovince: Object.keys(this.state.masterlist.locations)[0]
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

    edit = (elem) => {
        console.log(elem)
    }
    changeSelected = (elem, name) => {
        switch(name) {
            case "discipline":
                return this.setState({
                    ...this.state,
                    skill: {
                        ...this.state.skill,
                        disciplineID: elem
                    }
                })
               
            case "skill":
                return;
            case "province":
                return this.setState({
                    ...this.state,
                    location: {
                        ...this.state.location,
                        province: elem
                    }, 
                    selectedprovince: elem
                });
            case "city":
                return;
            default:
                console.log("ERR")
        }
    }

    listGen = (inputList, name) => {
        let list = []
        inputList.forEach(elem =>{
            list.push(<div key={list.length}>
                <List>
                    <ListItem button name={name} onClick={() => this.changeSelected(elem, name)}>
                    <ListItemText primary={elem} />
                    <EditIcon onClick={()=>this.edit(elem)}/>
                    </ListItem>
                </List>
            </div>)
        })
        return list
    }

    render() {
        const disciplinesObj = this.state.masterlist.disciplines
        const skills = []
        const disciplines = Object.keys(disciplinesObj)
        const selectedDiscipline = this.state.skill.disciplineID ? this.state.skill.disciplineID : disciplines[0]
        for(var skillArr of disciplinesObj[selectedDiscipline]) {
            skills.push(skillArr)
        }
        const provinces = Object.keys(this.state.masterlist.locations)
        const locations = this.state.masterlist.locations
        const cities = []
        const selectedProvince = this.state.selectedprovince ? this.state.selectedprovince : provinces[0]
        for(var cityArr of locations[selectedProvince]){
            cities.push(cityArr)
        }

        const disciplineList = this.listGen(disciplines, "discipline")
        let skillList = this.listGen(skills, "skill")
        const provinceList = this.listGen(provinces, "province")
        let cityList = this.listGen(cities, "city")

        skillList = skillList.length > 0 ? skillList : <div>No Skills Avaialbe for Selected Discipline</div>
        cityList = cityList.length > 0 ? cityList : <div>No Cities Available for Selected Province</div> 
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
                    <h2>{selectedDiscipline} Skills</h2>
                    {skillList}
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
                    <h2>{selectedProvince} Cities</h2>
                    {cityList}
                    <form name="city" onSubmit={this.onSubmit}>
                        <input type="text" onChange={this.handleLocationChange} name="city"/>
                    </form>
                    <button id="city" name="city" onClick={this.onSubmit}>Add City</button>
                </div>
            </div>
        )
    }
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
