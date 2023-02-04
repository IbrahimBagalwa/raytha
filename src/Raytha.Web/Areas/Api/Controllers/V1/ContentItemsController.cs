using CSharpVitamins;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raytha.Application.Common.Models;
using Raytha.Application.Common.Security;
using Raytha.Application.ContentItems;
using Raytha.Application.ContentItems.Commands;
using Raytha.Application.ContentItems.Queries;
using Raytha.Application.ContentTypes.Queries;
using Raytha.Application.Routes;
using Raytha.Application.Routes.Queries;
using Raytha.Domain.Entities;
using Raytha.Web.Authentication;
using System.Threading.Tasks;

namespace Raytha.Web.Areas.Api.Controllers.V1;


public class ContentItemsController : BaseController
{
    [HttpGet("{contentTypeDeveloperName}", Name = "GetContentItems")]
    [Authorize(Policy = RaythaApiAuthorizationHandler.POLICY_PREFIX + BuiltInContentTypePermission.CONTENT_TYPE_READ_PERMISSION)]
    public async Task<ActionResult<IQueryResponseDto<ListResultDto<ContentItemDto>>>> GetContentItems(
                                           string contentTypeDeveloperName,
                                           string viewId = "",
                                           string search = "",
                                           string filter = "",
                                           string orderBy = "",
                                           int pageNumber = 1,
                                           int pageSize = 50)
    {
        var input = new GetContentItems.Query 
        { 
            ContentType = contentTypeDeveloperName,
            ViewId = viewId,
            Search = search,
            Filter = filter,
            OrderBy = orderBy,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var response = await Mediator.Send(input) as QueryResponseDto<ListResultDto<ContentItemDto>>;
        return response;
    }

    [HttpGet("{contentTypeDeveloperName}/trash", Name = "GetDeletedContentItems")]
    [Authorize(Policy = RaythaApiAuthorizationHandler.POLICY_PREFIX + BuiltInContentTypePermission.CONTENT_TYPE_CONFIG_PERMISSION)]
    public async Task<ActionResult<IQueryResponseDto<ListResultDto<ContentItemDto>>>> GetDeletedContentItems(
                                           string contentTypeDeveloperName)
    {
        var input = new GetDeletedContentItems.Query { DeveloperName = contentTypeDeveloperName };
        var response = await Mediator.Send(input) as QueryResponseDto<ListResultDto<ContentItemDto>>;
        return response;
    }

    [HttpGet("{contentTypeDeveloperName}/{contentItemId}", Name = "GetContentItemById")]
    [Authorize(Policy = RaythaApiAuthorizationHandler.POLICY_PREFIX + BuiltInContentTypePermission.CONTENT_TYPE_READ_PERMISSION)]
    public async Task<ActionResult<IQueryResponseDto<ContentItemDto>>> GetContentItemById(
                                       string contentTypeDeveloperName,
                                       string contentItemId)
    {
        var input = new GetContentItemById.Query { Id = contentItemId };
        var response = await Mediator.Send(input) as QueryResponseDto<ContentItemDto>;
        return response;
    }

    [HttpPost("{contentTypeDeveloperName}", Name = "CreateContentItem")]
    [Authorize(Policy = RaythaApiAuthorizationHandler.POLICY_PREFIX + BuiltInContentTypePermission.CONTENT_TYPE_EDIT_PERMISSION)]
    public async Task<ActionResult<ICommandResponseDto<ShortGuid>>> CreateContentItem(
                                        string contentTypeDeveloperName, 
                                        [FromBody] CreateContentItem.Command request)
    {
        var input = request with { ContentTypeDeveloperName = contentTypeDeveloperName };
        var response = await Mediator.Send(input);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return CreatedAtAction(nameof(GetContentItemById), new { contentTypeDeveloperName, contentItemId = response.Result }, response);
    }

    [HttpPut("{contentTypeDeveloperName}/{contentItemId}", Name = "EditContentItem")]
    [Authorize(Policy = RaythaApiAuthorizationHandler.POLICY_PREFIX + BuiltInContentTypePermission.CONTENT_TYPE_EDIT_PERMISSION)]
    public async Task<ActionResult<ICommandResponseDto<ShortGuid>>> EditContentItem(
                            string contentType, 
                            string contentItemId,
                            [FromBody] EditContentItem.Command request)
    {
        var input = request with { Id = contentItemId };
        var response = await Mediator.Send(input);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return response;
    }

    [HttpPut("{contentTypeDeveloperName}/{contentItemId}/settings", Name = "EditContentItemSettings")]
    [Authorize(Policy = RaythaApiAuthorizationHandler.POLICY_PREFIX + BuiltInContentTypePermission.CONTENT_TYPE_EDIT_PERMISSION)]
    public async Task<ActionResult<ICommandResponseDto<ShortGuid>>> EditContentItemSettings(
                            string contentTypeDeveloperName,
                            string contentItemId,
                            [FromBody] EditContentItemSettings.Command request)
    {
        var input = request with { Id = contentItemId };
        var response = await Mediator.Send(input);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return response;
    }

    [HttpPut("{contentTypeDeveloperName}/{contentItemId}/unpublish", Name = "UnpublishContentItem")]
    [Authorize(Policy = RaythaApiAuthorizationHandler.POLICY_PREFIX + BuiltInContentTypePermission.CONTENT_TYPE_EDIT_PERMISSION)]
    public async Task<ActionResult<ICommandResponseDto<ShortGuid>>> UnpublishContentItem(
                            string contentTypeDeveloperName,
                            string contentItemId)
    {
        var input = new UnpublishContentItem.Command { Id = contentItemId };
        var response = await Mediator.Send(input);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return response;
    }

    [HttpDelete("{contentTypeDeveloperName}/{contentItemId}", Name = "DeleteContentItem")]
    [Authorize(Policy = RaythaApiAuthorizationHandler.POLICY_PREFIX + BuiltInContentTypePermission.CONTENT_TYPE_EDIT_PERMISSION)]
    public async Task<ActionResult<ICommandResponseDto<ShortGuid>>> DeleteContentItem(
                            string contentTypeDeveloperName,
                            string contentItemId)
    {
        var input = new DeleteContentItem.Command { Id = contentItemId };
        var response = await Mediator.Send(input);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return response;
    }

    [HttpGet("{contentTypeDeveloperName}/route/{routePath}", Name = "GetRouteByPath")]
    [Authorize(Policy = RaythaApiAuthorizationHandler.POLICY_PREFIX + BuiltInContentTypePermission.CONTENT_TYPE_READ_PERMISSION)]
    public async Task<ActionResult<IQueryResponseDto<RouteDto>>> GetRouteByPath(
                                       string routePath)
    {
        var input = new GetRouteByPath.Query
        {
            Path = routePath
        };
        var response = await Mediator.Send(input) as QueryResponseDto<RouteDto>;
        return response;
    }
}
