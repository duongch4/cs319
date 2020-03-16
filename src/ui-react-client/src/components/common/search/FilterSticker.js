import React, { Component } from 'react';
import './SearchStyles.css'
import CloseIcon from '@material-ui/icons/Close';

class FilterStickers extends Component {
    constructor(props){
        super(props);
        this.state = {
            showSticker: true,
            filterLabel: this.props.label,
        }
      }

    closeTab = () => {
        console.log(this.props.type);
        this.setState({
            showSticker: false,
            filterStickers: [],
        },() => this.props.deleteFilter(this.props.type, this.props.keyId));
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
