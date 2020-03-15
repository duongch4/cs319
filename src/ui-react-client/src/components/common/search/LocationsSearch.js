import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import { ReactDOM } from 'react-dom';

class LocationsSearch extends Component {
    constructor(props){
        super(props);
      }

    state = {
      key: this.props.keyName,
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
                      city: e.target.value,
                    }
                }, this.updateCity(e.target.value));
          } else if (e.target.id === "province") {
            let newCities = this.props.provinces[e.target.value];
            this.setState({
                ...this.state,
                  locations:
                    {
                      province: e.target.value,
                    },
                  cities: newCities,
                }, this.updateProvince(e.target.value, null));
        };
        this.props.addLocations(this.state);
    }

    updateProvince = (province, city) => {
        this.state.locations.province = province;
        this.state.locations.city = city;
    }

    updateCity = (val) => {
        this.state.locations.city = val;
    }

  render(){
    var provinces = this.props.provinces; 
    var provinces_render = [];
    var all_provinces_key = Array.from(Object.keys(provinces));
    provinces_render.push(<option value="DEFAULT" disabled>Province</option>);
    all_provinces_key.forEach((province, i) => {
      provinces_render.push(<option key={"province_" + i} value={province}>{province}</option>)
    });

    var cities = provinces[this.state.locations.province];
    var cities_render = [];
    if (this.state.locations.province === null){
        cities_render = <option disabled>Please select a province</option>
    } else {
        cities = this.state.cities;
        cities_render.push(<option value="DEFAULT" disabled>City</option>);
        Object.keys(cities).forEach((city, i) => {
        cities_render.push(<option key={"cities_" + i} value={city}>{city}</option>)
      })
    }

     return(
        <div className="form-section">
          <div className="form-row">
            <select className="input-box" defaultValue={'DEFAULT'}
                    id="province" onChange={this.handleChange}>
              {provinces_render}
            </select>
                {(this.state.locations.city == null) && 
                <select className="input-box" defaultValue={'DEFAULT'} value="DEFAULT"
                        id="city" onChange={this.handleChange}>
                    {cities_render}
                </select>}
                {(this.state.locations.city != null) && 
                <select className="input-box" defaultValue={'DEFAULT'} value={this.state.locations.city}
                        id="city" onChange={this.handleChange}>
                    {cities_render}
                </select>}
              </div>
        </div>
     );
    }
  }

export default LocationsSearch;
