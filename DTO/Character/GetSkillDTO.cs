using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fightclub.DTO.Character
{
    public class GetSkillDTO
    {
        public string Name { get; set; } = string.Empty;
        public int Damage { get; set; }
    }
}