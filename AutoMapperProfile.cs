using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using fightclub.DTO.Character;
using fightclub.DTO.Fight;
using fightclub.DTO.Weapon;
using fightclub.Models;

namespace fightclub
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDTO>();
            CreateMap<AddCharacterDTO, Character>();
            CreateMap<AddWeaponDTO, Weapon>();
            CreateMap<Weapon, GetWeaponDTO>();
            CreateMap<Skill, GetSkillDTO>();
            CreateMap<Character, HighScoreDTO>();
        }
    }
}