using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ELODIE.Common;
using ELODIE.Entities;
using System;

namespace ELODIE.Rpc.product
{
    public class Product_ProductHistoryDTO : DataEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public long RequestId { get; set; }
        public DateTime SavedAt { get; set; }
        public Product_ProductDTO OldContent { get; set; }
        public Product_ProductDTO Content { get; set; }
        public long? AppUserId { get; set; }
        public Product_AppUserDTO AppUser { get; set; }

        public Product_ProductHistoryDTO() { }

        public Product_ProductHistoryDTO(RequestHistory<Product> requestHistory)
        {
            this.Id = requestHistory.Id;

            this.RequestId = requestHistory.RequestId;

            this.SavedAt = requestHistory.SavedAt;

            this.OldContent = requestHistory.OldContent == null ? null : new Product_ProductDTO(requestHistory.OldContent);

            this.Content = requestHistory.Content == null ? null : new Product_ProductDTO(requestHistory.Content);

            this.AppUserId = requestHistory.AppUserId;

            this.AppUser = requestHistory.AppUser == null ? null : new Product_AppUserDTO(requestHistory.AppUser);


            object MethodObject;
            if (requestHistory.DisplayFields.TryGetValue("Method", out MethodObject))
            {
                this.Name = (string)MethodObject;
            }

        }

    }
}
