import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";


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
    var range_render = [];
    this.props.yearsOfExp.forEach((yearsOfExperience, i) => {
        range_render.push(<option key={"yearsOfExperience_" + i} value={yearsOfExperience}>{yearsOfExperience}</option>)
    });

        return (
        <div className="form-section">
                <div className="form-section opening">
            <label className="form-row" htmlFor= "yearsOfExp">
                <select className="input-box" defaultValue={'DEFAULT'}
                        id="yearsOfExp" onChange={this.handleChange}>
                    <option value="DEFAULT" disabled>Select a range</option>
                    {range_render}
                </select>
            </label>
        </div>
        </div>
        );
    }
}

export default YearsSearch;