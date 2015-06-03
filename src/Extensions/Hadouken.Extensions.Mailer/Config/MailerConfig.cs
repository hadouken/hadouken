using System.ComponentModel.DataAnnotations;

namespace Hadouken.Extensions.Mailer.Config {
    public sealed class MailerConfig {
        [DataType(DataType.Text)]
        [Display(Name = "Host", Order = 1)]
        [Required]
        public string Host { get; set; }

        [Display(Name = "Port", Order = 2)]
        [Required]
        public int Port { get; set; }

        [Display(Name = "Enable SSL", Order = 3)]
        [Required]
        public bool EnableSsl { get; set; }

        [Display(Name = "Username", Order = 4)]
        [Required]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password", Order = 5)]
        [Required]
        public string Password { get; set; }

        [Display(Name = "From", Order = 6)]
        [Required]
        public string From { get; set; }

        [Display(Name = "To", Order = 7)]
        [Required]
        public string To { get; set; }
    }
}