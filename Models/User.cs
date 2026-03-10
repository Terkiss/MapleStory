using System;
using System.ComponentModel.DataAnnotations;

namespace MapleStoryMarketGraph.Models
{
    public enum AuthType
    {
        Local,
        Google
    }

    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        public string? PasswordHash { get; set; }

        public string? GoogleSubjectId { get; set; }

        public AuthType AuthType { get; set; }

        public string? NexonApiKey { get; set; }

        public string? SelectedCharacterName { get; set; }

        public string? SelectedCharacterOcid { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
