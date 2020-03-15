import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
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
    this.setState({
        ...this.state,
          locations:
            {
              ...this.state.locations,
              province: e.target.value,
            },
        }, this.props.addLocations(this.state));
    }
 

    handleChangeCities = (e) => {
      if (e){
        var cities_arr = e.map(function (e) { return e.value; });
          this.setState({
            locations: {
              ...this.state.locations,
              cities: cities_arr
            }
         },this.props.addLocations(this.state));
        }
     };

  render(){
    var provinces = this.props.provinces; 
    var provinces_render = [];
    var all_provinces_key = Object.keys(provinces);
    provinces_render.push(<option value="DEFAULT" disabled>Province</option>);
    all_provinces_key.forEach((province, i) => {
      provinces_render.push(<option key={"province_" + i} value={province}>{province}</option>)
    });

    var cities = [];
    if (this.state.locations.province){
      cities =this.props.provinces[this.state.locations.province];
      var cities_format = [];
      var cities_key = [];
      Object.entries(cities).forEach((city, i) => {
        var single_city = {};
        single_city['label'] = city[0];
        single_city['value'] = {city: city[0], id: city[1]};
        cities_format.push(single_city);
        cities_key.push('cities_' + i);
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
