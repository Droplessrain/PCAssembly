using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCAssembly
{
    public partial class Review
    {
        public int ReviewId { get; set; }

        [Required(ErrorMessage = "Assembly is required.")]
        [ForeignKey("Assembly")]
        public int AssemblyId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Review text cannot exceed 1000 characters.")]
        public string? ReviewText { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? Rating { get; set; }

        [Required]
        public virtual Assembly Assembly { get; set; } = null!;

        [Required]
        public virtual User User { get; set; } = null!;
    }
}
