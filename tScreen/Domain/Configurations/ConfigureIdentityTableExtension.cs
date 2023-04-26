using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Domain.Configurations;

public static class ConfigureIdentityTablesExtension
{
    private const string SchemaName = "Identity";

    public static void ConfigureIdentityTables(this ModelBuilder builder)
    {
        builder.Entity<Role>(entity =>
        {
            entity.ToTable(nameof(Role), SchemaName);
            ConfigureTable.StandardEntityFields(entity);
        });

        builder.Entity<User>(entity =>
        {
            entity.ToTable(nameof(User), SchemaName);
            ConfigureTable.StandardEntityFields(entity);
        });

        builder.Entity<UserClaim>(entity =>
        {
            entity.ToTable(nameof(UserClaim), SchemaName);
            ConfigureTable.StandardDataFields(entity);
        });

        builder.Entity<UserLogin>(entity =>
        {
            entity.ToTable(nameof(UserLogin), SchemaName);
            ConfigureTable.StandardDataFields(entity);
        });

        builder.Entity<UserRole>(entity =>
        {
            entity.ToTable(nameof(UserRole), SchemaName);
            ConfigureTable.StandardDataFields(entity);
        });

        builder.Entity<RoleClaim>(entity =>
        {
            entity.ToTable(nameof(RoleClaim), SchemaName);
            ConfigureTable.StandardDataFields(entity);
        });

        builder.Entity<UserToken>(entity =>
        {
            entity.ToTable(nameof(UserToken), SchemaName);
            ConfigureTable.StandardDataFields(entity);
        });
    }
}