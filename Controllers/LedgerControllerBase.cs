using System.Security.Claims;
using GroupProject325.Data;
using GroupProject325.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupProject325.Controllers;

public abstract class LedgerControllerBase : Controller
{
    protected readonly LedgerContext _context;

    protected LedgerControllerBase(LedgerContext context)
    {
        _context = context;
    }

    // Reads the provider name and provider user ID from the 
    // signed-in OAuth identity.
    protected (string? Provider, string? Subject) GetOAuthIdentity()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return (claim?.Issuer, claim?.Value);
    }

    // Finds the User row that matches the signed-in OAuth identity.
    protected async Task<User?> GetCurrentUserAsync()
    {
        var (provider, subject) = GetOAuthIdentity();
        if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(subject))
            return null;

        return await _context.Users.FirstOrDefaultAsync(u =>
            u.OAuthProvider == provider && u.OAuthSubject == subject);
    }
}