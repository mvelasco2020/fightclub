using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fightclub.DTO.Fight
{
    public class FightRequestDTO
    {
        public List<int> CharacterIds { get; set; } = new List<int>();
    }
}