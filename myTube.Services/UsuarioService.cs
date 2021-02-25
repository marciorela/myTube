using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using myTube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace myTube.Services
{
    public class UsuarioService
    {
        private readonly IHttpContextAccessor accessor;

        public UsuarioService(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        public Guid Id
        {
            get
            {
                if (IsAuthenticated())
                    return Guid.Parse(accessor.HttpContext.User.FindFirst(ClaimTypes.Sid).Value);
                else
                    return Guid.Empty;
            }
        }

        public string Email
        {
            get
            {
                return accessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            }
        }

        public string Nome
        {
            get
            {
                return accessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            }
        }

        public bool IsAuthenticated()
        {
            return accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public async Task SignIn(Usuario usuario)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Sid, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nome),
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                "name", ""
                );

            var principal = new ClaimsPrincipal(identity);
            await accessor.HttpContext.SignInAsync(principal, new AuthenticationProperties()
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(7),
            });
        }

        public async Task SignOut()
        {
            await accessor.HttpContext.SignOutAsync();
        }

    }
}
