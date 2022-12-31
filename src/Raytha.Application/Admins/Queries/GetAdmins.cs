using System.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Raytha.Application.Common.Interfaces;
using Raytha.Application.Common.Models;
using Raytha.Application.Common.Utils;
using Raytha.Domain.ValueObjects;

namespace Raytha.Application.Admins.Queries;

public class GetAdmins
{
    public record Query : GetPagedEntitiesInputDto, IRequest<IQueryResponseDto<ListResultDto<AdminDto>>> 
    { 
        public override string OrderBy { get; init; } = $"LastLoggedInTime {SortOrder.DESCENDING}";
    }

    public class Handler : RequestHandler<Query, IQueryResponseDto<ListResultDto<AdminDto>>>
    {
        private readonly IRaythaDbContext _db;
        public Handler(IRaythaDbContext db)
        {
            _db = db;
        }
        protected override IQueryResponseDto<ListResultDto<AdminDto>> Handle(Query request)
        {                   
            var query = _db.Users.AsQueryable()
                .Include(p => p.Roles)
                .Where(p => p.IsAdmin);
               
            if (!string.IsNullOrEmpty(request.Search))
            {
                var searchQuery = request.Search.ToLower();
                query = query
                    .Include(p => p.Roles)
                    .Where(d =>
                        d.FirstName.ToLower().Contains(searchQuery) ||
                        d.LastName.ToLower().Contains(searchQuery) ||
                        d.EmailAddress.ToLower().Contains(searchQuery) ||
                        d.Roles.Any(p => p.Label.Contains(searchQuery)));              
            }

            var total = query.Count();
            var items = query.ApplyPaginationInput(request).Select(AdminDto.GetProjection()).ToArray();

            return new QueryResponseDto<ListResultDto<AdminDto>>(new ListResultDto<AdminDto>(items, total));
        }
    }
}
