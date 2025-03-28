﻿using System.Linq.Expressions;
using EC.SharedLibrary.Logs;
using EC.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Models;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Infrastructure.Repositories;

public class ProductRepository(ProductDbContext context) : IProduct
{
    public async Task<Response> CreateAsync(Product entity)
    {
        try
        {
            // check if the product already exist
            var getProduct = await GetByAsync(x => x.Name!.Equals(entity.Name));
            if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                return new Response(false, $"{entity.Name} already added");

            var currentEntity = context.Products.Add(entity).Entity;
            await context.SaveChangesAsync();

            if (currentEntity is not null && currentEntity.Id > 0)
                return new Response(true, $"{entity.Name} added");
            else
                return new Response(false, $"Error occurred while adding {entity.Name}");
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);
            
            // Display scary-free message to the client
            return new Response(false , "Error occurred adding new product");
        }
    }
    public async Task<Response> UpdateAsync(Product entity)
    {
        try
        {
            var product = await FindByIdAsync(entity.Id);
            
            if(product is null)
                return new Response(false, "Product not found");

            context.Entry(product).State = EntityState.Detached;
            context.Products.Update(entity);
            await context.SaveChangesAsync();
            
            return new Response(true, $"{entity.Name} updated");
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);
            
            // Display scary-free message to the client
            return new Response(false , "Error occurred del new product");
        }
    }
    public async Task<Response> DeleteAsync(Product entity)
    {
        try
        {
            var product = await FindByIdAsync(entity.Id);
            if(product is null)
                return new Response(false, $"{entity.Name} - not found");
            
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return new Response(true, $"{entity.Name} deleted");
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);
            
            // Display scary-free message to the client
            return new Response(false , "Error occurred deleting product");
        }
    }
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            var products = await context.Products.AsNoTracking().ToListAsync();
            return products is not null ? products : null!;
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);
            
            // Display scary-free message to the client
           throw new InvalidOperationException("Error occurred retrieving products");
        }
    }
    public async Task<Product> FindByIdAsync(int id)
    {
        try
        {
            var product = await context.Products.FindAsync(id);
            return product is not null ? product : null!;
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);
            
            // Display scary-free message to the client
            throw new Exception($"Error occurred getting product");
        }
    }
    public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
    {
        try
        {
            var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
        
            return product is not null ? product : null!;
        }
        catch (Exception ex)
        {
            // Log Original Exception
            LogException.LogExceptions(ex);
            
            // Display scary-free message to the client
            throw new InvalidOperationException("Error occurred retrieving product");
        }
    }
}
