using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpPost("getList")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> GetAll()
    {
        var response = await _articleService.GetArticlesAsync();
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpGet("getById/{id:int}")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> GetById(int id)
    {
        var response = await _articleService.GetArticleByIdAsync(id);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse>> Create(
        [FromBody] CreateArticleRequest request)
    {
        var response = await _articleService.CreateArticleAsync(request);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("update/{id:int}")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> Update(
        int id,
        [FromBody] UpdateArticleRequest request)
    {
        var response = await _articleService.UpdateArticleAsync(id, request);
        return StatusCode((int)response.HttpStatus, response);
    }

    [HttpPost("delete/{id:int}")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse>> Delete(int id)
    {
        var response = await _articleService.DeleteArticleAsync(id);
        return StatusCode((int)response.HttpStatus, response);
    }
}
