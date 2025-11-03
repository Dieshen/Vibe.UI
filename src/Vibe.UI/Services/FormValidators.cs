using System.Text.RegularExpressions;

namespace Vibe.UI.Services;

/// <summary>
/// Common validation functions for form inputs.
/// </summary>
public static class FormValidators
{
    /// <summary>
    /// Validates that a value is not null or empty.
    /// </summary>
    public static Func<T?, string?> Required<T>(string? fieldName = null)
    {
        return value =>
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return $"{fieldName ?? "This field"} is required";
            }
            return null;
        };
    }

    /// <summary>
    /// Validates that a string has a minimum length.
    /// </summary>
    public static Func<string?, string?> MinLength(int minLength, string? fieldName = null)
    {
        return value =>
        {
            if (value != null && value.Length < minLength)
            {
                return $"{fieldName ?? "This field"} must be at least {minLength} characters";
            }
            return null;
        };
    }

    /// <summary>
    /// Validates that a string has a maximum length.
    /// </summary>
    public static Func<string?, string?> MaxLength(int maxLength, string? fieldName = null)
    {
        return value =>
        {
            if (value != null && value.Length > maxLength)
            {
                return $"{fieldName ?? "This field"} must be no more than {maxLength} characters";
            }
            return null;
        };
    }

    /// <summary>
    /// Validates that a value is within a numeric range.
    /// </summary>
    public static Func<T?, string?> Range<T>(T min, T max, string? fieldName = null) where T : IComparable<T>
    {
        return value =>
        {
            if (value != null)
            {
                if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
                {
                    return $"{fieldName ?? "This field"} must be between {min} and {max}";
                }
            }
            return null;
        };
    }

    /// <summary>
    /// Validates that a string is a valid email address.
    /// </summary>
    public static Func<string?, string?> Email(string? fieldName = null)
    {
        return value =>
        {
            if (!string.IsNullOrEmpty(value))
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(value))
                {
                    return $"{fieldName ?? "This field"} must be a valid email address";
                }
            }
            return null;
        };
    }

    /// <summary>
    /// Validates that a string matches a regex pattern.
    /// </summary>
    public static Func<string?, string?> Pattern(string pattern, string errorMessage)
    {
        return value =>
        {
            if (!string.IsNullOrEmpty(value))
            {
                var regex = new Regex(pattern);
                if (!regex.IsMatch(value))
                {
                    return errorMessage;
                }
            }
            return null;
        };
    }

    /// <summary>
    /// Validates that a string is a valid URL.
    /// </summary>
    public static Func<string?, string?> Url(string? fieldName = null)
    {
        return value =>
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                {
                    return $"{fieldName ?? "This field"} must be a valid URL";
                }
            }
            return null;
        };
    }

    /// <summary>
    /// Validates that a string is a valid phone number.
    /// </summary>
    public static Func<string?, string?> Phone(string? fieldName = null)
    {
        return value =>
        {
            if (!string.IsNullOrEmpty(value))
            {
                var phoneRegex = new Regex(@"^\+?[\d\s\-\(\)]+$");
                if (!phoneRegex.IsMatch(value) || value.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Length < 10)
                {
                    return $"{fieldName ?? "This field"} must be a valid phone number";
                }
            }
            return null;
        };
    }

    /// <summary>
    /// Validates that a value matches another value (useful for password confirmation).
    /// </summary>
    public static Func<T?, string?> Matches<T>(Func<T?> otherValue, string otherFieldName = "password")
    {
        return value =>
        {
            var other = otherValue();
            if (value != null && !value.Equals(other))
            {
                return $"Must match {otherFieldName}";
            }
            return null;
        };
    }

    /// <summary>
    /// Combines multiple validators into one.
    /// </summary>
    public static Func<T?, string?> Combine<T>(params Func<T?, string?>[] validators)
    {
        return value =>
        {
            foreach (var validator in validators)
            {
                var error = validator(value);
                if (error != null)
                {
                    return error;
                }
            }
            return null;
        };
    }

    /// <summary>
    /// Validates password strength.
    /// </summary>
    public static Func<string?, string?> StrongPassword(
        int minLength = 8,
        bool requireUppercase = true,
        bool requireLowercase = true,
        bool requireDigit = true,
        bool requireSpecialChar = true)
    {
        return value =>
        {
            if (string.IsNullOrEmpty(value))
            {
                return null; // Use Required validator if needed
            }

            if (value.Length < minLength)
            {
                return $"Password must be at least {minLength} characters";
            }

            if (requireUppercase && !value.Any(char.IsUpper))
            {
                return "Password must contain at least one uppercase letter";
            }

            if (requireLowercase && !value.Any(char.IsLower))
            {
                return "Password must contain at least one lowercase letter";
            }

            if (requireDigit && !value.Any(char.IsDigit))
            {
                return "Password must contain at least one number";
            }

            if (requireSpecialChar && !value.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return "Password must contain at least one special character";
            }

            return null;
        };
    }

    /// <summary>
    /// Validates that a credit card number is valid (using Luhn algorithm).
    /// </summary>
    public static Func<string?, string?> CreditCard(string? fieldName = null)
    {
        return value =>
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var cardNumber = value.Replace(" ", "").Replace("-", "");

            if (!cardNumber.All(char.IsDigit))
            {
                return $"{fieldName ?? "Card number"} must contain only digits";
            }

            // Luhn algorithm
            int sum = 0;
            bool alternate = false;
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int digit = cardNumber[i] - '0';

                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }

                sum += digit;
                alternate = !alternate;
            }

            if (sum % 10 != 0)
            {
                return $"{fieldName ?? "Card number"} is not valid";
            }

            return null;
        };
    }

    /// <summary>
    /// Validates that a date is in the future.
    /// </summary>
    public static Func<DateTime?, string?> FutureDate(string? fieldName = null)
    {
        return value =>
        {
            if (value.HasValue && value.Value <= DateTime.Now)
            {
                return $"{fieldName ?? "Date"} must be in the future";
            }
            return null;
        };
    }

    /// <summary>
    /// Validates that a date is in the past.
    /// </summary>
    public static Func<DateTime?, string?> PastDate(string? fieldName = null)
    {
        return value =>
        {
            if (value.HasValue && value.Value >= DateTime.Now)
            {
                return $"{fieldName ?? "Date"} must be in the past";
            }
            return null;
        };
    }
}
