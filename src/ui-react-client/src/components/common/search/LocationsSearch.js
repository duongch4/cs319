import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import Select from 'react-select';


class LocationsSearch extends Component {

    state = {
      key: this.props.keyName,
      locations: 
          {
            province: null, 
            cities: [],
          },
        cityFilledIn: true,
    };
    

    handleChange = (e) => {
      if (e != null) {
        this.setState({
          ...this.state,
            locations:
              {
                ...this.state.locations,
                province: e.value,
              },
              cityFilledIn: false,
          }, () => this.props.addLocations(this.state));
      } else {
        this.setState({
          ...this.state,
            locations:
              {
                province: null,
                cities: [],
              },
              cityFilledIn: true,
          }, () => this.props.addLocations(this.state));
      }
    }
 

    handleChangeCities = (e) => {
      if (e !== null && e.length !== 0){
        if (e[0].label === "All cities"){
          var cities_return = [];
          e.map(function (e) { 
            return e.value.forEach((val, i) => {
              cities_return[i] = val;
            });
          });
          this.setState({
            locations: {
              ...this.state.locations,
              cities: cities_return,
            },
            cityFilledIn: true,
        }, () => this.props.addLocations(this.state));
      } else {
          var cities_arr = e.map(function (e) { 
            return e.value; 
          });
          this.setState({
            locations: {
              ...this.state.locations,
              cities: cities_arr
            },
            cityFilledIn: true,
         }, () => this.props.addLocations(this.state));
        }    
     } else {
      this.setState({
        locations: {
          ...this.state.locations,
          cities: []
        },
        cityFilledIn: false,
      }, () => this.props.addLocations(this.state));
     }
    }

  render(){
    var provinces = this.props.provinces; 
    var provinces_render = [];
    var province_key = [];
    Object.keys(provinces).forEach((province, i) => {
      province_key.push("province_" + i);
      var province_obj = {label: province, value: province};
      provinces_render.push(province_obj);
    });
    
    var cities = [];
    if (this.state.locations.province){
      cities =this.props.provinces[this.state.locations.province];
      var cities_format = ["all_cities"];
      var cities_key = ["all_cities"];
      var all_city = {};
      all_city['label'] = "All cities";
      all_city['value'] = [];
      
      Object.entries(cities).forEach((city, i) => {
        var single_city = {};
        single_city['label'] = city[0];
        single_city['value'] = {city: city[0], id: city[1]};
        all_city['value'].push({city: city[0], id: city[1]});
        cities_format.push(single_city);
        cities_key.push('cities_' + i);
      });
      
      cities_format.push(all_city);
      cities_key.push('all_cities');
    }

    return(
        <div className="form-row">
            <Select placeholder='Provinces' id="province" className="input-box" onChange={this.handleChange} options={provinces_render} isClearable/>
            {(this.state.cityFilledIn) && 
            (<Select id="cities" key={cities_key} className="input-box" onChange={this.handleChangeCities} options={cities_format} 
                      isMulti isClearable placeholder='Cities'/>)}
            {(!this.state.cityFilledIn) && 
            (<Select id="cities" key={cities_key} className="input-box" onChange={this.handleChangeCities} options={cities_format} isMulti isClearable
              placeholder='Must select a city' />)}
        </div>
     );
    }
  }

export default LocationsSearch;