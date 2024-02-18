using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fightclub.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Player 1";
        public int HitPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Intelligence { get; set; } = 10;

        public User? User { get; set; }
        public FighterClass Class { get; set; } = FighterClass.Knight;

        public Weapon? Weapon { get; set; }
    }
}