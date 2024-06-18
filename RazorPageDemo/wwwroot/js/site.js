"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalRServer")
    .build();

//connection.on("LoadCourses", function () {
//    location.href='/Courses'
//});
//connection.on("LoadInstructors", function () {
//    location.href='/Instructors'
//});
connection.on("LoadBooking", function () {
    location.href ='/Index'
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});