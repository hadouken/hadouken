namespace Hadouken.Extensions.AutoMove.Data.Models
{
    public sealed class Parameter
    {
        public int Id { get; set; }

        public int RuleId { get; set; }

        public ParameterSource Source { get; set; }

        public string Pattern { get; set; }
    }
}
