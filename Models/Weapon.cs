using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fightclub.Models
{
    public class Weapon
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int Damage { get; set; }

        public Character? Character { get; set; }

        public int CharacterId { get; set; }

    }
}