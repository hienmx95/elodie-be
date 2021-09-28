using ELODIE.Enums;
using ELODIE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Repositories
{
    public interface IIdGeneratorRepository
    {
        Task<long> GetCounter();
        Task<List<long>> ListCounter(long countElement);
    }
    public class IdGeneratorRepository : IIdGeneratorRepository
    {
        private DataContext DataContext;
        public IdGeneratorRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        public async Task<long> GetCounter()
        {
            var Counter = await DataContext.IdGenerator
                .Where(x => x.IdGeneratorTypeId == IdGeneratorTypeEnum.STORE.Id)
                .Where(x => x.Used)
                .MaxAsync(x => (long?)x.Counter) ?? 0;

            //chưa có cái nào đc sử dụng
            if (Counter == 0)
            {
                await DataContext.IdGenerator
                .Where(x => x.IdGeneratorTypeId == IdGeneratorTypeEnum.STORE.Id)
                .Where(x => x.Counter == 1)
                .UpdateFromQueryAsync(x => new IdGeneratorDAO
                {
                    Used = true
                });
                return 1;
            }
            else
            {
                await DataContext.IdGenerator
                .Where(x => x.IdGeneratorTypeId == IdGeneratorTypeEnum.STORE.Id)
                .Where(x => x.Counter == Counter + 1)
                .UpdateFromQueryAsync(x => new IdGeneratorDAO
                {
                    Used = true
                });
                return Counter + 1;
            }
        }

        public async Task<List<long>> ListCounter(long countElement)
        {
            var Counter = await DataContext.IdGenerator
                .Where(x => x.IdGeneratorTypeId == IdGeneratorTypeEnum.STORE.Id)
                .Where(x => x.Used)
                .MaxAsync(x => (long?)x.Counter) ?? 0;

            List<long> Counters = new List<long>();

            await DataContext.IdGenerator
            .Where(x => x.IdGeneratorTypeId == IdGeneratorTypeEnum.STORE.Id)
            .Where(x => Counter + 1 <= x.Counter && x.Counter <= Counter + countElement)
            .UpdateFromQueryAsync(x => new IdGeneratorDAO
            {
                Used = true
            });

            for (long i = Counter + 1; i <= Counter + countElement; i++)
            {
                Counters.Add(i);
            }

            return Counters;
        }
    }
}
