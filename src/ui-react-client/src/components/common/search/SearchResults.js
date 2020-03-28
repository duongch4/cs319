import React, {Component, useContext} from 'react';
import {connect} from 'react-redux';
import SearchUserCard from "./SearchUserCard";
import { performUserSearch } from "../../../redux/actions/searchActions";
import {CLIENT_DEV_ENV} from '../../../config/config';
import ChevronRightIcon from '@material-ui/icons/ChevronRight';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';
import {UserContext, getUserRoles} from "../userContext/UserContext";

class SearchResults extends Component {
    constructor(props) {
        super(props);
        this.yearsMap = this.getYears(this.props.master.yearsOfExp);
        this.state = {
            userSummaries: [],
            noResults: false,
            lastPage: false,
        };
    }

    getYears = (yearsArr) => {
        var arr = {};
        yearsArr.forEach((year) => {
            var digits = year.replace(/(^\d+)(.+$)/i,'$1');
            arr = {...arr, [year]: parseInt(digits)};
        });
        return arr;
    }
    
    componentDidMount() {
        if (CLIENT_DEV_ENV) {
            this.props.performUserSearch(this.props.data, ["adminUser"])
            this.setState({
                ...this.state,
                userSummaries: this.props.users,
                noResults: false,
            }, () => this.props.stopLoading());
        } else {
            const userRoles = getUserRoles(this.context);
            this.props.performUserSearch(this.props.data, userRoles)
            .then(() => {
                this.setState({
                    ...this.state,
                    userSummaries: this.props.users,
                    noResults: false,
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
        if (previousProps.data.page !== this.props.data.page) {
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

    // combines users when there is a single user with more than one discipline
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

    sortUsers = (users, yearMap) => {
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
                var numA = yearMap[a.resourceDiscipline[0].yearsOfExp];
                var numB = yearMap[b.resourceDiscipline[0].yearsOfExp];
                return numB - numA;
            });
        } else if (this.props.sortBy === "yearsOfExp-low") {
            users.sort(function(a,b){
                var numA = yearMap[a.resourceDiscipline[0].yearsOfExp];
                var numB = yearMap[b.resourceDiscipline[0].yearsOfExp];
                return numA - numB;
            });
        }
    };

    render(){
        var users = this.combineUsers();

        if (this.props.sortBy != null) {
            this.sortUsers(users, this.yearsMap);
        }

        if (this.state.noResults){
            return <div className="darkGreenHeader">There are no users with the selected filters</div>
        } else if ((this.state.userSummaries).length === 0) {
            return <div></div>
        } else{
            const userCards =[];
            users.forEach((user) => {
                userCards.push(
                <div className="card" key={userCards.length}>
                    <SearchUserCard user={user}
                    key={userCards.length}
                    canEdit={false}
                    isAssignable={this.props.isAssignable}
                    projectNumber={this.props.projectNumber}
                    openingId={this.props.openingId}
                    createAssignOpenings={(openingId, userId, utilization, user, userRoles) => 
                    this.props.createAssignOpenings(openingId, userId, utilization, user, userRoles)}/>
                    </div>)}); 
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

SearchResults.contextType = UserContext;

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
