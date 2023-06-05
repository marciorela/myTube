using Microsoft.EntityFrameworkCore;
using MR.PagedList;
using myTube.Data.Base;
using myTube.Domain.Entities;
using myTube.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace myTube.Data.Repositories;

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

    public async Task<PagedList<Filme>> GetListIndex(Guid usuarioId, string canalId, int pageNumber, string watch = "", string categoria = null)
    {
        watch ??= "";

        // SE ALGUM VÍDEO TIVER SCHEDULEDSTART, ELE SÓ DEVERÁ APARECER QUANDO FALTAR UMA HORA PARA O INÍCIO
        var x = GetQueryableByUsuario(usuarioId)
            .Where(v => DateTime.Now >= (v.ScheduledStartTime ?? DateTime.Now).AddHours(-1))
            .Where(v => v.DurationSecs > 0)
            .Where(v => canalId != null && v.CanalId == Guid.Parse(canalId) || canalId == null)
            .Where(v => string.IsNullOrWhiteSpace(categoria) || v.Canal.Categoria == categoria)
            .Where(v =>
                (watch == "" || watch.Length == 4) && v.Status == EStatusVideo.NaoAssistido ||
                watch.Contains('L') && v.Status == EStatusVideo.AssistirDepois ||
                watch.Contains('A') && v.Status == EStatusVideo.Assistido ||
                watch.Contains('I') && v.Status == EStatusVideo.Ignorado ||
                watch.Contains('F') && v.Status == EStatusVideo.Favorito
                );

        return await x
            .OrderBy(v => v.ScheduledStartTime ?? v.PublishedAt)
            .ToPagedListAsync(pageNumber);
    }

    public async Task<Filme> GetById(Guid id)
    {
        return await _ctx.Filmes
            .Include(c => c.Canal)
            .Include(c => c.Canal.Usuario)
            .Where(f => f.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task SetProgress(Guid id, double progress)
    {
        var video = new Filme()
        {
            Id = id,
            WatchedSecs = progress
        };

        _ctx.Filmes.Attach(video);
        _ctx.Entry(video).Property(p => p.WatchedSecs).IsModified = true;
        await _ctx.SaveChangesAsync();
    }

    public IQueryable<Filme> GetQueryableByUsuario(Guid usuarioId)
    {
        return _ctx.Filmes
            .Include(i => i.Canal)
            .Where(v => v.Canal.UsuarioId == usuarioId);
    }

    public async Task<IEnumerable<Filme>> GetByUsuario(Guid usuarioId)
    {
        return await GetQueryableByUsuario(usuarioId)
            .OrderBy(v => v.ScheduledStartTime ?? v.PublishedAt)
            .ToListAsync();
    }

    public async Task ChangeStatus(Guid id, EStatusVideo status)
    {
        var old = await GetById(id);

        old.DataStatus = DateTime.Now;
        if (old.Status == status)
        {
            old.Status = EStatusVideo.NaoAssistido;
        }
        else
        {
            old.Status = status;
        }

        //var video = new Filme()
        //{
        //    Id = id,
        //    Status = (old.Status == status ? EStatusVideo.NaoAssistido : status),
        //    DataStatus = DateTime.Now
        //};

        //_ctx.Filmes.Attach(video);
        //_ctx.Entry(video).Property(p => p.Status).IsModified = true;
        //_ctx.Entry(video).Property(p => p.DataStatus).IsModified = true;

        _ctx.Filmes.Update(old);
        await _ctx.SaveChangesAsync();
    }

    public async Task<IEnumerable<Filme>> GetZeroSecondsVideos(Canal canal)
    {
        return await _ctx.Filmes
            .Where(f => f.CanalId == canal.Id && f.DurationSecs == 0 && DateTime.Now >= (f.ScheduledStartTime ?? DateTime.MinValue))
            .Where(f => f.Status != EStatusVideo.Cancelado)
            .ToListAsync();
    }

    public async Task Update(Filme filme)
    {
        _ctx.Filmes.Update(filme);
        await _ctx.SaveChangesAsync();
    }
}
