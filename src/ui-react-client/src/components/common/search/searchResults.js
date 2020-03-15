import React, {Component} from 'react';
import {connect} from 'react-redux';
// import {performUserSearch} from "../../redux/actions/userProfileActions";
import SearchUserCard from "./SearchUserCard";

class SearchResults extends Component {
    state = {
        userSummaries: {}
    };

    render(){
        var users = this.props.data;
        const userCards =[];
        
        users.forEach(user => {
            userCards.push(
                <div className="card" key={userCards.length}>
                    <SearchUserCard user={user} key={userCards.length} canEdit={false}/>
                </div>)      
            });
          
          
        return (
            <div>{userCards}</div>
        )
    }

}

export default connect(

  )(SearchResults);