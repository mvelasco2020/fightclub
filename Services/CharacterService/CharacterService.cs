using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AutoMapper;
using fightclub.Data;
using fightclub.DTO.Character;
using fightclub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;

namespace fightclub.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public CharacterService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            var characters = await _context.Characters.ToListAsync();
            characters.Add(_mapper.Map<Character>(newCharacter));
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            serviceResponse.Data = characters.Select(c =>
            _mapper.Map<GetCharacterDTO>(c)
            ).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();

            var characters = await _context.Characters.ToListAsync();
            serviceResponse.Data = characters.Select(c =>
            _mapper.Map<GetCharacterDTO>(c)
            ).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var characters = await _context.Characters.ToListAsync();
            var character = characters.FirstOrDefault(c => c.Id == id);

            if (character is not null)
            {
                var serviceResponse = new ServiceResponse<GetCharacterDTO>();
                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);
                return serviceResponse;
            }
            throw new Exception($"Character id:{id} not found");
        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(UpdateCharacterDTO updateCharacter)
        {
            var characters = await _context.Characters.ToListAsync();
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var character = characters.FirstOrDefault(c => c.Id == updateCharacter.Id);
                if (character is null)
                {
                    throw new Exception($"Character do not exist id: ${updateCharacter.Id}");
                }

                character.Name = updateCharacter.Name;
                character.HitPoints = updateCharacter.HitPoints;
                character.Strength = updateCharacter.Strength;
                character.Defense = updateCharacter.Defense;
                character.Intelligence = updateCharacter.Intelligence;
                character.Class = updateCharacter.Class;
                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);
                return serviceResponse;

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return (serviceResponse);
            }


        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var characters = await _context.Characters.ToListAsync();
            try
            {
                var character = characters.First(c => c.Id == id);
                if (character is null)
                {
                    throw new Exception($"Character with id:{id} not found");
                }
                characters.Remove(character);
                serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();
                return serviceResponse;
            }
            catch (System.Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return serviceResponse;
            }

        }
    }
}