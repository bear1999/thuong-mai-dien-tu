using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TMDT.Models
{
    public class _FullAccountInfo
    {
        public int idAccount { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(32)]
        public string Password { get; set; }

        [Required]
        [StringLength(16)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Fullname { get; set; }

        [Column(TypeName = "date")]
        public DateTime Birthday { get; set; }

        [Required]
        [StringLength(100)]
        public string Address { get; set; }

        public int idRole { get; set; }

        [Required]
        [StringLength(50)]
        public string ChucVu { get; set; }

    }
}