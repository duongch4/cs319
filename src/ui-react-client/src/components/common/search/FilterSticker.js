import React, { Component } from 'react';
import './SearchStyles.css'
import CloseIcon from '@material-ui/icons/Close';

class FilterStickers extends Component {
    constructor(props){
        super();
      }

    state = {
       showSticker: true,
       filterStickers: [],
       filter: null,
    }

    closeTab = () => {
        this.setState({
            showSticker: false,
            filterStickers: [],
            render: true,
        },() => this.render());
    }

    getLocations = (locations) => {
        var loc_arr = [];
        if (locations.length == 0) {
            return loc_arr;
        } else {
            locations.forEach((location) => {
                loc_arr.push(location);
            } );
        }
        return loc_arr;
    }

    getDisciplines = (disciplines) => {
        var disciplines = {};
        if (Object.keys(disciplines).length == 0) {
            return disciplines;
        } else {
            disciplines.forEach((discipline) => {
                disciplines.push(discipline);
            } );
        }
        return disciplines;
    }

    render(){
        const filter = this.props.filter;
        var locations = filter.filter.locations; 
        var disciplines = filter.filter.disciplines; 
        var yearsOfExp = filter.filter.yearsOfExps; 
       
        var sticker_arr = [];
        if (locations.length != 0) {
            sticker_arr.push(locations);
        }
        if (Object.keys(disciplines).length != 0) {
            sticker_arr.push(disciplines);
        }
        if (yearsOfExp.length != 0) {
            sticker_arr.push(yearsOfExp);
        }
        
        console.log(this.state.showSticker);
            return (
                <div>
                </div>
            );
    }
}

export default FilterStickers
