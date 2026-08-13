using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities.Errors;

public static class IdentityErrors
{
    public static readonly Error InvalidCredentials =
        Error.UnAuthorized(
            "Identity.InvalidCredentials",
            "Invalid email or password.");

    public static readonly Error EmailAlreadyExists =
        Error.Conflict(
            "Identity.EmailAlreadyExists",
            "Email is already registered.");

    public static readonly Error UserNotFound =
        Error.NotFound(
            "Identity.UserNotFound",
            "User was not found.");

    public static readonly Error EmailNotConfirmed =
        Error.Forbidden(
            "Identity.EmailNotConfirmed",
            "Email address has not been confirmed. Confirm your email before logging in.");

    public static readonly Error InvalidVerificationCode =
        Error.Validation(
            "Identity.InvalidVerificationCode",
            "Invalid or expired verification code.");

    public static readonly Error EmailAlreadyConfirmed =
        Error.Conflict(
            "Identity.EmailAlreadyConfirmed",
            "Email address is already confirmed.");

    public static readonly Error EmailSendFailed =
        Error.Unavailable(
            "Identity.EmailSendFailed",
            "Our email service is currently unavailable. Please try again later.");

    public static readonly Error InvalidRefreshToken =
        Error.UnAuthorized(
            "Identity.InvalidRefreshToken",
            "Invalid refresh token.");

    public static readonly Error RefreshTokenExpired =
        Error.UnAuthorized(
            "Identity.RefreshTokenExpired",
            "Refresh token has expired. Please sign in again.");

    public static Error IdentityValidationFailed(string details) =>
        Error.Validation(
            "Identity.ValidationFailed", 
            details);

    public static Error RoleDoesNotExist(string role) =>
        Error.NotFound(
            "Identity.RoleDoesNotExist", 
            $"Role '{role}' does not exist.");

    public static Error RoleAlreadyGranted(string role) =>
        Error.Conflict(
            "Identity.RoleAlreadyGranted", 
            $"User already has the role '{role}' and everything it includes.");

    public static Error RoleNotAssigned(string role) =>
        Error.Conflict(
            "Identity.RoleNotAssigned", 
            $"User does not have the role '{role}'.");

    public static readonly Error UnexpectedFailure =
        Error.Failure(
            "Identity.UnexpectedFailure", 
            "An unexpected error occurred. Please try again later.");

    public static Error InvalidResetInput(string message) =>
        Error.Validation(
            "Identity.InvalidResetInput", 
            message);

    // don't split these into more specific errors later without checking whether doing
    // so would let an attacker distinguish "expired token" from "token/email mismatch,"
    public static readonly Error InvalidOrExpiredResetLink =
        Error.Validation(
            "Identity.InvalidOrExpiredResetLink", 
            "This password reset link is invalid or has expired.");

    public static readonly Error RegistrationAlreadyStarted =
        Error.Conflict(
            "Identity.RegistrationAlreadyStarted",
            "An account with this email already exists but hasn't been confirmed yet. Use the resend-verification endpoint to get a new code.");
}
