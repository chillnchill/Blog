using System.ComponentModel.DataAnnotations;

namespace YourProject.CustomAttributes
{
	public class StrongPasswordAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			string? password = value as string;

			if (string.IsNullOrWhiteSpace(password))
			{
				return new ValidationResult("Password is required.");
			}

			if (password.Length < 6 || !password.Any(char.IsUpper) || !password.Any(char.IsDigit))
			{
				return new ValidationResult("Password must be at least 6 characters long, contain at least one uppercase letter, and one digit.");
			}

			return ValidationResult.Success;
		}
	}
}
