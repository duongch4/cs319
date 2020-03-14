import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import YearsSearch from './YearsSearch';

class AddYears extends Component {
    constructor(props){
        super(props);
        this.state = {
            status: {},
            count: 1,
            view: [],
        }
      }

      newYears = () => {
        var keyId = ("year_" + (this.state.count + 1));
        var newYear = (
            <div className="form-row" key={keyId} >
            <input className="add" type="button" value="-" onClick={()=> this.deleteDiscipline(keyId)}/>
            <YearsSearch yearsOfExp={this.props.yearsOfExp}
                                addYears={this.addYears}
                                keyName={keyId}/>
           </div>
           );
           this.setState( {
            status: {...this.state.status, [keyId]: null},
            count: this.state.count + 1,
            view: [...this.state.view, newYear],
        })
        this.state.status = {...this.state.status, [keyId]: null};
        this.state.count = this.state.count + 1;
        this.state.view = [...this.state.view, newYear];
      }
      
      addYears = (state) => {
            var key = state.key;
            var years = state.yearsOfExp;
             this.setState({
                status: Object.assign({}, this.state.status, {[key]: years}),
            });
            this.state.status =  Object.assign({}, this.state.status, {[key]: years});
            this.props.updateYears(this.state.status);
        }

        deleteDiscipline = (keyId) => {
            delete this.state.status[keyId];
            var view_arr = this.state.view.slice();
            var mockState = this.state.view.slice();
            view_arr.forEach((location, index) => {
                if (location.key === keyId) {
                    mockState.splice(index, 1);
                    this.setState({
                        ...this.state,
                        view: mockState,
                    });
                    this.state.view = mockState;
                }
            });
            }

      render(){
          return(
            <div>
            <div className="form-row" key="disciplineSearch">
            <input className="add" type="button" value="+" onClick={()=> this.newYears()}/>
            <YearsSearch yearsOfExp={this.props.yearsOfExp}
                                addYears={this.addYears}
                                keyName={"disciplineSearch"}/>
            </div>
            {this.state.view}
            </div>
            );
      }
    };



export default AddYears;