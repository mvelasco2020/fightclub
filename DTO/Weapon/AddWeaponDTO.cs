using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fightclub.Models;

namespace fightclub.DTO.Weapon
{
    public class AddWeaponDTO
    {

        public string Name { get; set; } = string.Empty;
        public int Damage { get; set; }
        public int CharacterId { get; set; }


    }
}