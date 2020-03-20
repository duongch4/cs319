import React, { Component}  from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import {CLIENT_DEV_ENV} from '../../config/config';
import {loadMasterlists, 
    createDiscpline, 
    createSkill, 
    createProvince,
    createCity, 
    deleteDiscipline,
    deleteSkill,
    deleteProvince,
    deleteCity} from '../../redux/actions/masterlistsActions';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import DeleteIcon from '@material-ui/icons/Delete'

class Admin extends Component {
    state = {
        discipline: {
            name: "", 
            id: 0
        },
        skill: {
            disciplineID: 0, 
            name: "", 
            skillID: 0
        },
        location: {
            city: "", 
            province: "", 
            id: 0
        },
        selectedprovince: "",
        masterlist: {
            disciplines: {},
            locations: {},
            yearsOfExp: []
        }
    };

    componentDidMount(){
        if(this.state.masterlist.yearsOfExp.length === 0){
            if(CLIENT_DEV_ENV){
                this.props.loadMasterlists()
                var masterlist = this.props.masterlist
                    this.setState({
                        ...this.state,
                        masterlist,
                        skill: {
                            ...this.state.skill,
                            disciplineID: Object.values(masterlist.disciplines).length > 0 ? Object.values(masterlist.disciplines)[0].disciplineID : 0
                        },
                        location: {
                            ...this.state.location,
                            province: Object.keys(masterlist.locations)[0],
                            id: Object.values(masterlist.locations).length > 0 ? Object.values(Object.values(masterlist.locations)[0])[0] : 0
                        },
                        selectedprovince: Object.keys(masterlist.locations)[0]
                    })
            } else {
                this.props.loadMasterlists()
                .then(() => {
                    var masterlist = this.props.masterlist
                    this.setState({
                        ...this.state,
                        masterlist,
                        skill: {
                            ...this.state.skill,
                            disciplineID: Object.values(masterlist.disciplines).length > 0 ? Object.values(masterlist.disciplines)[0].disciplineID : 0
                        },
                        location: {
                            ...this.state.location,
                            id: Object.values(masterlist.locations).length > 0 ? Object.values(Object.values(masterlist.locations)[0])[0] : 0
                        },
                        selectedprovince: Object.keys(masterlist.locations)[0]
                    })
                })
            }
        }
    }
    componentDidUpdate(prevProps){
        let found = false;
        Object.values(this.props.masterlist.disciplines).filter(elem => {
            if(elem.disciplineID === this.state.skill.disciplineID){
                found = true
            }
            return true;
        })
        if(!found){
            let disciplineName = Object.keys(this.props.masterlist.disciplines)[0]
            this.setState({
                ...this.state,
                skill: {
                    ...this.state.skill,
                    disciplineID: this.props.masterlist.disciplines[disciplineName].disciplineID
                },
                masterlist: this.props.masterlist,
            })
        }
        else if(prevProps.masterlist !== this.props.masterlist){
            this.setState({
                ...this.state,
                masterlist: this.props.masterlist
            })
        }
        
        
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
                const discipline = this.state.discipline;
                this.props.createDiscpline(discipline);
                this.setState({
                    ...this.state,
                    discipline: {
                        name: "",
                        id: 0
                    }
                })
                
                return;
            case "skill":
                const skill = this.state.skill;
                this.props.createSkill(skill);
                this.setState({
                    ...this.state,
                    skill:{
                        ...this.state.skill,
                        name: ""
                    }
                })
                return ;
            case "province":
                const location = this.state.location;
                this.props.createProvince(location);
                this.setState({
                    ...this.state,
                    location:{
                        ...this.state.location,
                        province: ""
                    }
                })
                return;
            case "city":
                this.setState({
                    ...this.state,
                    location: {
                        ...this.state.location,
                        province: this.state.selectedprovince
                    }
                }, () =>  {
                    this.props.createCity(this.state.location)
                    this.setState({
                        ...this.state,
                        location:{
                            ...this.state.location,
                            city: "",
                            province: "",
                        }
                    })})
                
                return;
            default:
                console.log("ERR")
        }
    }

    removeItem = (e, item) => {
        switch(e) {
            case "discipline":
                return this.props.deleteDiscipline(this.state.masterlist.disciplines[item].disciplineID);
            case "skill":
                return this.props.deleteSkill(this.state.skill.disciplineID, item);
            case "province":
                return this.props.deleteProvince(item);
            case "city":
                return this.props.deleteCity(item, this.state.masterlist.locations[this.state.selectedprovince][item]);
            default:
                console.log("ERR")
        }
    }

    changeSelected = (elem, name, id) => {
        switch(name) {
            case "discipline":
                return this.setState({
                    ...this.state,
                    skill: {
                        ...this.state.skill,
                        disciplineID: id
                    }
                })
               
            case "skill":
                return;
            case "province":
                return this.setState({
                    ...this.state,
                    location: {
                        ...this.state.location,
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
                    <DeleteIcon name={name} onClick={()=>this.removeItem(name, elem)}/>
                    </ListItem>
                </List>
            </div>)
        })
        return list
    }

    listGenID = (inputList, name, param) => {
        let list = []
        Object.keys(inputList).forEach(elem =>{
            list.push(<div key={list.length}>
                <List>
                    <ListItem button name={name} onClick={() => this.changeSelected(elem, name, inputList[elem][param])}>
                    <ListItemText primary={elem} />
                    <DeleteIcon name={name} onClick={()=>this.removeItem(name, elem)}/>
                    </ListItem>
                </List>
            </div>)
        })
        return list
    }

    render() {
        const disciplinesObj = this.state.masterlist.disciplines
        
        var disciplineName = null
        for(var discipline in disciplinesObj) {
            if(disciplinesObj[discipline].disciplineID === this.state.skill.disciplineID){
                disciplineName = discipline
            }
        }
        
        const selectedDiscipline = disciplineName ? disciplineName : Object.keys(disciplinesObj)[0]
        const skills = disciplinesObj[selectedDiscipline] && disciplinesObj[selectedDiscipline].skills ? disciplinesObj[selectedDiscipline].skills : []
        const provinces = Object.keys(this.state.masterlist.locations)
        const locations = this.state.masterlist.locations
        const selectedProvince = this.state.selectedprovince ? this.state.selectedprovince : provinces[0]
        const cities = locations[selectedProvince] && Object.keys(locations[selectedProvince]) ? Object.keys(locations[selectedProvince]) : []

        const disciplineList = this.listGenID(disciplinesObj, "discipline", "disciplineID")
        let skillList = this.listGen(skills, "skill")
        const provinceList = this.listGen(provinces, "province", "id")
        let cityList = this.listGen(cities, "city")

        skillList = skillList.length > 0 ? skillList : <div>No Skills Available for Selected Discipline</div>
        cityList = cityList.length > 0 ? cityList : <div>No Cities Available for Selected Province</div> 
        return (
            <div className="activity-container">
                <h1 className="greenHeader">Admin</h1>
                <div>
                    <h2>Disciplines</h2>
                    {disciplineList}
                    <form name="discipline" onSubmit={this.onSubmit}>
                    <input type="text" onChange={this.handleChange} value={this.state.discipline.name} name="discipline"/>
                    </form>
                    <button name="discipline" id="discipline" onClick={this.onSubmit} >Add Discipline</button>
                </div>
                <div>
                    <h2>{selectedDiscipline} Skills</h2>
                    {skillList}
                    <form name="skill" onSubmit={this.onSubmit}>
                        <input type="text" onChange={this.handleChange} value={this.state.skill.name} name="skill"/>
                    </form>
                    <button id="skill" name="skill" onClick={this.onSubmit}>Add Skill</button>
                </div>
                <div>
                    <h2>Province</h2>
                    {provinceList}
                    <form name="province" onSubmit={this.onSubmit}>
                        <input type="text" onChange={this.handleLocationChange} value={this.state.location.province} name="province"/>
                    </form>
                    <button id="province" name="province" onClick={this.onSubmit} >Add Province</button>
                </div>
                <div>
                    <h2>{selectedProvince} Cities</h2>
                    {cityList}
                    <form name="city" onSubmit={this.onSubmit}>
                        <input type="text" onChange={this.handleLocationChange} value={this.state.location.city} name="city"/>
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
    createCity,
    deleteDiscipline,
    deleteSkill,
    deleteProvince,
    deleteCity
};
  
export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Admin)
