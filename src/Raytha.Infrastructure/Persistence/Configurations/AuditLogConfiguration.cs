﻿using Raytha.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Raytha.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder
            .HasIndex(b => b.Category);

        builder
            .HasIndex(b => b.CreationTime);

        builder
            .HasIndex(b => b.EntityId);
    }
}
