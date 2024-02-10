using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using fightclub.DTO.Character;
using fightclub.Models;

namespace fightclub.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character>(){
            new Character(),
            new Character(){
                Id = 99,
                Name = "player2"
            }
        };
        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            characters.Add(newCharacter);
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            serviceResponse.Data = characters;
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            serviceResponse.Data = characters;
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var character = characters.FirstOrDefault(c => c.Id == id);

            if (character is not null)
            {
                var serviceResponse = new ServiceResponse<GetCharacterDTO>();
                serviceResponse.Data = character;
                return serviceResponse;
            }
            throw new Exception($"Character id:{id} not found");
        }
    }
}