import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import { ReactDOM } from 'react-dom';
import LocationsSearch from "./LocationsSearch";

class AddLocation extends Component {
    constructor(props){
        super(props);
        this.state = {
            status: [{"locationSearch": {province: null, city: null}}],
            count: 1,
            view: [],
            curr: [],
        }
      }

      newLocations = () => {
        var keyId = ("location_" + (this.state.count + 1));
        console.log(keyId);
        var newLoc = (
            <div className="form-row" key={keyId} >
            <input className="add" type="button" value="-" onClick={()=> this.deleteLocation(keyId)}/>
            <LocationsSearch provinces={this.props.locations}
                                            addLocations={this.addLocations}
                                            keyName={keyId}/>  
           </div>
           );
           this.setState( {
            status: this.state.status.concat([{[keyId]: {province: null, city: null}}]),
            count: this.state.count + 1,
            view: [...this.state.view, newLoc],
        })
        this.state.count = this.state.count + 1;
      }
      
      addLocations = (state) => {
            this.setState({
                curr: this.state.curr.concat([{[state.key]: state.locations}]),
            });
            this.state.curr = [state.key = state.locations];
        }

        deleteLocation = (keyId) => {
            var status_arr = this.state.status;
            var view_arr = this.state.view;
            var curr_arr = this.state.curr;
            curr_arr.forEach((curr, index) => {
                if (Object.keys(curr) == keyId) {
                    this.state.curr.splice(index,1);
                    this.setState({
                        ...this.state,
                        curr: this.state.curr.splice(index,1),
                    });
                }
            });
            status_arr.forEach((component, index) => {
                if (Object.keys(component) == keyId) {
                    this.state.status.splice(index, 1);
                    this.setState({
                        ...this.state,
                        status: this.state.status.splice(index, 1),
                    });
                }
            });
            view_arr.forEach((location, index) => {
                if (location.key == keyId) {
                    this.state.view.splice(index, 1);
                    this.setState({
                        ...this.state,
                        view: this.state.view.splice(index, 1),
                    });
                }
            });
            }

      render(){
          console.log(this.state);
          return(
                <div>
                    <input className="add" type="button" value="+" onClick={()=> this.newLocations()}/>
                <div key="locationSearch">
                    <LocationsSearch provinces={this.props.locations}
                                 addLocations={this.addLocations}
                                 keyName={"locationSearch"}/>  
                </div>
                {this.state.view}
              </div>
            );
      }
    };

export default AddLocation;
