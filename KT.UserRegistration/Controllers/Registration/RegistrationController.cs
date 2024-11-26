using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KT.Exceptions;
using KT.Exceptions.API;
using KT.Exceptions.DB;
using KT.Models.Registration.Registration.Request;
using KT.Models.Registration.Registration.Response;
using KT.Registration.Services.Registration;
using KT.Validators.Registration;
using KT.Models.Common;
using PFM.Registration.Contracts;

namespace Registration.Controllers.Registration
{
    public class RegistrationController : Controller
    {
        private readonly PhoneNumberValidator _phoneNumberValidator;
        private readonly RegistrationService _registrationService;

        public RegistrationController(RegistrationService registrationService,
            PhoneNumberValidator phoneNumberValidator)
        {
            _registrationService = registrationService;
            _phoneNumberValidator = phoneNumberValidator;

        }

        public IActionResult Index()
        {
            return View();
        }
       

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Registration.PostRegistrationDetails)]
        [ProducesResponseType(typeof(PreliminaryRegistrationResponse), 200)]
        [ProducesResponseType(typeof(BadRequestResponse), 400)]
        public async Task<IActionResult> Registration([FromBody] RegistrationRequest registrationRequest)
        {

            try
            {
                var isValidPhoneNumber = _phoneNumberValidator.ValidatePhoneNumber(registrationRequest.PhoneNumber);
                if (!isValidPhoneNumber)
                {
                    return BadRequest(new BadRequestResponse
                    {
                        Code = "400",
                        Message = "First name, Last name, Mobile number or Email are invalid"
                    }
                  );
                }

                var premliminaryRegistrationResponse = await _registrationService.PostRegistrationDetails(registrationRequest);

                if (premliminaryRegistrationResponse == null)
                {
                    return StatusCode(403, new BadRequestResponse
                    {
                        Code = "403",
                        Message = "User is in Pending OTP Verification stage"
                    });
                }
                else
                {
                    return Ok(premliminaryRegistrationResponse);
                }
            }
            catch (DuplicateDatabaseKeyException e)
            {
                return StatusCode(409, new BadRequestResponse { Code = "409", Message = "This mobile number or email has already been registered" });
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


        [AllowAnonymous]
        [HttpPost(ApiRoutes.Registration.loginBase)]
        [ProducesResponseType(typeof(PreliminaryRegistrationResponse), 200)]
        [ProducesResponseType(typeof(BadRequestResponse), 400)]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {

            try
            {

                var login = await _registrationService.Login(loginRequest);

                if (login == null)
                {
                    return StatusCode(404, new BadRequestResponse
                    {
                        Code = "404",
                        Message = "User not found"
                    });
                }
                else
                {
                    return Ok(login);
                }
            }
            catch (DuplicateDatabaseKeyException e)
            {
                return StatusCode(409, new BadRequestResponse { Code = "409", Message = "This mobile number or email has already been registered" });
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


        [AllowAnonymous]
        [HttpPost(ApiRoutes.Registration.ResendOTP)]
        [ProducesResponseType(typeof(EmptyResult), 200)]
        [ProducesResponseType(typeof(BadRequestResponse), 400)]
        public async Task<IActionResult> ResendOTP_v1([FromBody] ResendOTPRequest resendOTPRequest)
        {


            if (resendOTPRequest.Email == null && resendOTPRequest.PhoneNumber == null && resendOTPRequest.UUID == null)
            {
                return BadRequest(new BadRequestResponse { Code = "400", Message = "Either UUID, Email or Phone Number is missing" });
            }
            if (resendOTPRequest.UDID == null)
            {
                return BadRequest(new BadRequestResponse { Code = "400", Message = "UDID is missing" });
            }
            try
            {
                var resendOTPResult = await _registrationService.ResendOTP(resendOTPRequest);
                return Ok();
            }
            catch (NonConnectivityException e)
            {
                BadRequestResponse errorDetails = new BadRequestResponse()
                {
                    Code = "412",
                    Message = "Device not found",
                };
                return StatusCode(412, errorDetails);
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

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Registration.VerifyOTP)]
        [ProducesResponseType(typeof(AccessTokenResponse), 200)]
        [ProducesResponseType(typeof(BadRequestResponse), 401)]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPRequest otp)
        {
            if (otp.Email == null && otp.PhoneNumber == null && otp.UUID == null)
            {
                return BadRequest(new BadRequestResponse { Code = "400", Message = "Either UUID or OTP " });
            }

            try
            {
                var accessTokenResponse = await _registrationService.VerifyOTP(otp);
                if (accessTokenResponse == null)
                {
                    BadRequestResponse errorDetails = new BadRequestResponse()
                    {
                        Code = "410",
                        Message = "OTP Expired",
                    };
                    return StatusCode(410, errorDetails);
                }
                return Ok(accessTokenResponse);
            }
            catch (NonConnectivityException e)
            {
                BadRequestResponse errorDetails = new BadRequestResponse()
                {
                    Code = "412",
                    Message = "Device not found",
                };
                return StatusCode(412, errorDetails);
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
            catch (ArgumentException e)
            {

                return StatusCode(403);
            }
            catch (Exception e)
            {
                return BadRequest(new BadRequestResponse { Code = "400", Message = e.Message });
            }
        }



    }
}