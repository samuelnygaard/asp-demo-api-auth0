using AutoMapper;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class BaseRepository<TContext, TModel, TEntity, T> : IRepository<TModel, TEntity, T>
            where TContext : DbContext
            where TModel : class, IModel<T>
            where TEntity : class, IEntity<T>
    {
        protected readonly TContext db;
        protected readonly IMapper mapper;

        public BaseRepository(TContext db)
        {
            this.db = db;
            mapper = MapperService.ConfigureAutoMapper();
        }

        public async Task<TModel> Add(TModel data)
        {
            var entity = await db.Set<TEntity>().AddAsync(Map(data));

            return Map(entity.Entity);
        }

        public async Task Delete(T id)
        {
            var entity = await db.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return;
            }

            db.Set<TEntity>().Remove(entity);
            await db.SaveChangesAsync();
        }

        public async Task Delete(TModel data)
        {
            await Delete(data.Id);
        }

        public async Task<IEnumerable<TModel>> Find(Expression<Func<TModel, bool>> expression)
        {
            var entityExpression = mapper.Map<Expression<Func<TEntity, bool>>>(expression);
            var data = db.Set<TEntity>().Where(entityExpression);
            return Map(await data.ToListAsync());
        }

        public async Task<TModel?> Get(T id)
        {
            var data = await db.Set<TEntity>().FindAsync(id);
            if (data == null)
            {
                return null;
            }

            return Map(data);
        }

        public async Task<IEnumerable<TModel>> GetAll()
        {
            return Map(await db.Set<TEntity>().ToListAsync());
        }

        public async Task<TModel> Update(TModel data)
        {
            var entity = Map(data);
            db.Entry(entity).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Map(entity);
        }

        internal IEnumerable<TEntity> Map(IEnumerable<TModel> models)
        {
            return mapper.Map<IEnumerable<TEntity>>(models);
        }

        internal IEnumerable<TModel> Map(IEnumerable<TEntity> entities)
        {
            return mapper.Map<IEnumerable<TModel>>(entities);
        }

        internal TModel Map(TEntity entity)
        {
            return mapper.Map<TModel>(entity);
        }

        internal TEntity Map(TModel model)
        {
            return mapper.Map<TEntity>(model);
        }
    }
}