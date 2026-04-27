using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpPost("getList")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> GetAll()
    {
        var response = await _authorService.GetAuthorsAsync();
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpGet("getById/{id:int}")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> GetById(int id)
    {
        var response = await _authorService.GetAuthorByIdAsync(id);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse>> Create(
        [FromBody] CreateAuthorRequest request)
    {
        var response = await _authorService.CreateAuthorAsync(request);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("update/{id:int}")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> Update(
        int id,
        [FromBody] UpdateAuthorRequest request)
    {
        var response = await _authorService.UpdateAuthorAsync(id, request);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("delete/{id:int}")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> Delete(int id)
    {
        var response = await _authorService.DeleteAuthorAsync(id);
        return StatusCode((int)response.HttpStatus, response);
    }
}
