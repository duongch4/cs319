import React from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';

const MonthlyDetails = ({commitment}) => {

    let months = [];
    let hours = [];
    const monthArr = []
    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        ];
    let start = new Date(commitment[0][0]);
    let end = new Date(commitment[commitment.length - 1][0]);
    let startIndex = start.getMonth() + 1;
    let startYear = start.getYear();
    const endIndex = end.getMonth() + 1;
    const endYear = end.getYear();
    
    let hoursCount = 0;
    while((startIndex <= endIndex || startYear < endYear) && !(startYear > endYear)){
        const year = 1900 + startYear;
        months.push(monthNames[startIndex] + " " + year);
        hours.push(commitment[hoursCount][1]);
        startIndex++;
        if(startIndex > 11){
            startIndex = 0;
            startYear++;
        }
        hoursCount++;
    }

    months.forEach((month, index) => {
        monthArr.push(
        <div className="entry" key={monthArr.length} >
            <p className="label"  style={{color: "#2c6232"}}><b>{month}</b></p>
            <p className="label">{hours[index]} hrs</p>
        </div>
        )
    })

    return (
        <div>
        <p><b>Expected Number of Hours per Month:</b></p>
        <div className="flex">{monthArr}</div>
        </div>
       );
    
}

    MonthlyDetails.propTypes = {
        commitment: PropTypes.array,
      };
      
      export default MonthlyDetails;
