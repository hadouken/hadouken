using System.ComponentModel.DataAnnotations;

namespace Hadouken.Extensions.Pushbullet.Config {
    public sealed class PushbulletConfig {
        [DataType(DataType.Password)]
        [Display(Name = "Access token")]
        [Required]
        public string AccessToken { get; set; }
    }
}