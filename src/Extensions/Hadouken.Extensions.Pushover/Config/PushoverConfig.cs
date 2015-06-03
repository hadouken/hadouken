using System.ComponentModel.DataAnnotations;

namespace Hadouken.Extensions.Pushover.Config {
    public sealed class PushoverConfig {
        [DataType(DataType.Password)]
        [Display(Name = "App key", Order = 1)]
        [Required]
        public string AppKey { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "User key", Order = 2)]
        [Required]
        public string UserKey { get; set; }
    }
}