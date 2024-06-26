using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GuestBookRepos.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public int Id_User { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime MessageDate { get; set; }

        public string Email { get; set; }

        public string WWW { get; set; }

        [ForeignKey("Id_User")]
        public User User { get; set; }
    }
}

