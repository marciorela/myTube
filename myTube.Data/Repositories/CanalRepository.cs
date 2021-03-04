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
    public class CanalRepository : BaseRepository
    {
        public CanalRepository(AppDbContext ctx) : base(ctx)
        {
        }

        public async Task<IEnumerable<Canal>> GetAtivos(Guid usuarioId)
        {
            return await _ctx.Canais
                .Where(c => c.Status == EStatusCanal.Ativo && c.UsuarioId == usuarioId)
                .Include(u => u.Usuario)
                .OrderBy(c => c.UltimaBusca)
                .ToListAsync();
        }

        public async Task<IEnumerable<Canal>> GetByStatus(EStatusCanal status)
        {
            return await _ctx.Canais
                .Where(c => c.Status == status)
                .Include(u => u.Usuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<Canal>> GetAll(Guid usuarioId)
        {
            return await _ctx.Canais
                .Where(c => c.UsuarioId == usuarioId)
                .OrderBy(c => c.Title)
                .ToListAsync();
        }

        public async Task Add(Canal canal)
        {
            await _ctx.Canais.AddAsync(canal);
            await _ctx.SaveChangesAsync();
        }

        public async Task<Canal> GetByYoutubeId(Guid usuarioId, string youtubeCanalId)
        {
            return await _ctx.Canais
                .Where(c => c.UsuarioId == usuarioId && c.YoutubeCanalId == youtubeCanalId)
                .FirstOrDefaultAsync();
        }

        public async Task Update(Canal canal)
        {
            _ctx.Canais.Update(canal);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateUltimaBusca(Canal canal, DateTime ultimaBusca, DateTime? ultimoVideo)
        {
            canal.UltimaBusca = ultimaBusca;
            canal.UltimoVideo = ultimoVideo;

            //_ctx.Canais.Attach(canal);
            _ctx.Entry(canal).Property(x => x.UltimaBusca).IsModified = true;
            _ctx.Entry(canal).Property(x => x.UltimoVideo).IsModified = true;
            await _ctx.SaveChangesAsync();
        }

        public async Task SetSituacao(Canal canal, EStatusCanal status, Exception e = null)
        {
            canal.Status = status;

            _ctx.Entry(canal).Property(x => x.Status).IsModified = true;
            await _ctx.SaveChangesAsync();
        }
    }
}
