using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using fightclub.Data;
using fightclub.DTO.Fight;
using fightclub.Models;
using Microsoft.EntityFrameworkCore;

namespace fightclub.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;

        public FightService(DataContext context)
        {
            _context = context;

        }

        public async Task<ServiceResponse<AttackResultDTO>> WeaponAttack(WeaponAtkDTO request)
        {
            var response = new ServiceResponse<AttackResultDTO>();
            try
            {
                var attacker = await _context
                                .Characters
                                .Include(c => c.Weapon)
                                .FirstOrDefaultAsync(c =>
                                  c.Id == request.AttackerId);

                var Opponent = await _context
                                .Characters
                                .FirstOrDefaultAsync(c =>
                                  c.Id == request.OpponentId);

                if (Opponent is null || attacker is null || attacker.Weapon is null)
                {
                    throw new Exception();
                }

                int attackerDamage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
                attackerDamage -= new Random().Next(Opponent.Defense);


                if (attackerDamage > 0)
                {
                    Opponent.HitPoints -= attackerDamage;
                }
                await _context.SaveChangesAsync();

                response.Data = new AttackResultDTO
                {
                    Attacker = attacker.Name,
                    Opponent = Opponent.Name,
                    AttackerHp = attacker.HitPoints,
                    OpponentHp = Opponent.HitPoints,
                    Damage = attackerDamage
                };
                if (Opponent.HitPoints <= 0)
                {
                    response.Message = $"{Opponent.Name} has been defeated";
                }
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = "Unable to perform action";
            }
            return response;
        }


        public async Task<ServiceResponse<AttackResultDTO>> SkillAttack(SkillAtkDTO request)
        {
            var response = new ServiceResponse<AttackResultDTO>();
            try
            {
                var attacker = await _context
                                .Characters
                                .Include(c => c.Skills)
                                .FirstOrDefaultAsync(c =>
                                  c.Id == request.AttackerId);

                var Opponent = await _context
                                .Characters
                                .FirstOrDefaultAsync(c =>
                                  c.Id == request.OpponentId);

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);
                if (Opponent is null || attacker is null || attacker.Skills is null || skill is null)
                {
                    throw new Exception();
                }

                int attackerDamage = skill.Damage + new Random().Next(attacker.Intelligence);
                attackerDamage -= new Random().Next(Opponent.Defense);


                if (attackerDamage > 0)
                {
                    Opponent.HitPoints -= attackerDamage;
                }
                await _context.SaveChangesAsync();
                if (Opponent.HitPoints <= 0)
                {
                    response.Message = $"{Opponent.Name} has been defeated";
                }

                response.Data = new AttackResultDTO
                {
                    Attacker = attacker.Name,
                    Opponent = Opponent.Name,
                    AttackerHp = attacker.HitPoints,
                    OpponentHp = Opponent.HitPoints,
                    Damage = attackerDamage
                };
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = "Unable to perform action";
            }
            return response;
        }


    }
}