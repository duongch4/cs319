import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import Select from 'react-select';


class YearsSearch extends Component {
    constructor(props){
        super(props);
      }

    state = {
        yearsOfExp: [],
    };

    handleChange = (e) => {
        var years_arr = e.map(function (e) { return e.label; });
          this.setState({
              ...this.state,
            yearsOfExp: years_arr,
         })
        this.state.yearsOfExp = years_arr;
        this.props.updateYears(this.state.yearsOfExp);
        };

  render(){
  
    var year_format = [];
    var year_key = [];
    this.props.yearsOfExp.forEach((year, i) => {
    var single_year = {};
    single_year['label'] = year;
    single_year['value'] = year;
    year_format.push(single_year);
    });

    return (
        <div>
<div className="form-section">
        <div className="form-row">
        <Select id="years" key={year_key} className="input-box" onChange={this.handleChange} options={year_format} isMulti
                        placeholder='Years' />
        </div>
    </div>
        </div>
    
        );
    }
}

export default YearsSearch;