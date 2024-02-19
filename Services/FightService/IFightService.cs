using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fightclub.DTO.Fight;
using fightclub.Models;

namespace fightclub.Services.FightService
{
    public interface IFightService
    {
        Task<ServiceResponse<AttackResultDTO>> WeaponAttack(WeaponAtkDTO request);
        Task<ServiceResponse<AttackResultDTO>> SkillAttack(SkillAtkDTO request);
    }
}