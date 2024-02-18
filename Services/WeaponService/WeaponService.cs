using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using fightclub.Data;
using fightclub.DTO.Character;
using fightclub.DTO.Weapon;
using fightclub.Models;
using Microsoft.EntityFrameworkCore;

namespace fightclub.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;

        public WeaponService(DataContext context, IHttpContextAccessor httpContext, IMapper mapper)
        {
            _httpContext = httpContext;
            _mapper = mapper;
            _context = context;
        }
        public async Task<ServiceResponse<GetCharacterDTO>> AddWeapon(AddWeaponDTO newWeapon)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();
            Character character = await _context.Characters
                                    .Include(c => c.User)
                                    .Where(c => c.User.Id == GetUserId())
                                    .FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId);


            try
            {
                if (character is null)
                {
                    throw new Exception();
                }
                await _context.Weapons.AddAsync(_mapper.Map<Weapon>(newWeapon));
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);
                return serviceResponse;

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Something went wrong while trying to create a weapon.";
                return serviceResponse;
            }
        }


        private int GetUserId()
        {
            return int.Parse(_httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}