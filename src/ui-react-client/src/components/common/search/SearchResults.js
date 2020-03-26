import React, {Component} from 'react';
import {connect} from 'react-redux';
import SearchUserCard from "./SearchUserCard";
import { performUserSearch } from "../../../redux/actions/searchActions";
import {CLIENT_DEV_ENV} from '../../../config/config';
import ChevronRightIcon from '@material-ui/icons/ChevronRight';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';

class SearchResults extends Component {
    constructor(props) {
        super(props);
        this.state = {
            userSummaries: [],
            noResults: false,
            lastPage: false,
        };
    }
    
    componentDidMount() {
        if (CLIENT_DEV_ENV) {
            this.props.performUserSearch(this.props.data)
            this.setState({
                ...this.state,
                userSummaries: this.props.users,
            }, () => this.props.stopLoading());
        } else {
            this.props.performUserSearch(this.props.data)
            .then(() => {
                this.setState({
                    ...this.state,
                    userSummaries: this.props.users,
                }, this.props.stopLoading())
            }).catch(err => {
                this.setState({
                    ...this.state,
                    noResults: true,
                }, this.props.stopLoading());
            });
        }
    }

    // to make multiple calls without having to refresh
    componentDidUpdate(previousProps) {
        // check if next page has any users on it
        if (!(previousProps.data.page === this.props.data.page)) {
            this.props.performUserSearch(this.props.data)
            .then(() => {
                this.setState({
                    ...this.state,
                    lastPage: false,
                }, () => this.componentDidMount());
            }).catch(err => {
                this.setState({
                    ...this.state,
                    lastPage: true,
                    loading: false,
                }, () =>  this.props.pageLeft(), this.props.stopLoading());
            });
        } else if (!(previousProps.data === this.props.data)) {
           this.componentDidMount();
        }
    }

    combineUsers = () => {
        var users = [];
        this.state.userSummaries.map(function(i) {
            if (!users.some(e => e.userID === i.userID)) {
                var obj = {userID: null, firstName: "", lastName: "", location: {}, resourceDiscipline: [{discipline: "", yearsOfExp: ""}], utilization: null};
                obj.userID = i.userID;
                obj.firstName = i.firstName;
                obj.lastName = i.lastName;
                obj.location = i.location;
                obj.resourceDiscipline[0].discipline = i.resourceDiscipline.discipline;
                obj.resourceDiscipline[0].yearsOfExp = i.resourceDiscipline.yearsOfExp;
                obj.utilization = i.utilization;
                users.push(obj);
            } else {
                let obj1 = users.find(o => o.userID === i.userID);
                obj1.resourceDiscipline.push({discipline: i.resourceDiscipline.discipline, yearsOfExp: i.resourceDiscipline.yearsOfExp});
                obj1.resourceDiscipline.sort(function(a,b){
                    var textA = a.discipline.toUpperCase();
                    var textB = b.discipline.toUpperCase();
                    return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
                }); 
            }
        });
        return users;
    }

    sortUsers = (users) => {
        if (this.props.sortBy === "name-AZ"){
            users.sort(function(a,b){
                var textA = a.lastName.toUpperCase();
                var textB = b.lastName.toUpperCase();
                return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
            });
        } else if (this.props.sortBy === "name-ZA") {
            users.sort(function(a,b){
                var textA = a.lastName.toUpperCase();
                var textB = b.lastName.toUpperCase();
                return (textA > textB) ? -1 : (textA < textB) ? 1 : 0;
            });
        } else if (this.props.sortBy === "util-low") {
            users.sort(function(a, b){return a.utilization-b.utilization});
        } else if (this.props.sortBy === "util-high") {
            users.sort(function(a, b){return b.utilization-a.utilization});
        } else if (this.props.sortBy === "locations-AZ") {
            users.sort(function(a,b){
                var textA = a.location.city.toUpperCase();
                var textB = b.location.city.toUpperCase();
                return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
            });
        } else if (this.props.sortBy === "locations-ZA") {
            users.sort(function(a,b){
                var textA = a.location.city.toUpperCase();
                var textB = b.location.city.toUpperCase();
                return (textA > textB) ? -1 : (textA < textB) ? 1 : 0;
            });
        } else if (this.props.sortBy === "disciplines-AZ") {
            users.sort(function(a,b){
                var textA = a.resourceDiscipline[0].discipline.toUpperCase();
                var textB = b.resourceDiscipline[0].discipline.toUpperCase();
                return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
            });
        } else if (this.props.sortBy === "disciplines-ZA") {
            users.sort(function(a,b){
                var textA = a.resourceDiscipline[0].discipline.toUpperCase();
                var textB = b.resourceDiscipline[0].discipline.toUpperCase();
                return (textA > textB) ? -1 : (textA < textB) ? 1 : 0;
            });
        } else if (this.props.sortBy === "yearsOfExp-high") {
            users.sort(function(a,b){
                // need to fix sorting here - because years of experience is a string, 
                // it is sorted alphabetically so not always accurate
                var textA = a.resourceDiscipline[0].yearsOfExp.toUpperCase();
                var textB = b.resourceDiscipline[0].yearsOfExp.toUpperCase();
                return (textA > textB) ? -1 : (textA < textB) ? 1 : 0;
            });
        } else if (this.props.sortBy === "yearsOfExp-low") {
            users.sort(function(a,b){
                // here too
                var textA = a.resourceDiscipline[0].yearsOfExp.toUpperCase();
                var textB = b.resourceDiscipline[0].yearsOfExp.toUpperCase();
                return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
            });
        }
    };

    render(){
        var users = this.combineUsers();

        if (this.props.sortBy != null) {
            this.sortUsers(users);
        }

        if (this.state.noResults){
            return <div className="darkGreenHeader">There are no users with the selected filters</div>
        } else if ((this.state.userSummaries).length === 0) {
            return <div></div>
        }
        else{
            const userCards =[];
            users.forEach(user => {
            userCards.push(
            <div className="card" key={userCards.length}>
                <SearchUserCard user={user} key={userCards.length} canEdit={false}/>
            </div>)      
        });
            return (
                <div>
                    <div>
                    {(this.props.data.page == 1) && 
                    (<ChevronLeftIcon style={{color: "#E8E8E8"}}/>)}
                    {(this.props.data.page > 1) && 
                    (<ChevronLeftIcon onClick={() => this.props.pageLeft()}/>)}
                        Page {this.props.data.page}
                    {(!this.state.lastPage && (this.state.userSummaries).length == 50) && 
                    (<ChevronRightIcon onClick={() => this.props.pageRight()}/>)}
                    {(this.state.lastPage || (this.state.userSummaries).length < 50) && 
                    (<ChevronRightIcon style={{color: "#E8E8E8"}} />)}
                    </div>
                    <div>{userCards}</div>
                </div>
                
            )}
        }
}

const mapStateToProps = state => {
    return {
        users: state.users,
    };
  };
  
  const mapDispatchToProps = {
    performUserSearch
  };
  
  export default connect(
    mapStateToProps,
    mapDispatchToProps,
  )(SearchResults);
  