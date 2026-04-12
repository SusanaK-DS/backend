using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("getList")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _userService.GetUsersAsync(cancellationToken);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpGet("getById/{id:int}")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await _userService.GetUserByIdAsync(id, cancellationToken);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _userService.CreateUserAsync(request, cancellationToken);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("update/{id:int}")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> Update(
        int id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _userService.UpdateUserAsync(id, request, cancellationToken);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("delete/{id:int}")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await _userService.DeleteUserAsync(id, cancellationToken);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> Login(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _userService.LoginAsync(request, cancellationToken);
        return StatusCode((int)response.HttpStatus, response);
    }
}
