import React, { Component } from 'react';
import './SearchStyles.css'
import CloseIcon from '@material-ui/icons/Close';

class FilterStickers extends Component {
    constructor(props){
        super(props);
      }

    state = {
        id: this.props.keyId,
        showSticker: true,
        filterLabel: this.props.label,
    }

    closeTab = () => {
        this.setState({
            showSticker: false,
            filterStickers: [],
            render: true,
        },() => this.props.deleteFilter());
    }

    render(){ 
        const {showing} = this.state.showSticker;
            return (
                <div className="filter-sticker" style={{display: (showing ? 'none' : 'block')}}>
                    {this.state.filterLabel}
                    <CloseIcon onClick={()=> this.closeTab()}/>
                </div>
            );
    }
}

export default FilterStickers
