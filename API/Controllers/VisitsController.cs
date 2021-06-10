using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    public class VisitsController : BaseApiController {
        private readonly IUnitOfWork _unitOfWork;
        public VisitsController (IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;

        }

        //[Authorize (Policy = "RequireVIPRole")]
        [HttpPost ("{username}")]
        public async Task<ActionResult> AddVisit (string username) {
            
            var sourceUserId = User.GetUserId();
            var VisitedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync (username);
            var sourceUser = await _unitOfWork.VisitsRepository.GetUserWithVisits(sourceUserId);

            if (VisitedUser == null) return NotFound ();

            if (sourceUser.UserName == username) return BadRequest ("You can not visit yourself");

            var userVisit = await _unitOfWork.VisitsRepository.GetUserVisit (sourceUserId, VisitedUser.Id);

            if (userVisit != null)
            { 
                userVisit.Created = DateTime.UtcNow;
                return Ok();
            }

            userVisit = new Entities.UserVisits {
                SourceUserId = sourceUserId,
                VisitedUserId = VisitedUser.Id,
                Created = DateTime.UtcNow           
            };

            sourceUser.VisitedUsers.Add(userVisit);

            if (await _unitOfWork.Complete ()) return Ok ();

            return BadRequest ("Visit Failed");
        }

        [Authorize (Policy = "RequireVIPRole")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VisitsDto>>> GetUserVisits ([FromQuery] VisitsParams visitsParams) {
            visitsParams.UserId = User.GetUserId ();
            var users = await _unitOfWork.VisitsRepository.GetUserVisits (visitsParams);

            Response.AddPaginationHeader (users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok (users);
        }

    }
}