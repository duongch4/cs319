import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import { ReactDOM } from 'react-dom';


class LocationsSearch extends Component {
    state = {
      locations: [
        {
            province: null, 
            city: null,
        },
      ],
      locationsAvailable: {
          count: 1,
      },
      cities: [],
    };

    handleChange = (e) => {
        if (e.target.id === "city") {
            this.setState({
                ...this.state,
                  locations:
                  [
                    {
                    ...this.state.locations,
                      city: e.target.value,
                    }
                  ]
                });
          } else if (e.target.id === "province") {
            let newCities = this.props.provinces[e.target.value];
            this.setState({
                ...this.state,
                  locations:
                  [
                    {
                    ...this.state.locations,
                      province: e.target.value,
                    }
                  ],
                  cities: newCities,
                });
        };
    }

    closeDiscipline(id) {
        this.setState({
            ...this.state.locations,
            locationsAvailable: {
                count: (this.state.locationsAvailable.count) - 1
            }
        });

    };

    handleSubmit = (e) =>{
      e.preventDefault();
    //   this.props.addOpening(this.state.opening);
      this.setState({
          ...this.state.locations,
          locationsAvailable: {
              count: (this.state.locationsAvailable.count) + 1
          }
      })
    };

  render(){
    var provinces = this.props.provinces; 

    var provinces_render = [];
    var all_provinces_key = Array.from(Object.keys(provinces));
    all_provinces_key.forEach((province, i) => {
      provinces_render.push(<option key={"province_" + i} value={province}>{province}</option>)
    });

    var cities = [];
    var cities_render = [];
    if (this.state.locations[0].province === null){
      cities_render = <option disabled>Please select a province</option>
    } else {
      cities = this.state.cities;
      cities.forEach((city, i) => {
        cities_render.push(<option key={"cities_" + i} value={city}>{city}</option>)
      })
    }

    var inputType = null; 
    var locationsHTML = [];
    for (var i = 1; i <= this.state.locationsAvailable.count; i++) {
        var id = "locations_" + i;
        if (i <= 1){
            inputType = (<input className="add" type="submit" value="+"/>);
        } else {
            inputType = (<input className="add" type="submit" value="-" onClick={()=> this.closeDiscipline("locations_" + i)}/>);
        }

        locationsHTML.push(<div className="form-section">
        <label htmlFor= "location" className="form-row">
            {inputType}
                <select className="input-box" id="province" onChange={this.handleChange}>
                    {provinces_render}
                </select>
                <select className="input-box" id="city" onChange={this.handleChange}>
                    {cities_render}
                </select>
            </label>
        </div>);
    }

     return(
        <div className="form-section">
            <h2 key={1} className="darkGreenHeader">Locations</h2>
                <form onSubmit={this.handleSubmit}>
                    {locationsHTML}
                </form>
        </div>
     );
       
    }
}

export default LocationsSearch;
