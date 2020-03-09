import React, { Component } from 'react';
import './SearchStyles.css'
import CloseIcon from '@material-ui/icons/Close';

class FilterStickers extends Component {
    state = {
       showSticker: true,
       filterStickers: []
    }

    render(){
        const {showSticker} = this.state;
        const {filters} = this.props;
        const {filterStickers} = this.state;
        filters.forEach(filter => {
            this.state.filterStickers.push(
            <div className="filter-sticker" key={filterStickers.length} style={{color:"white", display: (showSticker ? 'block' : 'none')}}>
            {filter}
            <CloseIcon onClick={()=> this.setState({showSticker: false})}/>
            </div>
            )
            console.log(filterStickers);
        });
        console.log(filters);
            return (
                <div>
                {filterStickers}
                </div>
            );
    }
}

export default FilterStickers
