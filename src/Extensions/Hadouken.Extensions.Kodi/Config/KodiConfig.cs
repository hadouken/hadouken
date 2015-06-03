using System;
using System.ComponentModel.DataAnnotations;

namespace Hadouken.Extensions.Kodi.Config {
    public sealed class KodiConfig {
        [DataType(DataType.Url)]
        [Display(Name = "Url", Order = 1)]
        [Required]
        public Uri Url { get; set; }

        [Display(Name = "Enable authentication", Order = 2)]
        public bool EnableAuthentication { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Username", Order = 3)]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password", Order = 4)]
        public string Password { get; set; }
    }
}