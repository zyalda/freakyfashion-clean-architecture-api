using ApplicationLayer.Interfaces;
using DomainLayer;
using InfrastructureLayer.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace InfrastructureLayer.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FashionContext _fashionContext;
        private IDbContextTransaction? currentTransaction;

        public UnitOfWork(FashionContext context)
        {
            _fashionContext = context;
            ProductRepository = new ProductRepository(_fashionContext);
            CategoryRepository = new CategoryRepository(_fashionContext);
            CustomerRepository = new CustomerRepository(_fashionContext);
            OrderRepository = new OrderRepository(_fashionContext);
            OrderItemRepository = new OrderItemRepository(_fashionContext);

        }

        public IProductRepository ProductRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }
        public ICustomerRepository CustomerRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }
        public IOrderItemRepository OrderItemRepository { get; private set; }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (currentTransaction != null || _fashionContext.Database.CurrentTransaction != null)
                return;

            currentTransaction = await _fashionContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _fashionContext.SaveChangesAsync(cancellationToken);

                if (currentTransaction != null)
                    await currentTransaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                //safely revert transaction.
                if (currentTransaction != null)
                {
                    await currentTransaction.RollbackAsync(CancellationToken.None);
                }
                throw;
            }
            finally
            {
                ClearTransaction();
            }
        }
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (currentTransaction != null)
                {
                    await currentTransaction.RollbackAsync(cancellationToken);
                }
            }
            finally
            {
                DisposeTransaction();
            }
        }

        private void DisposeTransaction()
        {
            if (currentTransaction != null)
            {
                currentTransaction.Dispose();
                currentTransaction = null;
            }
        }
        private void ClearTransaction()
        {
            if (currentTransaction != null)
            {
                currentTransaction.Dispose();
                currentTransaction = null;
            }
        }
        public int Complete()
        {
            return _fashionContext.SaveChanges();
        }

        public void Dispose()
        {
            _fashionContext.Dispose();
        }
    }
}
