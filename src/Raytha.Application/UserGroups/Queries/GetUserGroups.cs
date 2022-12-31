﻿using MediatR;
using Raytha.Application.Common.Interfaces;
using Raytha.Application.Common.Models;
using Raytha.Application.Common.Utils;
using Raytha.Domain.ValueObjects;

namespace Raytha.Application.UserGroups.Queries;

public class GetUserGroups
{
    public record Query : GetPagedEntitiesInputDto, IRequest<IQueryResponseDto<ListResultDto<UserGroupDto>>>
    {
        public override string OrderBy { get; init; } = $"Label {SortOrder.ASCENDING}";
    }

    public class Handler : RequestHandler<Query, IQueryResponseDto<ListResultDto<UserGroupDto>>>
    {
        private readonly IRaythaDbContext _db;
        public Handler(IRaythaDbContext db)
        {
            _db = db;
        }
        protected override IQueryResponseDto<ListResultDto<UserGroupDto>> Handle(Query request)
        {
            var query = _db.UserGroups.AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
            {
                var searchQuery = request.Search.ToLower();
                query = query.Where(d =>
                    d.Label.ToLower().Contains(searchQuery) ||
                    d.DeveloperName.ToLower().Contains(searchQuery));
            }

            var total = query.Count();
            var items = query.ApplyPaginationInput(request).Select(UserGroupDto.GetProjection()).ToArray();

            return new QueryResponseDto<ListResultDto<UserGroupDto>>(new ListResultDto<UserGroupDto>(items, total));
        }
    }
}
