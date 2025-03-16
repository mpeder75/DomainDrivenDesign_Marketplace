using Marketplace.Contracts;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Repositories;
using Marketplace.Domain.Services;
using Marketplace.Domain.ValueObjects;
using Marketplace.Framework;
using Marketplace.Framework.DomainService;
using static Marketplace.Contracts.ClassifiedAds;

namespace Marketplace.Api
{
    public class ClassifiedAdsApplicationService : IApplicationService
    {
        private readonly IClassifiedAdRepository _repository;
        private readonly ICurrencyLookup _currencyLookup;
        // Unit of work pattern
        private readonly IUnitOfWork _unitOfWork;

        public ClassifiedAdsApplicationService(IClassifiedAdRepository repository, 
            ICurrencyLookup currencyLookup, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _currencyLookup = currencyLookup;
            _unitOfWork = unitOfWork;
        }

        public Task Handle(object command) =>
            command switch
            {
                V1.Create cmd => HandleCreate(cmd),
                V1.SetTitle cmd => HandleUpdate(
                        cmd.Id,
                        c => c.SetTitle(
                            ClassifiedAdTitle.FromString(cmd.Title)
                        )
                    ),
                V1.UpdateText cmd =>
                    HandleUpdate(
                        cmd.Id,
                        c => c.UpdateText(
                            ClassifiedAdText.FromString(cmd.Text)
                        )
                    ),
                V1.UpdatePrice cmd =>
                    HandleUpdate(
                        cmd.Id,
                        c => c.UpdatePrice(
                            Price.FromDecimal(
                                cmd.Price,
                                cmd.Currency,
                                _currencyLookup
                            )
                        )
                    ),
                V1.RequestToPublish cmd =>
                    HandleUpdate(
                        cmd.Id,
                        c => c.RequestToPublish()
                    ),
                _ => Task.CompletedTask
            };


        private async Task HandleCreate(V1.Create cmd)
        {
            if (await _repository.Exists(cmd.Id.ToString()))
                throw new InvalidOperationException($"DomainEntity with id {cmd.Id} already exists");

            var classifiedAd = new ClassifiedAd(
                new ClassifiedAdId(cmd.Id),
                new UserId(cmd.OwnerId)
            );

            // Repository pattern bruges
            await _repository.Add(classifiedAd);
            // Unit of work pattern bruges
            await _unitOfWork.Commit();
        }

        private async Task HandleUpdate(Guid classifiedAdId, Action<ClassifiedAd> operation)
        {
            var classifiedAd = await _repository.Load(classifiedAdId.ToString());

            if (classifiedAd == null)
                throw new InvalidOperationException(
                    $"DomainEntity with id {classifiedAdId} cannot be found"
                );

            operation(classifiedAd);

            // Unit of work pattern bruges
            await _unitOfWork.Commit();
        }
    }
}