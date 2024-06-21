using System;
using System.Collections.Generic;

namespace DataModel.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string CustomerFullName { get; set; }

    public string Telephone { get; set; }

    public string EmailAddress { get; set; }

    public DateTime? CustomerBirthday { get; set; }

    public byte? CustomerStatus { get; set; }

    public string Password { get; set; }
    public int? EmailVerifyCode { get; set; }
    public DateTime? ExpiredCode { get; set; }

    public virtual ICollection<BookingReservation> BookingReservations { get; set; } = new List<BookingReservation>();
}
