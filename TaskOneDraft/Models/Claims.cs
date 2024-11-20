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

        // Overtime fields
        public double? OvertimeHours { get; set; } // Nullable, as it's optional
        public double? OvertimeRate { get; set; } // Nullable, as it's optional
        public double OvertimePay => (OvertimeHours ?? 0) * (OvertimeRate ?? 0); // Calculated field

        [Required]
        public double TotalAmount { get; set; }

        public string? DescriptionOfWork { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string SupportingDocuments { get; set; }

        [Required]
        public string UserID { get; set; }

        [Required]
        [StringLength(20)]
        public string ClaimStatus { get; set; } = "Pending";

        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        public virtual ICollection<FilesModel> File { get; set; }

        public Claims()
        {
            File = new HashSet<FilesModel>();
        }
    }

}

