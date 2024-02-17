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

            _context.Characters.Add(_mapper.Map<Character>(newCharacter));
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
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var character = await _context
                                        .Characters
                                        .FirstOrDefaultAsync(c => c.Id == updateCharacter.Id && c.User.Id == GetUserId());
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

                await _context.SaveChangesAsync();
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
                serviceResponse.Data = await _context.Characters.Select(c =>
                                                                    _mapper.Map<GetCharacterDTO>(c))
                                                                    .ToListAsync();

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