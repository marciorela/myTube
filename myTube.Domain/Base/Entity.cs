using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Domain.Base
{
    public class Entity
    {
        public Entity()
        {
            Id = Guid.NewGuid();
            Data =  DateTime.UtcNow;
        }

        public Guid Id { get; set; }

        public DateTime Data { get; set; }


    }
}
