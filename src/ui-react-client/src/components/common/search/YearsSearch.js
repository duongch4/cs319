import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import Select from 'react-select';


class YearsSearch extends Component {
    constructor(props){
        super(props);
      }

    state = {
        key: this.props.keyName,
        yearsOfExp: null,
    };

    handleChange = (e) => {
          this.setState({
            disciplines: {
                ...this.state.disciplines,
              yearsOfExp: e.target.value,
             }
            });
            this.state.yearsOfExp = e.target.value;
            this.props.addYears(this.state);
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
    <div className="form-section">
        <div className="form-row">
        <Select id="years" key={year_key} className="input-box" onChange={this.handleChangeSkills} options={year_format} isMulti
                        placeholder='Years' />
        </div>
    </div>
        );
    }
}

export default YearsSearch;