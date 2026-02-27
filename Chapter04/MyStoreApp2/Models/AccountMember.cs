using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyStoreApp2.Models;

[Table("AccountMember")]
[Index("EmailAddress", Name = "UQ__AccountM__49A14740C99F428E", IsUnique = true)]
public partial class AccountMember
{
    [Key]
    [StringLength(20)]
    public string MemberID { get; set; } = null!;

    [StringLength(80)]
    public string MemberPassword { get; set; } = null!;

    [StringLength(80)]
    public string FullName { get; set; } = null!;

    [StringLength(100)]
    public string? EmailAddress { get; set; }

    public int? MemberRole { get; set; }
}
