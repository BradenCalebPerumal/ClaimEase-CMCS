using System.ComponentModel.DataAnnotations.Schema;//for mapping database table relationships
using System.ComponentModel.DataAnnotations;//for adding validation attributes

namespace TaskOneDraft.Models
{
    public class Claims//represents the Claims entity model
    {
        public int ID { get; set; }//primary key for the Claims table

        [Required]//makes the LecturerID property mandatory
        public string LecturerID { get; set; }//stores the unique ID of the lecturer

        [Required]//makes the FirstName property mandatory
        public string FirstName { get; set; }//stores the first name of the lecturer

        [Required]//makes the LastName property mandatory
        public string LastName { get; set; }//stores the last name of the lecturer

        [Required]//makes the Email property mandatory
        public string Email { get; set; }//stores the email address of the lecturer

        [Required]//makes the ClaimsPeriodStart property mandatory
        public DateTime ClaimsPeriodStart { get; set; }//start date of the claims period

        [Required]//makes the ClaimsPeriodEnd property mandatory
        public DateTime ClaimsPeriodEnd { get; set; }//end date of the claims period

        [Required]//makes the HoursWorked property mandatory
        public double HoursWorked { get; set; }//total hours worked by the lecturer

        [Required]//makes the RateHour property mandatory
        public double RateHour { get; set; }//hourly rate of pay for the lecturer

        // Overtime fields
        public double? OvertimeHours { get; set; }//stores optional overtime hours
        public double? OvertimeRate { get; set; }//stores optional overtime rate
        public double OvertimePay => (OvertimeHours ?? 0) * (OvertimeRate ?? 0);//calculates overtime pay dynamically

        [Required]//makes the TotalAmount property mandatory
        public double TotalAmount { get; set; }//total claimable amount

        public string? DescriptionOfWork { get; set; }//optional description of the work done

        [Required]//makes the SupportingDocuments property mandatory
        [DataType(DataType.MultilineText)]//specifies the data type for multiline text input
        public string SupportingDocuments { get; set; }//stores links or paths to supporting documents

        [Required]//makes the UserID property mandatory
        public string UserID { get; set; }//user ID of the lecturer submitting the claim

        [Required]//makes the ClaimStatus property mandatory
        [StringLength(20)]//limits the maximum length of the status string
        public string ClaimStatus { get; set; } = "Pending";//default status for new claims

        public DateTime DateSubmitted { get; set; } = DateTime.Now;//stores the submission date, defaulting to the current date

        public virtual ICollection<FilesModel> File { get; set; }//represents related files associated with the claim

        public Claims()//constructor to initialize the File collection
        {
            File = new HashSet<FilesModel>();//initializes the collection as an empty set
        }
    }
}
