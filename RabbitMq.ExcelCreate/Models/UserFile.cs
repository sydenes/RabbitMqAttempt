using System.ComponentModel.DataAnnotations.Schema;

namespace RabbitMq.ExcelCreate.Models
{
    public enum FileStatus
    {
        Created,
        Processing,
        Completed,
        Failed
    }
    public class UserFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string UserId { get; set; }
        public string? FilePath { get; set; }
        public DateTime? CreatedAt { get; set; }
        public FileStatus Status { get; set; }

        [NotMapped] //If we don't want to save this property in the database, we can use the NotMapped attribute.
        public string GetCreatedAt => CreatedAt.HasValue ? CreatedAt.Value.ToString("dd.MM.yyyy HH:mm:ss") : string.Empty;

    }
}
