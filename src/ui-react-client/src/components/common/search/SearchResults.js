import React, {Component} from 'react';
import {connect} from 'react-redux';
import SearchUserCard from "./SearchUserCard";
import { performUserSearch } from "../../../redux/actions/searchActions";
import {CLIENT_DEV_ENV} from '../../../config/config';

class SearchResults extends Component {
    constructor(props) {
        super(props);
        this.state = {
            userSummaries: [],
            noResults: false,
        };
    }
    
    componentDidMount() {
        if (CLIENT_DEV_ENV) {
            this.props.performUserSearch(this.props.data)
            this.setState({
                ...this.state,
                userSummaries: this.props.users,
            });
        } else {
            this.props.performUserSearch(this.props.data)
            .then(() => {
                this.setState({
                    ...this.state,
                    userSummaries: this.props.users,
                })
            }).catch(err => {
                this.setState({
                    ...this.state,
                    noResults: true,
                });
            });
        }
    }

    combineUsers = () => {
        var users = [];
        this.state.userSummaries.map(function(i) {
            if (!users.some(e => e.userID === i.userID)) {
                var obj = {userID: null, firstName: "", lastName: "", location: {}, discipline: "", utilization: null, yearsOfExp: []};
                obj.userID = i.userID;
                obj.discipline = i.resourceDiscipline.discipline;
                obj.firstName = i.firstName;
                obj.lastName = i.lastName;
                obj.location = i.location;
                obj.utilization = i.utilization;
                obj.yearsOfExp = [i.resourceDiscipline.yearsOfExp];
                users.push(obj);
            } else {
                let obj1 = users.find(o => o.userID === i.userID);
                obj1.discipline = obj1.discipline.concat(", " + i.resourceDiscipline.discipline);
                obj1.yearsOfExp.push(i.resourceDiscipline.yearsOfExp);
                obj1.yearsOfExp.sort();
            }
        });
        return users;
    }

    sortUsers = (users) => {
        if (this.props.sortBy === "util-low") {
            users.sort(function(a, b){return a.utilization-b.utilization});
        } else if (this.props.sortBy === "util-high") {
            users.sort(function(a, b){return b.utilization-a.utilization});
        } else if (this.props.sortBy === "locations-AZ") {
            users.sort(function(a){return a.location});
        } else if (this.props.sortBy === "locations-ZA") {
            users.sort(function(a){return a.location});
            users.reverse();
        } else if (this.props.sortBy === "disciplines-AZ") {
            users.sort(function(a){return a.discipline});
        } else if (this.props.sortBy === "disciplines-ZA") {
            users.sort(function(a){return a.discipline});
            users.reverse();
        } else if (this.props.sortBy === "yearsOfExp-high") {
            users.sort(function(a){return a.yearsOfExp});
        } else if (this.props.sortBy === "yearsOfExp-low") {
            users.sort(function(a){return a.yearsOfExp});
            users.reverse();
        }
    };

    render(){
        var users = this.combineUsers();

        if (this.props.sortBy != null) {
            this.sortUsers(users);
        }

        if (this.state.noResults){
            return <div className="darkGreenHeader">There are no users with the selected filters</div>
        } else if ((this.state.userSummaries).length === 0 ) {
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
                <div>{userCards}</div>
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
  