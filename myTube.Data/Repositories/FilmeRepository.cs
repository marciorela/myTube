using Microsoft.EntityFrameworkCore;
using myTube.Data.Base;
using myTube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Data.Repositories
{
    public class FilmeRepository : BaseRepository
    {
        public FilmeRepository(AppDbContext ctx) : base(ctx)
        {
        }

        public async Task<Filme> GetByYoutubeId(string youtubeId, Guid usuarioId)
        {
            return await _ctx.Filmes
                .Include(c => c.Canal)
                .Where(f => f.YoutubeFilmeId == youtubeId && f.Canal.UsuarioId == usuarioId)
                .FirstOrDefaultAsync();
        }

        public async Task Add(Filme filme)
        {
            await _ctx.Filmes.AddAsync(filme);
            await _ctx.SaveChangesAsync();
        }
    }
}
