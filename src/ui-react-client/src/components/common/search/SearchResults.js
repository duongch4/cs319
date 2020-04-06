import React, {Component} from 'react';
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
            noResultsNextPage: false,
            currPage: 1,
            userSummariesAll: [],
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
        var data = this.props.data;
        if (CLIENT_DEV_ENV) {
            this.props.performUserSearch(data, ["adminUser"])
            this.setState({
                ...this.state,
                userSummaries: this.props.users,
                noResults: false,
                currPage: 1,
            }, () => this.props.stopLoading());
        } else {
            const userRoles = getUserRoles(this.context);
            this.props.performUserSearch(data, userRoles)
            .then(() => {
                this.setState({
                    ...this.state,
                    userSummaries: this.props.users,
                    noResults: false,
                    currPage: 1,
                    userSummariesAll: [this.props.users],
                }, () => (
                    this.props.users.isLastPage ? (this.props.stopLoading()) : (this.getAll(userRoles, 1))
                    ))
            })
        }
    }

    // to make multiple calls without having to refresh
    componentDidUpdate(previousProps) {
        if (previousProps.data !== this.props.data) {
            this.newSearch();
        }
    }

    newSearch = () => {
        var data = this.props.data;
        const userRoles = getUserRoles(this.context);
        this.props.performUserSearch(data, userRoles)
        .then(() => {
            this.setState({
                ...this.state,
                userSummaries: this.props.users,
                noResults: false,
                currPage: 1,
                userSummariesAll: [this.props.users],
            }, () => (
                this.props.users.isLastPage ? (this.props.stopLoading()) : (this.getAll(userRoles, 1))
                ))
        })
    }

    getAll(userRoles, currPage) {
        if (!this.state.userSummariesAll[this.state.userSummariesAll.length - 1].isLastPage) {
            var mock_data = JSON.parse(JSON.stringify(this.props.data));
            mock_data.page = currPage + 1;
            this.props.performUserSearch(mock_data, userRoles)
            .then(() => {
                this.setState({
                    ...this.state,
                    userSummariesAll: [...this.state.userSummariesAll, this.props.users],
                    noResults: false,
                }, () => (this.props.users.isLastPage ? this.setState({
                    ...this.state,
                    noResultsNextPage: true,
                },() => this.props.stopLoading()) : this.getAll(userRoles, mock_data.page)))
            })
        }
        else {
            this.setState({
                ...this.state,
                noResultsNextPage: true,
            },() => this.props.stopLoading());
        }
    }

    toNextPage = () => {
        var new_page = this.state.currPage + 1;
        var page_index = this.state.currPage;
        this.setState({
            ...this.state,
            userSummaries: this.state.userSummariesAll[page_index],
            currPage: new_page,
        })
      }
    
    toPrevPage = () => {
        var new_page = this.state.currPage - 1;
        var page_index = new_page - 1;
        this.setState({
            ...this.state,
            userSummaries: this.state.userSummariesAll[page_index],
            currPage: new_page,
        })
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
                return users;
            } else {
                let obj1 = users.find(o => o.userID === i.userID);
                obj1.resourceDiscipline.push({discipline: i.resourceDiscipline.discipline, yearsOfExp: i.resourceDiscipline.yearsOfExp});
                obj1.resourceDiscipline.sort(function(a,b){
                    var textA = a.discipline.toUpperCase();
                    var textB = b.discipline.toUpperCase();
                    return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
                }); 
                return obj1
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
                    <SearchUserCard user={user}
                    key={userCards.length}
                    canEdit={false}
                    isAssignable={this.props.isAssignable}
                    projectNumber={this.props.projectNumber}
                    openingId={this.props.openingId}
                    createAssignOpenings={(openingId, userId, utilization, user, userRoles) => 
                    this.props.createAssignOpenings(openingId, userId, utilization, user, userRoles)}/>
                    )});
            return (
                <div>
                    <div className="pagination-controls">
                    {(this.state.currPage === 1) && 
                    (<ChevronLeftIcon style={{color: "#E8E8E8"}}/>)}
                    {(this.state.currPage > 1) && 
                    (<ChevronLeftIcon onClick={() => this.toPrevPage()}/>)}
                        Page {this.state.currPage}
                    {(this.state.noResultsNextPage && !this.state.userSummaries.isLastPage) && 
                    (<ChevronRightIcon onClick={() => this.toNextPage()}/>)}
                    {(!this.state.noResultsNextPage || this.state.userSummaries.isLastPage) && 
                    (<ChevronRightIcon style={{color: "#E8E8E8"}} />)}
                    </div>
                    <hr />
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
