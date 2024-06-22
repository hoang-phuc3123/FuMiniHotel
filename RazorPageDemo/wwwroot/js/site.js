"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalRServer")
    .build();

connection.on("LoadBooking", function () {
    location.href = '/BookingHistory'
});

connection.on("LoadRoom", function () {
    location.href = '/Booking'
})

//LoadRoom();

//function LoadRoom(DateTime startDate, DateTime endDate) {
//    var tr = '';
//    $.ajax({
//        url: '/Booking/GetRoom(startDate, endDate)',
//        method: 'GET',
//        success: (result) => {
//            $.each(result, (k, v) => {
//                tr +=
//                //`<tr>
//                //        <td> ${v.ProdName} </td> 
//                //        <td> ${v.Category} </td>
//                //        <td> ${v.UnitPrice} </td>
//                //        <td> ${v.StockQty} </td>
//                //        <td>
//                //            <a href='../Products/Edit?id=${v.ProdId}'> Edit </a> | 
//                //            <a href='../Products/Details?id=${v.ProdId}'> Details </a> | 
//                //            <a href='../Products/Delete?id=${v.ProdId}'> Delete </a>
//                //        </td>
//                //        </tr>`
//                        `<tr>
//                                    <td>${(v.Rooms.IndexOf(room) + 1)}</td>
//                                    <td>${v.RoomNumber}</td>
//                                    <td>${v.RoomDetailDescription}</td>
//                                    <td>${v.RoomMaxCapacity}</td>
//                                    <td>${v.RoomPricePerDay}</td>
//                                    <td>${v.RoomTypeName}</td>
//                                    <td>
//                                        <input type="checkbox" id="roomCheckbox_${v.RoomNumber}" name="SelectedRooms" value="${v.RoomNumber}" />
//                                    </td>
//                                </tr>`
//            })

//            $("#roomTableBody").html(tr);
//        },
//        error: (error) => {
//            console.log(error)
//        }

//    });
//}

connection.start().catch(function (err) {
    return console.error(err.toString());
});