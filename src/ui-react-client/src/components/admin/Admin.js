import React, { Component}  from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import {CLIENT_DEV_ENV} from '../../config/config';
import {loadMasterlists, createDiscpline} from '../../redux/actions/masterlistsActions';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';

class Admin extends Component {
    state = {
        discipline: {
            name: "", 
            id: -1
        },
        skill: {
            disciplineID: -1, 
            name: "", 
            skillID: -1
        },
        location: {
            city: "", 
            province: "", 
            id: -1
        },
        masterlist: {}
    };

    static getDerivedStateFromProps(props, state){
        if(CLIENT_DEV_ENV){
            props.loadMasterlists();
            return{
                masterlist: props.masterlist
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

    handleChange = (e) => {
        this.setState({
            [e.target.name]: e.target.value
        })
    }

    onSubmit = (e) =>  {
        e.preventDefault();
        let targetname = e.target.name
        // db call
        // console.log(this.state[targetname])
        this.props.createDiscpline(this.state[targetname])
    }

    render() {
        // console.log(this.state)
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
                    <button id="skill">Add Skill</button>
                </div>
                <div>
                    <h2>Province</h2>
                    {provinceList}
                    <button id="province">Add Province</button>
                </div>
                <div>
                    <h2>Cities</h2>
                    {cityList}
                    <button id="city">Add City</button>
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
    createDiscpline
};
  
export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Admin)
