using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TravelDataAccess.Models;

[Table("Trip")]
[Index("Code", Name = "UQ__Trip__A25C5AA7F21B81BA", IsUnique = true)]
public partial class Trip
{
    [Key]
    public int TripID { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [StringLength(200)]
    public string Destination { get; set; } = null!;

    [Column(TypeName = "decimal(12, 2)")]
    public decimal Price { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [InverseProperty("Trip")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
