import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import { ReactDOM } from 'react-dom';
import Select from 'react-select';


class LocationsSearch extends Component {
    constructor(props){
        super(props);
      }

    state = {
      key: this.props.keyName,
      locations: 
          {
            province: null, 
            cities: [],
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

    handleChangeCities = (e) => {
      if (e){
        var cities_arr = e.map(function (e) { return e.label; });
          this.setState({
            locations: {
              ...this.state.locations,
              cities: cities_arr
            }
         })
        }
      this.state.locations.cities = cities_arr;
      this.props.addLocations(this.state);
     };

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

    var cities = [];
    if (this.state.locations.province){
      cities =this.props.provinces[this.state.locations.province];
      var cities_format = [];
      var cities_key = [];
      Object.keys(cities).forEach((city, i) => {
        var single_city = {};
        single_city['label'] = city;
        single_city['value'] = city;
        cities_format.push(single_city);
        cities_key.push('skills_' + i);
      });
    }

     return(
        <div className="form-section">
          <div className="form-row">
            <select className="input-box" defaultValue={'DEFAULT'}
                    id="province" onChange={this.handleChange}>
              {provinces_render}
            </select>
            <Select id="cities" key={cities_key} className="input-box" onChange={this.handleChangeCities} options={cities_format} isMulti
                            placeholder='Cities' />
              </div>
        </div>
     );
    }
  }

export default LocationsSearch;
