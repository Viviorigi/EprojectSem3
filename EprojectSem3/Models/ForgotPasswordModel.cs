using System.ComponentModel.DataAnnotations;

namespace Realtors_Portal.Models
{
	public class ForgotPasswordModel
	{
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email address.")]
		[StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
		public string Email { get; set; }
	}
}
