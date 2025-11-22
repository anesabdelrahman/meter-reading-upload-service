using En.Metering.Validation.Interfaces;

namespace En.Metering.Validation.Implementaions
{
    public sealed class AndSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public async Task<(bool, string)> IsSatisfiedByAsync(T entity, CancellationToken token)
        {
            var leftResult = await _left.IsSatisfiedByAsync(entity, token);
            if (!leftResult.Item1)
            {
                return (false, leftResult.Item2);
            }

            var rightResult = await _right.IsSatisfiedByAsync(entity, token);
            if (!rightResult.Item1)
            {
                return (false, rightResult.Item2);
            }

            return (true, string.Empty);
        }
    }
}
