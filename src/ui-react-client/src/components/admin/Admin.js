import React, { Component}  from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import {CLIENT_DEV_ENV} from '../../config/config';
import {loadMasterlists} from '../../redux/actions/masterlistsActions';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';

class Admin extends Component {
    state = {
        // discipline: {
        //     name: "", 
        //     id: -1
        // },
        // skill: {
        //     disciplineID: -1, 
        //     name: "", 
        //     skillID: -1
        // },
        // location: {
        //     city: "", 
        //     province: "", 
        //     id: -1
        // },
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

        let disciplineList = []
        disciplines.forEach(elem =>{
            disciplineList.push(<div key={disciplineList.length}>
                <List>
                    <ListItem button>
                    <ListItemText primary={elem} />
                    </ListItem>
                </List>
            </div>)
        })

        let skillList = []
        skills.forEach(elem =>{
            skillList.push(<div key={skillList.length}>
                <List>
                    <ListItem button>
                    <ListItemText primary={elem} />
                    </ListItem>
                </List>
            </div>)
        })
 
        let provinceList = []
        provinces.forEach(elem =>{
            provinceList.push(<div key={provinceList.length}>
                <List>
                    <ListItem button>
                    <ListItemText primary={elem} />
                    </ListItem>
                </List>
            </div>)
        })

        let cityList = []
        cities.forEach(elem =>{
            cityList.push(<div key={cityList.length}>
                <List>
                    <ListItem button>
                    <ListItemText primary={elem} />
                    </ListItem>
                </List>
            </div>)
        })
        
        return (
            <div className="activity-container">
                <h1 className="greenHeader">Admin</h1>
                <div>
                    <h2>Disciplines</h2>
                    {disciplineList}
                </div>
                <div>
                    <h2>Skills</h2>
                    {skillList}
                </div>
                <div>
                    <h2>Province</h2>
                    {provinceList}
                </div>
                <div>
                    <h2>Cities</h2>
                    {cityList}
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
    loadMasterlists
};
  
export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(Admin)
