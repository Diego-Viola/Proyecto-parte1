namespace Products.Api.Persistence.Interfaces;
public interface IAdapter<TEntity, TModel>
{
    TModel ToDomainModel(TEntity entity);
}
