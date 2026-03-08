using System.Security.Claims;
using Domain.Entities;

namespace Application.Interfaces;

public interface ITokenService
{
    string GenerarToken(Usuario usuario);
    string GenerarRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
