using ELODIE.Common;
using ELODIE.Entities;

namespace ELODIE.Rpc.role
{
    public class Role_PermissionActionMappingDTO : DataDTO
    {
        public long PermissionId { get; set; }
        public long ActionId { get; set; }
        public Role_ActionDTO Action { get; set; }

        public Role_PermissionActionMappingDTO() { }
        public Role_PermissionActionMappingDTO(PermissionActionMapping PermissionActionMapping)
        {
            this.PermissionId = PermissionActionMapping.PermissionId;
            this.ActionId = PermissionActionMapping.ActionId;
            this.Action = PermissionActionMapping.Action == null ? null : new Role_ActionDTO(PermissionActionMapping.Action);

            this.Errors = PermissionActionMapping.Errors;
        }
    }

    public class Role_PermissionPageMappingFilterDTO : FilterDTO
    {

        public IdFilter PermissionId { get; set; }

        public IdFilter PageId { get; set; }

        public PermissionPageMappingOrder OrderBy { get; set; }
    }
}
