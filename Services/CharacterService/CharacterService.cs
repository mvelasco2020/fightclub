using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AutoMapper;
using fightclub.DTO.Character;
using fightclub.Models;

namespace fightclub.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        public CharacterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        private static List<Character> characters = new List<Character>(){
            new Character(),
            new Character(){
                Id = 99,
                Name = "player2"
            }
        };


        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
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
            serviceResponse.Data = characters.Select(c =>
            _mapper.Map<GetCharacterDTO>(c)
            ).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var character = characters.FirstOrDefault(c => c.Id == id);

            if (character is not null)
            {
                var serviceResponse = new ServiceResponse<GetCharacterDTO>();
                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);
                return serviceResponse;
            }
            throw new Exception($"Character id:{id} not found");
        }
    }
}