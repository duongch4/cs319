import React, {useEffect} from 'react';
import {loadMasterlists} from "../../../redux/actions/masterlistsActions";
import {connect} from 'react-redux';
import {CLIENT_DEV_ENV} from '../../../config/config';
import FilterTab from "./filterTab";

const Search = ({
  masterlist,
  loadMasterlists,
}) => {
  useEffect(() => {
      if (CLIENT_DEV_ENV) {
        loadMasterlists()
      } else {
        loadMasterlists()
        .catch(error => {
          alert('Loading projects failed' + error);
        });
      }
  }, [masterlist, loadMasterlists]);
  return (
      <div className="activity-container">
        <h2 className="greenHeader">Search</h2>
        <FilterTab />
      </div>
      );
};

const mapStateToProps = state => {
  return {
    masterlist: state.masterlist,
  };
};

const mapDispatchToProps = {
  loadMasterlists
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(Search);
