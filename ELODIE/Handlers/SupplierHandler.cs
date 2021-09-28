using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Models;
using ELODIE.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Handlers
{
    public class SupplierHandler : Handler
    {
        private string UsedKey => $"{Name}.Used";
        private string UpdateKey => $"{Name}.Update";
        public override string Name => nameof(Supplier);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == UsedKey)
                await Used(UOW, content);
            else if (routingKey == UpdateKey)
                await Update(UOW, content);
        }

        private async Task Used(IUOW UOW, string json)
        {
            List<Supplier> Suppliers = JsonConvert.DeserializeObject<List<Supplier>>(json);
            List<long> Ids = Suppliers.Select(a => a.Id).Distinct().ToList();
            try
            {
                await UOW.SupplierRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(SupplierHandler));
            }
        }

        private async Task Update(IUOW UOW, string json)
        {
            List<Supplier> Suppliers = JsonConvert.DeserializeObject<List<Supplier>>(json);
            await UOW.SupplierRepository.BulkUpdate(Suppliers);
            List<long> Ids = Suppliers.Select(x => x.Id).ToList();
            Suppliers = await UOW.SupplierRepository.List(Ids);
            Sync(Suppliers);
        }

        private void Sync(List<Supplier> Suppliers)
        {
            List<AppUser> AppUsers = Suppliers.Where(x => x.PersonInChargeId.HasValue).Select(x => new AppUser { Id = x.PersonInChargeId.Value }).Distinct().ToList();
            List<Nation> Nations = Suppliers.Where(x => x.NationId.HasValue).Select(x => new Nation { Id = x.NationId.Value }).Distinct().ToList();
            List<Province> Provinces = Suppliers.Where(x => x.ProvinceId.HasValue).Select(x => new Province { Id = x.ProvinceId.Value }).Distinct().ToList();
            List<District> Districts = Suppliers.Where(x => x.DistrictId.HasValue).Select(x => new District { Id = x.DistrictId.Value }).Distinct().ToList();
            List<Ward> Wards = Suppliers.Where(x => x.WardId.HasValue).Select(x => new Ward { Id = x.WardId.Value }).Distinct().ToList();

            RabbitManager.PublishList(Suppliers, RoutingKeyEnum.SupplierSync);
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserUsed);
            RabbitManager.PublishList(Nations, RoutingKeyEnum.NationUsed);
            RabbitManager.PublishList(Provinces, RoutingKeyEnum.ProvinceUsed);
            RabbitManager.PublishList(Districts, RoutingKeyEnum.DistrictUsed);
            RabbitManager.PublishList(Wards, RoutingKeyEnum.WardUsed);
        }

    }
}
