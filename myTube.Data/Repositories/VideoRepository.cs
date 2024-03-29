﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Filme>> GetListIndex(Guid usuarioId)
        {
            // SE ALGUM VÍDEO TIVER SCHEDULEDSTART, ELE SÓ DEVERÁ APARECER QUANDO FALTAR UMA HORA PARA O INÍCIO
            return (await GetByStatus(usuarioId, EStatusVideo.NaoAssistido))
                .Where(v => DateTime.Now >= (v.ScheduledStartTime ?? DateTime.Now).AddHours(-1))
                .Where(v => v.DurationSecs > 0)
                .ToList();
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

        public async Task<IEnumerable<Filme>> GetByStatus(Guid usuarioId, EStatusVideo status)
        {
            return await _ctx.Filmes
                .Include(i => i.Canal)
                .Where(v => v.Canal.UsuarioId == usuarioId && v.Status == status)
                .OrderBy(v => v.ScheduledStartTime ?? v.PublishedAt)
                .ToListAsync();
        }

        public async Task ChangeStatus(Guid id, EStatusVideo status)
        {
            var video = new Filme()
            {
                Id = id,
                Status = status,
                DataStatus = DateTime.Now
            };

            _ctx.Filmes.Attach(video);
            _ctx.Entry(video).Property(p => p.Status).IsModified = true;
            _ctx.Entry(video).Property(p => p.DataStatus).IsModified = true;

            await _ctx.SaveChangesAsync();
        }

        public async Task Update(Filme filme)
        {
            _ctx.Filmes.Update(filme);
            await _ctx.SaveChangesAsync();
        }
    }
}
