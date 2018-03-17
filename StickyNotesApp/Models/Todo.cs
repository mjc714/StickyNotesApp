using System;
using System.ComponentModel.DataAnnotations;

namespace StickyNotesApp.Models
{
    public class Todo
    {
        // ID of a Todo.
        public int ID { get; set; }

        // user ID from AspNetUser table.
        public string OwnerID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsDone { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expire Date")]
        public DateTime? ExpireDate { get; set; }
    }
}
