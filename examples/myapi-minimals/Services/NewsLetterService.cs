using myapi_minimals.DTOs;
using myapi_minimals.infra.Models;
using myapi_minimals.Repository;

namespace myapi_minimals.Services
{
    public class NewsLetterService : INewsLetterService
    {
        private readonly IRepository<NewsLetter> _newslettersRepository;

        public NewsLetterService(IRepository<NewsLetter> newslettersRepository)
        {
            _newslettersRepository = newslettersRepository;
        }

        public Task SubscribeAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(NewsLetterDto update, bool autoSave = true)
        {
            await _newslettersRepository.AddAsync(new NewsLetter
            {
                Id = Guid.NewGuid(),
                Content = update.Content,
                CreatedAt = DateTime.UtcNow,
                Date = update.Date ?? DateTime.UtcNow.Date,
                Title = update.Title,
            }, autoSave: autoSave);
        }

        public async Task<List<NewsLetterDto>> List(DateTime? since)
        {
            since ??= DateTime.UtcNow.AddDays(-7);
            var results = await _newslettersRepository.GetAllAsync(_ => _.Date >= since);
            return results.Select(_ => new NewsLetterDto
            {
                Id = _.Id.ToString(),
                Title = _.Title,
                Content = _.Content,
                Date = _.Date
            }).ToList();
        }
    }
    public interface INewsLetterService
    {
        Task SubscribeAsync(string email);
        Task UnsubscribeAsync(string email);
        Task<List<NewsLetterDto>> List(DateTime? since);
        Task AddAsync(NewsLetterDto update, bool autoSave = true);
    }
}
