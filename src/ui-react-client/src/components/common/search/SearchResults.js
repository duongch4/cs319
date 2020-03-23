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
            sort: this.props.sortBy,
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

    sortUsers = () => {
        var users = [];
        this.state.userSummaries.map(function(i) {
            if (!users.some(e => e.userID === i.userID)) {
                var obj = {userID: null, firstName: "", lastName: "", location: {}, discipline: "", utilization: null};
                obj.userID = i.userID;
                obj.discipline = i.resourceDiscipline.discipline;
                obj.firstName = i.firstName;
                obj.lastName = i.lastName;
                obj.location = i.location;
                obj.utilization = i.utilization;
                users.push(obj);
            } else {
                let obj1 = users.find(o => o.userID === i.userID);
                obj1.discipline = obj1.discipline.concat(", " + i.resourceDiscipline.discipline);
            }
        });
        return users;
    }

    render(){

        if (this.state.noResults){
            return <div className="darkGreenHeader">There are no users with the selected filters</div>
        } else if ((this.state.userSummaries).length === 0 ) {
            return <div></div>
        }
        else{
            var users = this.sortUsers();
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
  