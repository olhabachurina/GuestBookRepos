using System.ComponentModel.DataAnnotations;

namespace GuestBookRepos.Models
{
    public class MessageViewModel
    {
        [Required(ErrorMessage = "The Name field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Url(ErrorMessage = "Invalid URL.")]
        public string WWW { get; set; }

        [Required(ErrorMessage = "The Content field is required.")]
        public string Content { get; set; }
        public User User { get; set; }  
        public DateTime MessageDate { get; set; }
    }
}
