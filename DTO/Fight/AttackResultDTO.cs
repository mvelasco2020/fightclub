using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fightclub.DTO.Fight
{
    public class AttackResultDTO
    {

        public string Attacker { get; set; } = string.Empty;
        public string Opponent { get; set; } = string.Empty;

        public int AttackerHp { get; set; }
        public int OpponentHp { get; set; }
        public int Damage { get; set; }
    }
}