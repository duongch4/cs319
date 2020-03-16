import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import DisciplineSearch from "./DisciplineSearch";

class AddDisciplines extends Component {
    constructor(props){
        super(props);
        this.state = {
            status: {"disciplineSearch":{
                name: null,
                skills: [],
                yearsOfExp: null,
                }},
            count: 1,
            view: [],
            curr: [],
        }
      }

      newDisciplines = () => {
        var keyId = ("discipline_" + (this.state.count + 1));
        var newDisc = (
            <div className="form-row" key={keyId} >
            <input className="add" type="button" value="-" onClick={()=> this.deleteDiscipline(keyId)}/>
            <DisciplineSearch disciplines={this.props.disciplines}
                                masterYearsOfExperience={this.props.yearsOfExp}
                                addDisciplines={this.addDisciplines}
                                keyName={keyId}/>
           </div>
           );
           this.setState( {
            status: {...this.state.status, [keyId]:{
                name: null,
                skills: [],
                yearsOfExp: null,
                }},
            count: this.state.count + 1,
            view: [...this.state.view, newDisc]})
      }
      
      addDisciplines = (state) => {
            var key = state.key;
            var disciplines = state.disciplines;
             this.setState({
                status: Object.assign({}, this.state.status, {[key]: disciplines}),
            }, () => this.props.updateDisciplines(Object.values(this.state.status)));
        }

        deleteDiscipline = (keyId) => {
            var status_mock = this.state.status; 
            delete status_mock[keyId];
            var view_arr = this.state.view.slice();
            var mockState = this.state.view.slice();
            view_arr.forEach((location, index) => {
                if (location.key === keyId) {
                    mockState.splice(index, 1);
                    this.setState({
                        ...this.state,
                        status: status_mock,
                        view: mockState,
                    });
                }
            });
            }

      render(){
          return(
            <div>
            <div className="form-row" key="disciplineSearch">
            <input className="add" type="button" value="+" onClick={()=> this.newDisciplines()}/>
            <DisciplineSearch disciplines={this.props.disciplines}
                                masterYearsOfExperience={this.props.yearsOfExp}
                                addDisciplines={this.addDisciplines}
                                keyName={"disciplineSearch"}/>
            </div>
            {this.state.view}
            </div>
            );
      }
    };



export default AddDisciplines;