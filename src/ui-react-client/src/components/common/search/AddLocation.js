import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import LocationsSearch from "./LocationsSearch";

class AddLocation extends Component {
    constructor(props){
        super(props);
        this.state = {
            status: {"locationSearch": {province: null, city: null}},
            count: 1,
            view: [],
        }
      }

      newLocations = () => {
        var keyId = ("location_" + (this.state.count + 1));
        var newLoc = (
            <div className="form-row" key={keyId} >
            <input className="add" type="button" value="-" onClick={()=> this.deleteLocation(keyId)}/>
            <LocationsSearch provinces={this.props.locations}
                                            addLocations={this.addLocations}
                                            keyName={keyId}/>  
           </div>
           );
           this.setState( {
            status: {...this.state.status, [keyId]:{province: null, city: null} },
            count: this.state.count + 1,
            view: [...this.state.view, newLoc],
        });
      }
      
      addLocations = (state) => {
            var key = state.key;
            var location = state.locations;
             this.setState({
                status: Object.assign({}, this.state.status, {[key]: location}),
            }, () => this.props.updateLocations(Object.values(this.state.status)));
        }

        deleteLocation = (keyId) => {
            var status_mock = this.state.status;
            delete status_mock[keyId];
            var view_arr = this.state.view.slice();
            var mockState = this.state.view.slice();
            view_arr.forEach((location, index) => {
                if (location.key === keyId) {
                    mockState.splice(index, 1);
                    this.setState({
                        ...this.state,
                        status: status_mock,
                        view: mockState,
                    });
                }
            });
            }

    render(){
          return(
              <div>
                <div className="form-row" key={"locationSearch"} >
                    <input className="add" type="button" value="+" onClick={()=> this.newLocations()}/>
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
