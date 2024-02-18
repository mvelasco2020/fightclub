using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fightclub.DTO.Character;
using fightclub.DTO.Weapon;
using fightclub.Models;

namespace fightclub.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDTO>> AddWeapon(AddWeaponDTO newWeapon);
    }
}