using Microsoft.AspNetCore.Mvc;
using PFM.Registration.Contracts;

using KT.Models.Registration.Device.Request;
using KT.Models.Common;
using KT.Registration.Services.User;
using KT.Validators.Registration;
using KT.Interfaces.Repositories;
using KT.Exceptions.API;
using KT.Exceptions;
using KT.Models.Registration.Device.Response;
using Microsoft.AspNetCore.Authorization;

namespace Registration.Controllers.User
{
    public class UserDeviceController : Controller
    {
        private readonly UserDeviceService _userDeviceService;
        private readonly PhoneNumberValidator _phoneNumberValidator;
        private readonly IUnitOfWork _unitOfWork;

        public UserDeviceController(UserDeviceService userDeviceService,
            PhoneNumberValidator phoneNumberValidator, IUnitOfWork unitOfWork)
        {
            _userDeviceService = userDeviceService;
            _phoneNumberValidator = phoneNumberValidator;
            _unitOfWork = unitOfWork;
        }

        [HttpPut(ApiRoutes.User.Device.PinReset)]
        [ProducesResponseType(typeof(EmptyResult), 200)]
        [ProducesResponseType(typeof(BadRequestResponse), 400)]
        public async Task<IActionResult> InitiatePinReset([FromBody] PinCodeRequest pinCodeResetRequest)
        {

            try
            {
                await _userDeviceService.PostPin(pinCodeResetRequest);
                return Ok();
            }

            catch (OperationCanceledException e)
            {
                BadRequestResponse errorDetails = new BadRequestResponse()
                {
                    Code = "423",
                    Message = "User locked",
                };
                return StatusCode(423, errorDetails);
            }
            catch (ForbiddenException e)
            {
                BadRequestResponse errorDetails = new BadRequestResponse()
                {
                    Code = "404",
                    Message = "User does not exist",
                };
                return StatusCode(404, errorDetails);
            }
            catch (Exception e)
            {
                if (e is OTPException)
                {
                    return StatusCode(428, new BadRequestResponse
                    {
                        Code = "428",
                        Message = e.Message
                    });
                }
                // this means e is DuplicateKeyException
                else
                {
                    return BadRequest(new BadRequestResponse
                    {
                        Code = "400",
                        Message = e.Message
                    });
                }
            }
        }

        [HttpPut(ApiRoutes.User.Device.Pin)]
        [ProducesResponseType(typeof(EmptyResult), 200)]
        [ProducesResponseType(typeof(BadRequestResponse), 400)]
        public async Task<IActionResult> InitiatePinSet([FromBody] PinCodeRequest pinCodeResetRequest)
        {        
  
            try
            {
                await _userDeviceService.PostPin(pinCodeResetRequest);
                return Ok();
            }
         
            catch (OperationCanceledException e)
            {
                BadRequestResponse errorDetails = new BadRequestResponse()
                {
                    Code = "423",
                    Message = "User locked",
                };
                return StatusCode(423, errorDetails);
            }
            catch (ForbiddenException e)
            {
                BadRequestResponse errorDetails = new BadRequestResponse()
                {
                    Code = "404",
                    Message = "User does not exist",
                };
                return StatusCode(404, errorDetails);
            }
            catch (Exception e)
            {
                if (e is OTPException)
                {
                    return StatusCode(428, new BadRequestResponse
                    {
                        Code = "428",
                        Message = e.Message
                    });
                }
                // this means e is DuplicateKeyException
                else
                {
                    return BadRequest(new BadRequestResponse
                    {
                        Code = "400",
                        Message = e.Message
                    });
                }
            }
        }
        [HttpPost(ApiRoutes.User.Device.Biometric)]
        [ProducesResponseType(typeof(BiometricResponse), 200)]
        [ProducesResponseType(typeof(BadRequestResponse), 400)]
        public async Task<IActionResult> PostBiometric(string uuid)
        {
            try
            {
                var biometricResponse = await _userDeviceService.PostBiometric(uuid);
                return Ok(biometricResponse);
            }
            catch (ForbiddenException e)
            {
                return Forbid();
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
        }

    }
}