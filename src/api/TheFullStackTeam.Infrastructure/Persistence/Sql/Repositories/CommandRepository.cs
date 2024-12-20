﻿using TheFullStackTeam.Domain.Entities;
using TheFullStackTeam.Domain.Repositories.Command;
using Microsoft.EntityFrameworkCore;
using NUlid;

namespace TheFullStackTeam.Infrastructure.Persistence.Sql.Repositories;

public class CommandRepository<T> : ICommandRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public CommandRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Ulid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
}
