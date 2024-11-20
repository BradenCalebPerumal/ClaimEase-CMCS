using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskOneDraft.Models
{
    public class Claims
    {
        public int ID { get; set; }

        [Required]
        public string LecturerID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime ClaimsPeriodStart { get; set; }

        [Required]
        public DateTime ClaimsPeriodEnd { get; set; }

        [Required]
        public double HoursWorked { get; set; }

        [Required]
        public double RateHour { get; set; }

        [Required]
        public double TotalAmount { get; set; }

        public string? DescriptionOfWork { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string SupportingDocuments { get; set; }

        // Property to link claims to the logged-in lecturer by UserID
        [Required]
        public string UserID { get; set; }

        // Add a property for Claim Status with a default value of 'Pending'
        [Required]
        [StringLength(20)]
        public string ClaimStatus { get; set; } = "Pending"; // Set default status to "Pending"
                                                             // Property to store the date and time the claim was submitted
        public DateTime DateSubmitted { get; set; } = DateTime.Now;  // Automatically set to the current date and time
        public virtual ICollection<FilesModel> File { get; set; }

        public Claims()
        {
            File = new HashSet<FilesModel>();
        }

    }
}

