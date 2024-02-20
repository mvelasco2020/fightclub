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

                int attackerDamage = DoWeaponAttack(attacker, Opponent);
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

                int attackerDamage = DoSkillAttack(attacker, Opponent, skill);
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


        public async Task<ServiceResponse<FightResultDTO>> Fight(FightRequestDTO request)
        {
            var response = new ServiceResponse<FightResultDTO>
            {
                Data = new FightResultDTO()
            };

            try
            {
                // get characters from request
                var characters = await _context.Characters
                                        .Include(c => c.Weapon)
                                        .Include(c => c.Skills)
                                        .Where(c => request.CharacterIds.Contains(c.Id))
                                        .ToListAsync();

                //Roll dice on who goes first by reversing the list
                if (new Random().Next(2) == 0)
                {
                    characters.Reverse();
                }
                response.Data.Log.Add($"{characters[0].Name} takes initiative!");

                bool defeated = false;
                while (!defeated)
                {
                    foreach (var attacker in characters)
                    {
                        var defenders = characters
                                        .Where(c => c.Id != attacker.Id)
                                        .ToList();
                        var defender = defenders[new Random().Next(defenders.Count)];
                        int damage = 0;
                        string attackUsed = string.Empty;

                        // roll dice if weapon will be used 1/2
                        bool attackerUseWeapon = new Random().Next(2) == 0;
                        if (attackerUseWeapon && attacker.Weapon is not null)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, defender);
                        }
                        else if (!attackerUseWeapon && attacker.Skills is not null)
                        {
                            var skillUsed = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skillUsed.Name;
                            damage = DoSkillAttack(attacker, defender, skillUsed);
                        }
                        else
                        {
                            response.Data.Log.Add($"{attacker.Name} attacked but misses...");
                            continue;
                        }
                        response.Data.Log.Add($"{attacker.Name} hits {defender.Name} with {attackUsed} for {damage} damage!");

                        if (defender.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Wins++;
                            defender.Losses++;
                            response.Data.Log.Add($"{attacker.Name} has won this match!");
                            response.Data.Log.Add($"{defender.Name} lost this match.");
                            break;
                        }
                    }
                }

                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoints = 100;
                });
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = "Unable to perform action";
            }
            return response;
        }

        private static int DoWeaponAttack(Character attacker, Character Opponent)
        {
            int attackerDamage = 0;
            if (attacker.Weapon is null || new Random().Next(3) == 0)
            {
                return attackerDamage;
            }
            attackerDamage = attacker.Weapon.Damage + new Random().Next(attacker.Strength);
            attackerDamage -= new Random().Next(Opponent.Defense);

            if (attackerDamage > 0)
            {
                Opponent.HitPoints -= attackerDamage;
            }

            return attackerDamage;
        }

        private static int DoSkillAttack(Character attacker, Character Opponent, Skill skill)
        {
            int attackerDamage = 0;
            if (attacker.Skills.Count == 0)
            {
                return attackerDamage;
            }
            attackerDamage = skill.Damage + new Random().Next(attacker.Intelligence);
            attackerDamage -= new Random().Next(Opponent.Defense);

            if (attackerDamage > 0)
            {
                Opponent.HitPoints -= attackerDamage;
            }

            return attackerDamage;
        }

    }
}