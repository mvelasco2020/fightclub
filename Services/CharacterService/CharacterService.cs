using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
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
        private readonly IHttpContextAccessor _httpContext;
        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
            _context = context;
            _mapper = mapper;
        }


        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();

            Character character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context
                                    .Characters.Where(c => c.User.Id == GetUserId())
                                    .Select(c =>
                                    _mapper.Map<GetCharacterDTO>(c)
                                    ).ToListAsync();

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var characters = await _context.Characters.Where(c => c.User!.Id == GetUserId()).ToListAsync();
            serviceResponse.Data = characters.Select(c =>
            _mapper.Map<GetCharacterDTO>(c)
            ).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {

            var character = await _context.Characters
                            .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());

            var serviceResponse = new ServiceResponse<GetCharacterDTO>();
            if (character is not null)
            {
                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);
            }
            return serviceResponse;

        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(UpdateCharacterDTO updateCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();

            var character = await _context
                                    .Characters
                                    .FirstOrDefaultAsync(c => c.Id == updateCharacter.Id && c.User.Id == GetUserId());
            if (character is null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Character not found";
                return serviceResponse;
            }

            character.Name = updateCharacter.Name;
            character.HitPoints = updateCharacter.HitPoints;
            character.Strength = updateCharacter.Strength;
            character.Defense = updateCharacter.Defense;
            character.Intelligence = updateCharacter.Intelligence;
            character.Class = updateCharacter.Class;

            await _context.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);
            return serviceResponse;






        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            try
            {
                var character = await _context
                                        .Characters
                                        .FirstAsync(c => c.Id == id && c.User.Id == GetUserId());

                if (character is null)
                {
                    throw new Exception($"Character with id:{id} not found");
                }
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();

                var characters = await _context.Characters.Where(c => c.User.Id == GetUserId()).ToListAsync();
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

        private int GetUserId()
        {
            return int.Parse(_httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}