using System.ComponentModel.DataAnnotations;

namespace Hadouken.Extensions.Pushalot.Config {
    public sealed class PushalotConfig {
        [DataType(DataType.Password)]
        [Display(Name = "Authorization token")]
        [Required]
        public string AuthorizationToken { get; set; }
    }
}