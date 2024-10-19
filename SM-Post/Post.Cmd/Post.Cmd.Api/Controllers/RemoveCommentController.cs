using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers {
  [ApiController]
  [Route("api/v1/[controller]")]
  public class RemoveCommentController : ControllerBase {
    private readonly ILogger<RemoveCommentController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public RemoveCommentController(ILogger<RemoveCommentController> logger, ICommandDispatcher commandDispatcher) {
      _logger = logger;
      _commandDispatcher = commandDispatcher;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveCommentAsync(Guid id, RemoveCommentCommand command) {
      try {
        command.Id = id;
        await _commandDispatcher.SendAsync(command);
        
        return Ok(new BaseResponse {
          Message = "Remove comment request completed successfully!"
        });
      }

      catch (InvalidOperationException exception) {
        _logger.Log(LogLevel.Warning, exception, "Client made a bad request!");
        return BadRequest(new BaseResponse {
          Message = exception.Message
        });
      }

      catch (AggregateNotFoundException exception) {
        _logger.Log(LogLevel.Warning, exception, "Could not retrieve aggregate, client passed an incorrect post ID targetting the aggregate!");
        return BadRequest(new BaseResponse {
          Message = exception.Message
        });
      }
      
      catch(Exception exception) {
        const string SAFE_ERROR_MESSAGE = "Error while processing request to remove a comment from a post!";
        _logger.Log(LogLevel.Error, exception, SAFE_ERROR_MESSAGE);
        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse {
          Message = SAFE_ERROR_MESSAGE
        });
      }
    }
  }
}