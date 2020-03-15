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
        });
        this.state.showSticker = false;
        this.state.render = true;
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
        var locations = filter.filter.locations; // arr of objects 
        var disciplines = filter.filter.disciplines; // object {name: skills, name: skills}
        var yearsOfExp = filter.filter.yearsOfExps; //arr
       
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
        

        // if (filterStickers.length != 0) {
        //     var filter_render = [];
        //     filter_render.push(
        //     <div className="filter-sticker" key={filterStickers.length} style={{color:"white", display: (showSticker ? 'block' : 'none')}}>
        //     {filterStickers[1]}, {filterStickers[0]}
        //     <CloseIcon onClick={()=> this.closeTab(location)}/>
        //     </div>
        //     )
        // }
        console.log(this.state.showSticker);
            return (
                <div>
                hi
                </div>
            );
    }
}

export default FilterStickers
