using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskOneDraft.Models
{
    public class FilesModel
    {
        [Key]
        public int FileId { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public long Length { get; set; }
        public string ContentType { get; set; }
        [ForeignKey("Claims")]
        public int ClaimId { get; set; }
        public Claims Claims { get; set; }
    }
}
