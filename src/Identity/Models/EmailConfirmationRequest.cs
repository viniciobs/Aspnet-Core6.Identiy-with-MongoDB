using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Identity.Models
{
    [ExcludeFromCodeCoverage]
    public record EmailConfirmationRequest
    {
        [Required]
        public Guid UserId { get; init; }

        [Required]
        public string Token { get; init; }
    }
}
