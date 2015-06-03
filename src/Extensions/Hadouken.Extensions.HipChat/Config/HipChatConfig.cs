using System.ComponentModel.DataAnnotations;

namespace Hadouken.Extensions.HipChat.Config {
    public sealed class HipChatConfig {
        [DataType(DataType.Password)]
        [Display(Name = "Auth. token")]
        [Required]
        public string AuthenticationToken { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Room")]
        [Required]
        public string RoomId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "From")]
        [Required]
        public string From { get; set; }

        [Display(Name = "Notify")]
        public bool Notify { get; set; }
    }
}