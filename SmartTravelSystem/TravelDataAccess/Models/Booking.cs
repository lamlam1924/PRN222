using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TravelDataAccess.Models;

[Table("Booking")]
public partial class Booking
{
    [Key]
    public int BookingID { get; set; }

    public int TripID { get; set; }

    public int CustomerID { get; set; }

    public DateOnly BookingDate { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [ForeignKey("CustomerID")]
    [InverseProperty("Bookings")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("TripID")]
    [InverseProperty("Bookings")]
    public virtual Trip Trip { get; set; } = null!;
}
