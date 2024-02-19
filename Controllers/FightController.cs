using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using fightclub.DTO.Fight;
using fightclub.Models;
using fightclub.Services.FightService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace fightclub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;

        public FightController(IFightService fightService)
        {
            _fightService = fightService;
        }


        [HttpPost("Weapon")]
        public async Task<ActionResult<ServiceResponse<AttackResultDTO>>> WeaponAttack(WeaponAtkDTO request)
        {

            return Ok(await _fightService.WeaponAttack(request));
        }

        [HttpPost("Skill")]
        public async Task<ActionResult<ServiceResponse<AttackResultDTO>>> SkillAttack(SkillAtkDTO request)
        {

            return Ok(await _fightService.SkillAttack(request));
        }
    }
}