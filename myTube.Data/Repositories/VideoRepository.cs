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
    public class VideoRepository : BaseRepository
    {
        public VideoRepository(AppDbContext ctx) : base(ctx)
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

        public async Task<IEnumerable<Filme>> GetByStatus(Guid usuarioId, EStatusVideo status)
        {
            return await _ctx.Filmes
                .Include(i => i.Canal)
                .Where(v => v.Canal.UsuarioId == usuarioId && v.Status == status)
                .OrderBy(v => v.PublishedAt)
                .ToListAsync();
        }
    }
}
