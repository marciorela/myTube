using Microsoft.EntityFrameworkCore;
using myTube.Data.Base;
using myTube.Domain.Entities;
using myTube.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Data.Repositories
{
    public class UsuarioRepository : BaseRepository
    {
        public UsuarioRepository(AppDbContext ctx) : base(ctx)
        {
        }

        public async Task<IEnumerable<Usuario>> ListAtivos()
        {
            return await _ctx
                .Usuarios
                .Where(u => u.Status == EStatusUsuario.Ativo)
                .ToListAsync();
        }

        public async Task<Usuario> GetByEmailAndPassoword(string email, string password)
        {
            return await _ctx
                .Usuarios
                .Where(u => u.Email == email && u.Password == password)
                .FirstOrDefaultAsync();
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _ctx
                .Usuarios
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _ctx.Usuarios.AddAsync(usuario);
            await _ctx.SaveChangesAsync();
        }
    }
}
