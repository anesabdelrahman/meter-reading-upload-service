namespace En.Metering.Validation.Interfaces
{
    public interface ISpecification<T>
    {
        Task<(bool, string)> IsSatisfiedByAsync(T entity, CancellationToken token);
    }
}
