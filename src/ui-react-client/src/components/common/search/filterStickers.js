import React, { Component } from 'react';
import './SearchStyles.css'
import CloseIcon from '@material-ui/icons/Close';

class FilterStickers extends Component {
    constructor(props){
        super(props);
      }

    state = {
       showSticker: true,
       key: this.props.keyId,
       type: this.props.type,
       label: this.props.filterName,
    }

    closeTab = () => {
        this.props.deleteFilter(this.state.key, this.state.type);
    }

    render(){
        const showSticker = this.state.showSticker;
            return (
                <div className="filter-sticker" style={{color:"white", display: (showSticker ? 'block' : 'none')}}>
                 {this.state.label}
                    <CloseIcon onClick={()=> this.closeTab()}/>
                   </div>
            );
    }
}

export default FilterStickers
