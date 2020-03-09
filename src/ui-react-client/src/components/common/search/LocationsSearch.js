import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import { ReactDOM } from 'react-dom';


class LocationsSearch extends Component {
    constructor(props){
        // this.addDisciplines = this.addDisciplines.bind(this);
        super();
      }

    state = {
      locations:
        {
            province: null, 
            city: null,
        },
      cities: [],
    };

    handleChange = (e) => {
        if (e.target.id === "city") {
            this.setState({
                ...this.state,
                  locations:
                    {
                        ...this.state.locations,
                      city: e.target.value,
                    }
                });
          } else if (e.target.id === "province") {
            let newCities = this.props.provinces[e.target.value];
            this.setState({
                ...this.state,
                  locations:
                    {
                        ...this.state.locations,
                      province: e.target.value,
                    },
                  cities: newCities,
                });
        };
        console.log(this.state.locations);
        this.props.addLocations(this.state.locations);
    }

    handleSubmit = (e) =>{
      e.preventDefault();
    //   this.props.addOpening(this.state.opening);
    //   this.props.addLocations(this.state.locations);
    };

  render(){
    var provinces = this.props.provinces; 

    var provinces_render = [];
    var all_provinces_key = Array.from(Object.keys(provinces));
    all_provinces_key.forEach((province, i) => {
      provinces_render.push(<option key={"province_" + i} value={province}>{province}</option>)
    });

    var cities = provinces[this.state.locations.province];
    var cities_render = [];
    if (this.state.locations.province === null){
      cities_render = <option disabled>Please select a province</option>
    } else {
      cities = this.state.cities;
      cities.forEach((city, i) => {
        cities_render.push(<option key={"cities_" + i} value={city}>{city}</option>)
      })
    }

     return(
        <div className="form-section">
            <div className="form-row">
            <select className="input-box" defaultValue={'DEFAULT'}
                        id="province" onChange={this.handleChange}>
                    <option value="DEFAULT" disabled>Province</option>
                    {provinces_render}
                </select>
                <select className="input-box" defaultValue={'DEFAULT'}
                        id="city" onChange={this.handleChange}>
                    <option value="DEFAULT" disabled>City</option>
                    {cities_render}
                </select>
            
        </div>
        </div>
     );
       
    }
}

export default LocationsSearch;
