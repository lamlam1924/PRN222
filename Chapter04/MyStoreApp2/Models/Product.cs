using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyStoreApp2.Models;

public partial class Product
{
    [Key]
    public int ProductID { get; set; }

    [StringLength(40)]
    public string ProductName { get; set; } = null!;

    public int? CategoryID { get; set; }

    public short? UnitsInStock { get; set; }

    [Column(TypeName = "money")]
    public decimal? UnitPrice { get; set; }

    [ForeignKey("CategoryID")]
    [InverseProperty("Products")]
    public virtual Category? Category { get; set; }
}
