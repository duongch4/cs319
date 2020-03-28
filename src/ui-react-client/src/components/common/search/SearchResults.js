import React, {Component} from 'react';
import {connect} from 'react-redux';
import SearchUserCard from "./SearchUserCard";
import { performUserSearch } from "../../../redux/actions/searchActions";
import {CLIENT_DEV_ENV} from '../../../config/config';
import {UserContext, getUserRoles} from "../userContext/UserContext";

class SearchResults extends Component {
    constructor(props) {
        super(props);
        this.state = {
            userSummaries: []
        };
    }
    
    componentDidMount() {
        if (CLIENT_DEV_ENV) {
            this.props.performUserSearch(this.props.data, ["adminUser"])
            this.setState({
                ...this.state,
                userSummaries: this.props.users,
            });
        } else {
            const userRoles = getUserRoles(this.context);
            this.props.performUserSearch(this.props.data, userRoles)
            .then(() => {
                this.setState({
                    ...this.state,
                    userSummaries: this.props.users,
                })
            })
        }
    }

    render(){
            if((this.state.userSummaries).length === 0 ){
                return <div></div>
            }  else{
                var users = this.state.userSummaries;
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
