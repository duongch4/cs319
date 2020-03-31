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
    deleteCity,
    clearError} from '../../redux/actions/masterlistsActions';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import DeleteIcon from '@material-ui/icons/Delete'
import {UserContext, getUserRoles} from "../common/userContext/UserContext";
import Loading from '../common/Loading';
import LoadingOverlay from 'react-loading-overlay'
import ClipLoader from 'react-spinners/ClipLoader'

class Admin extends Component {
    constructor(props){
        super(props);
        this.removeItem = this.removeItem.bind(this);
        this.state = {
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
            },
            sending: false
        };
    }
    

    componentDidMount(){
        if(this.state.masterlist.yearsOfExp.length === 0){
            if(CLIENT_DEV_ENV){
                this.props.loadMasterlists(['adminUser'])
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
            } else {
                const userRoles = getUserRoles(this.context);
                this.props.loadMasterlists(userRoles)
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
        if(this.props.masterlist.error) {
            this.setState({
                sending:false
            })
            this.props.clearError();
        }
        if (this.props.masterlist.disciplines) {
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
                    sending: false
                })
            }
            else if(prevProps.masterlist !== this.props.masterlist){
                this.setState({
                    ...this.state,
                    masterlist: this.props.masterlist,
                    sending: false
                })
            }
        }
    }

    handleChange = (e) => {
        this.setState({
            [e.target.name]: {
                ...this.state[e.target.name],
                name: e.target.value
            }
        })
    }

    handleLocationChange = (e) => {
        this.setState({
            location: {
                ...this.state.location,
                [e.target.name]: e.target.value
            }
        })
    }

    onSubmit = (e) => {
        e.preventDefault();
        let userRoles;
        if (CLIENT_DEV_ENV) {
            userRoles = ['adminUser'];
        } else {
            userRoles = getUserRoles(this.context);
        }
        switch(e.target.name) {
            case "discipline":
                const discipline = this.state.discipline;
                this.props.createDiscpline(discipline, userRoles);
                this.setState({
                    ...this.state,
                    discipline: {
                        name: "",
                        id: 0
                    },
                    sending: true
                })
                
                return;
            case "skill":
                const skill = this.state.skill;
                this.props.createSkill(skill, userRoles);
                this.setState({
                    ...this.state,
                    skill:{
                        ...this.state.skill,
                        name: ""
                    },
                    sending: true
                })
                return ;
            case "province":
                const location = this.state.location;
                this.props.createProvince(location, userRoles);
                this.setState({
                    ...this.state,
                    location:{
                        ...this.state.location,
                        province: ""
                    },
                    sending: true
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
                this.props.createCity(this.state.location, userRoles)
                this.setState({
                    ...this.state,
                    location:{
                        ...this.state.location,
                        city: "",
                        province: "",
                        sending: true
                    }
                })})

                
                return;
            default:
                console.log("ERR")
        }
    }

    removeItem(e, item) {
        let userRoles;
        if (CLIENT_DEV_ENV) {
            userRoles = ['adminUser'];
        } else {
            userRoles = getUserRoles(this.context);
        }
        switch(e) {
            case "discipline":
                this.setSending();
                return this.props.deleteDiscipline(
                    this.state.masterlist.disciplines[item].disciplineID,
                    userRoles
                );
            case "skill":
                this.setSending();
                return this.props.deleteSkill(
                    this.state.skill.disciplineID, item, userRoles
                );
            case "province":
                this.setSending();
                return this.props.deleteProvince(item, userRoles);
            case "city":
                this.setSending();
                return this.props.deleteCity(
                    item,
                    this.state.masterlist.locations[this.state.selectedprovince][item],
                    userRoles
                );
            default:
                console.log("ERR")
        }
    }

    setSending = () => {
        this.setState({
            ...this.state,
            sending: true
        })
    }

    changeSelected = (elem, name, id) => {
        switch(name) {
            case "discipline":
                return this.setState({
                    skill: {
                        ...this.state.skill,
                        disciplineID: id
                    }
                })
               
            case "skill":
                return;
            case "province":
                return this.setState({
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
        return list;
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
        return list;
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
            <LoadingOverlay active={this.state.sending} spinner={<ClipLoader />}>
                <h1 className="greenHeader">Admin</h1>
                <div>
                    <h2>Disciplines</h2>
                    {disciplineList.length === 0 ? <Loading /> : disciplineList}
                    <form name="discipline" onSubmit={this.onSubmit}>
                    <input type="text" onChange={this.handleChange} value={this.state.discipline.name} name="discipline"/>
                    </form>
                    <button name="discipline" id="discipline" onClick={this.onSubmit} >Add Discipline</button>
                </div>
                <div>
                    <h2>{selectedDiscipline} Skills</h2>
                    {disciplineList.length === 0 ? <Loading /> : skillList}
                    <form name="skill" onSubmit={this.onSubmit}>
                        <input type="text" onChange={this.handleChange} value={this.state.skill.name} name="skill"/>
                    </form>
                    <button id="skill" name="skill" onClick={this.onSubmit}>Add Skill</button>
                </div>
                <div>
                    <h2>Province</h2>
                    {provinceList.length === 0 ? <Loading /> : provinceList}
                    <form name="province" onSubmit={this.onSubmit}>
                        <input type="text" onChange={this.handleLocationChange} value={this.state.location.province} name="province"/>
                    </form>
                    <button id="province" name="province" onClick={this.onSubmit} >Add Province</button>
                </div>
                <div>
                    <h2>{selectedProvince} Cities</h2>
                    {provinceList.length === 0 ? <Loading /> : cityList}
                    <form name="city" onSubmit={this.onSubmit}>
                        <input type="text" onChange={this.handleLocationChange} value={this.state.location.city} name="city"/>
                    </form>
                    <button id="city" name="city" onClick={this.onSubmit}>Add City</button>
                </div>
                </LoadingOverlay>
            </div>
        )
    }
}

Admin.propTypes = {
    masterlist: PropTypes.object.isRequired
};

Admin.contextType = UserContext;

const mapStateToProps = state => {
    return {
        masterlist: state.masterlist,
        error: state.masterlist.error,
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
    deleteCity,
    clearError
};
  
export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Admin)
