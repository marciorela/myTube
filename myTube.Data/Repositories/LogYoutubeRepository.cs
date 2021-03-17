using myTube.Data.Base;
using myTube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Data.Repositories
{
    public class LogYoutubeRepository : BaseRepository
    {
        public LogYoutubeRepository(AppDbContext ctx) : base(ctx)
        {
        }

        public async Task Add(string service, int cost, Exception e = null)
        {
            await _ctx.LogYoutube.AddAsync(new LogYoutube()
            {
                Service = service,
                Cost = cost,
                Error = String.Join("\n", new List<string>() { 
                    e?.Message, 
                    e?.StackTrace 
                })
            });

            await _ctx.SaveChangesAsync();
        }
    }
}
