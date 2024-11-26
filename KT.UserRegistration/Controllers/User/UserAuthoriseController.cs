using KT.Models.Authorisation.Request;
using KT.Models.Authorisation.Response;
using KT.Models.Common;
using KT.Registration.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFM.Registration.Contracts;
using System.Threading.Tasks;

namespace Registration.Controllers.User
{
    public class UserAuthoriseController : Controller
    {
        private readonly UserAuthoriseService _userAuthoriseService;

        public UserAuthoriseController(UserAuthoriseService userAuthoriseService)
        {
            _userAuthoriseService = userAuthoriseService;
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.User.Authorise.AuthoriseBase)]
        [ProducesResponseType(typeof(UserAuthorisationResponse), 200)]
        [ProducesResponseType(typeof(BadRequestResponse), 400)]
        public async Task<IActionResult> PostAuthorisation([FromBody] UserAuthorisationRequest userAuthorisationRequest)
        {
            var userAuthorisationResponse = await _userAuthoriseService.PostAuthorisation(userAuthorisationRequest);
            if (userAuthorisationResponse != null)
            {
                return Ok(userAuthorisationResponse);
            }
            return BadRequest();
        }
    }
}