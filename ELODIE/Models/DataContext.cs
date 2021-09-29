using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ELODIE.Models
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<ActionDAO> Action { get; set; }
        public virtual DbSet<ActionPageMappingDAO> ActionPageMapping { get; set; }
        public virtual DbSet<AggregatedCounterDAO> AggregatedCounter { get; set; }
        public virtual DbSet<AppUserDAO> AppUser { get; set; }
        public virtual DbSet<AppUserPermissionDAO> AppUserPermission { get; set; }
        public virtual DbSet<AppUserRoleMappingDAO> AppUserRoleMapping { get; set; }
        public virtual DbSet<AppUserSiteMappingDAO> AppUserSiteMapping { get; set; }
        public virtual DbSet<BrandDAO> Brand { get; set; }
        public virtual DbSet<CategoryDAO> Category { get; set; }
        public virtual DbSet<CodeGeneratorRuleDAO> CodeGeneratorRule { get; set; }
        public virtual DbSet<CodeGeneratorRuleEntityComponentMappingDAO> CodeGeneratorRuleEntityComponentMapping { get; set; }
        public virtual DbSet<ColorDAO> Color { get; set; }
        public virtual DbSet<CounterDAO> Counter { get; set; }
        public virtual DbSet<DistrictDAO> District { get; set; }
        public virtual DbSet<EntityComponentDAO> EntityComponent { get; set; }
        public virtual DbSet<EntityTypeDAO> EntityType { get; set; }
        public virtual DbSet<FieldDAO> Field { get; set; }
        public virtual DbSet<FieldTypeDAO> FieldType { get; set; }
        public virtual DbSet<HashDAO> Hash { get; set; }
        public virtual DbSet<IdGeneratorDAO> IdGenerator { get; set; }
        public virtual DbSet<ImageDAO> Image { get; set; }
        public virtual DbSet<ItemDAO> Item { get; set; }
        public virtual DbSet<ItemHistoryDAO> ItemHistory { get; set; }
        public virtual DbSet<ItemImageMappingDAO> ItemImageMapping { get; set; }
        public virtual DbSet<ItemVariationMappingDAO> ItemVariationMapping { get; set; }
        public virtual DbSet<JobDAO> Job { get; set; }
        public virtual DbSet<JobParameterDAO> JobParameter { get; set; }
        public virtual DbSet<JobQueueDAO> JobQueue { get; set; }
        public virtual DbSet<ListDAO> List { get; set; }
        public virtual DbSet<MenuDAO> Menu { get; set; }
        public virtual DbSet<NationDAO> Nation { get; set; }
        public virtual DbSet<OrganizationDAO> Organization { get; set; }
        public virtual DbSet<PageDAO> Page { get; set; }
        public virtual DbSet<PermissionDAO> Permission { get; set; }
        public virtual DbSet<PermissionActionMappingDAO> PermissionActionMapping { get; set; }
        public virtual DbSet<PermissionContentDAO> PermissionContent { get; set; }
        public virtual DbSet<PermissionOperatorDAO> PermissionOperator { get; set; }
        public virtual DbSet<ProductDAO> Product { get; set; }
        public virtual DbSet<ProductGroupingDAO> ProductGrouping { get; set; }
        public virtual DbSet<ProductImageMappingDAO> ProductImageMapping { get; set; }
        public virtual DbSet<ProductProductGroupingMappingDAO> ProductProductGroupingMapping { get; set; }
        public virtual DbSet<ProductTypeDAO> ProductType { get; set; }
        public virtual DbSet<ProvinceDAO> Province { get; set; }
        public virtual DbSet<RequestStateDAO> RequestState { get; set; }
        public virtual DbSet<RequestWorkflowDefinitionMappingDAO> RequestWorkflowDefinitionMapping { get; set; }
        public virtual DbSet<RequestWorkflowHistoryDAO> RequestWorkflowHistory { get; set; }
        public virtual DbSet<RequestWorkflowParameterMappingDAO> RequestWorkflowParameterMapping { get; set; }
        public virtual DbSet<RequestWorkflowStepMappingDAO> RequestWorkflowStepMapping { get; set; }
        public virtual DbSet<RoleDAO> Role { get; set; }
        public virtual DbSet<SchemaDAO> Schema { get; set; }
        public virtual DbSet<ServerDAO> Server { get; set; }
        public virtual DbSet<SetDAO> Set { get; set; }
        public virtual DbSet<SexDAO> Sex { get; set; }
        public virtual DbSet<SiteDAO> Site { get; set; }
        public virtual DbSet<StateDAO> State { get; set; }
        public virtual DbSet<StatusDAO> Status { get; set; }
        public virtual DbSet<SupplierDAO> Supplier { get; set; }
        public virtual DbSet<SupplierBankAccountDAO> SupplierBankAccount { get; set; }
        public virtual DbSet<SupplierCategoryMappingDAO> SupplierCategoryMapping { get; set; }
        public virtual DbSet<SupplierContactorDAO> SupplierContactor { get; set; }
        public virtual DbSet<TaxTypeDAO> TaxType { get; set; }
        public virtual DbSet<ThemeDAO> Theme { get; set; }
        public virtual DbSet<UnitOfMeasureDAO> UnitOfMeasure { get; set; }
        public virtual DbSet<UnitOfMeasureGroupingDAO> UnitOfMeasureGrouping { get; set; }
        public virtual DbSet<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContent { get; set; }
        public virtual DbSet<UsedVariationDAO> UsedVariation { get; set; }
        public virtual DbSet<VariationDAO> Variation { get; set; }
        public virtual DbSet<VariationGroupingDAO> VariationGrouping { get; set; }
        public virtual DbSet<WardDAO> Ward { get; set; }
        public virtual DbSet<WarehouseDAO> Warehouse { get; set; }
        public virtual DbSet<WorkflowDefinitionDAO> WorkflowDefinition { get; set; }
        public virtual DbSet<WorkflowDirectionDAO> WorkflowDirection { get; set; }
        public virtual DbSet<WorkflowDirectionConditionDAO> WorkflowDirectionCondition { get; set; }
        public virtual DbSet<WorkflowOperatorDAO> WorkflowOperator { get; set; }
        public virtual DbSet<WorkflowParameterDAO> WorkflowParameter { get; set; }
        public virtual DbSet<WorkflowParameterTypeDAO> WorkflowParameterType { get; set; }
        public virtual DbSet<WorkflowStateDAO> WorkflowState { get; set; }
        public virtual DbSet<WorkflowStepDAO> WorkflowStep { get; set; }
        public virtual DbSet<WorkflowTypeDAO> WorkflowType { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("data source=192.168.20.200;initial catalog=MDM;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActionDAO>(entity =>
            {
                entity.ToTable("Action", "PER");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.Actions)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Action_Menu");
            });

            modelBuilder.Entity<ActionPageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ActionId, e.PageId });

                entity.ToTable("ActionPageMapping", "PER");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.ActionPageMappings)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActionPageMapping_Action");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.ActionPageMappings)
                    .HasForeignKey(d => d.PageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActionPageMapping_Page");
            });

            modelBuilder.Entity<AggregatedCounterDAO>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<AppUserDAO>(entity =>
            {
                entity.ToTable("AppUser", "MDM");

                entity.Property(e => e.Id)
                    .HasComment("Id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ nhà");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(4000)
                    .HasComment("Ảnh đại diện");

                entity.Property(e => e.Birthday)
                    .HasColumnType("datetime")
                    .HasComment("Ngày sinh");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tạo");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày xoá");

                entity.Property(e => e.Department)
                    .HasMaxLength(500)
                    .HasComment("Phòng ban");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(500)
                    .HasComment("Tên hiển thị");

                entity.Property(e => e.Email)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ email");

                entity.Property(e => e.OtpCode).HasMaxLength(50);

                entity.Property(e => e.OtpExpired).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    //.IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.OrganizationId).HasComment("Đơn vị công tác");

                entity.Property(e => e.Phone)
                    .HasMaxLength(500)
                    .HasComment("Số điện thoại liên hệ");

                entity.Property(e => e.RowId).HasComment("Trường để đồng bộ");

                entity.Property(e => e.StatusId).HasComment("Trạng thái");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày cập nhật");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasComment("Tên đăng nhập");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUser_Organization");

                entity.HasOne(d => d.Sex)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.SexId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUser_Sex");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUser_UserStatus");
            });

            modelBuilder.Entity<AppUserPermissionDAO>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("AppUserPermission", "PER");

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(400);
            });

            modelBuilder.Entity<AppUserRoleMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.AppUserId, e.RoleId })
                    .HasName("PK_UserRoleMapping");

                entity.ToTable("AppUserRoleMapping", "MDM");

                entity.Property(e => e.AppUserId).HasComment("Id nhân viên");

                entity.Property(e => e.RoleId).HasComment("Id nhóm quyền");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.AppUserRoleMappings)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserRoleMapping_AppUser");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AppUserRoleMappings)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserRoleMapping_Role");
            });

            modelBuilder.Entity<AppUserSiteMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.AppUserId, e.SiteId });

                entity.ToTable("AppUserSiteMapping", "MDM");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.AppUserSiteMappings)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserSiteMapping_AppUser");

                entity.HasOne(d => d.Site)
                    .WithMany(p => p.AppUserSiteMappings)
                    .HasForeignKey(d => d.SiteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUserSiteMapping_Site");
            });

            modelBuilder.Entity<BrandDAO>(entity =>
            {
                entity.ToTable("Brand", "MDM");

                entity.Property(e => e.Id).HasComment("Id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasComment("Mã nhãn hiệu");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tạo");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày xoá");

                entity.Property(e => e.Description)
                    .HasMaxLength(2000)
                    .HasComment("Mô tả");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasComment("Tên nhãn nhiệu");

                entity.Property(e => e.StatusId).HasComment("Trạng thái hoạt động");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày cập nhật");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Brands)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Brand_Status");
            });

            modelBuilder.Entity<CategoryDAO>(entity =>
            {
                entity.ToTable("Category", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Prefix).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_Category_Image");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Category_Category");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Category_Status");
            });

            modelBuilder.Entity<CodeGeneratorRuleDAO>(entity =>
            {
                entity.ToTable("CodeGeneratorRule", "MDM");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.EntityType)
                    .WithMany(p => p.CodeGeneratorRules)
                    .HasForeignKey(d => d.EntityTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CodeGeneratorRule_EntityType");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.CodeGeneratorRules)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CodeGeneratorRule_Status");
            });

            modelBuilder.Entity<CodeGeneratorRuleEntityComponentMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.CodeGeneratorRuleId, e.EntityComponentId });

                entity.ToTable("CodeGeneratorRuleEntityComponentMapping", "MDM");

                entity.Property(e => e.Value).HasMaxLength(500);

                entity.HasOne(d => d.CodeGeneratorRule)
                    .WithMany(p => p.CodeGeneratorRuleEntityComponentMappings)
                    .HasForeignKey(d => d.CodeGeneratorRuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CodeGeneratorRuleEntityComponentMapping_CodeGeneratorRule");

                entity.HasOne(d => d.EntityComponent)
                    .WithMany(p => p.CodeGeneratorRuleEntityComponentMappings)
                    .HasForeignKey(d => d.EntityComponentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CodeGeneratorRuleEntityComponentMapping_EntityComponent");
            });

            modelBuilder.Entity<ColorDAO>(entity =>
            {
                entity.ToTable("Color", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CounterDAO>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Counter", "HangFire");

                entity.HasIndex(e => e.Key)
                    .HasName("CX_HangFire_Counter")
                    .IsClustered();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<DistrictDAO>(entity =>
            {
                entity.ToTable("District", "MDM");

                entity.Property(e => e.Id).HasComment("Id");

                entity.Property(e => e.Code)
                    .HasMaxLength(500)
                    .HasComment("Mã quận huyện");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tạo");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày xoá");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasComment("Tên quận huyện");

                entity.Property(e => e.Priority).HasComment("Thứ tự ưu tiên");

                entity.Property(e => e.ProvinceId).HasComment("Tỉnh phụ thuộc");

                entity.Property(e => e.RowId).HasComment("Trường để đồng bộ");

                entity.Property(e => e.StatusId).HasComment("Trạng thái hoạt động");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày cập nhật");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.ProvinceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_District_Province");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_District_Status");
            });

            modelBuilder.Entity<EntityComponentDAO>(entity =>
            {
                entity.ToTable("EntityComponent", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<EntityTypeDAO>(entity =>
            {
                entity.ToTable("EntityType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<FieldDAO>(entity =>
            {
                entity.ToTable("Field", "PER");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.FieldType)
                    .WithMany(p => p.Fields)
                    .HasForeignKey(d => d.FieldTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Field_FieldType");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.Fields)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionField_Menu");
            });

            modelBuilder.Entity<FieldTypeDAO>(entity =>
            {
                entity.ToTable("FieldType", "PER");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<HashDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<IdGeneratorDAO>(entity =>
            {
                entity.ToTable("IdGenerator", "MDM");
            });

            modelBuilder.Entity<ImageDAO>(entity =>
            {
                entity.ToTable("Image", "MDM");

                entity.Property(e => e.Id)
                    .HasComment("Id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tạo");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày xoá");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .HasComment("Tên");

                entity.Property(e => e.ThumbnailUrl)
                    .HasMaxLength(4000)
                    .HasComment("Đường dẫn Url");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasComment("Ngày cập nhật");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .HasComment("Đường dẫn Url");
            });

            modelBuilder.Entity<ItemDAO>(entity =>
            {
                entity.ToTable("Item", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.ERPCode).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.RetailPrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ScanCode).HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Item_Product");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Item_Status");
            });

            modelBuilder.Entity<ItemHistoryDAO>(entity =>
            {
                entity.ToTable("ItemHistory", "MDM");

                entity.Property(e => e.NewPrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.OldPrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemHistories)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemHistory_Item");

                entity.HasOne(d => d.Modifier)
                    .WithMany(p => p.ItemHistories)
                    .HasForeignKey(d => d.ModifierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemHistory_AppUser");
            });

            modelBuilder.Entity<ItemImageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ItemId, e.ImageId });

                entity.ToTable("ItemImageMapping", "MDM");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.ItemImageMappings)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemImageMapping_Image");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemImageMappings)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemImageMapping_Item");
            });

            modelBuilder.Entity<ItemVariationMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ItemId, e.VariationId });

                entity.ToTable("ItemVariationMapping", "MDM");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemVariationMappings)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemVariationMapping_Item");

                entity.HasOne(d => d.Variation)
                    .WithMany(p => p.ItemVariationMappings)
                    .HasForeignKey(d => d.VariationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemVariationMapping_Variation");
            });

            modelBuilder.Entity<JobDAO>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.StateName)
                    .HasName("IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.HasIndex(e => new { e.StateName, e.ExpireAt })
                    .HasName("IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameterDAO>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameters)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueueDAO>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ListDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<MenuDAO>(entity =>
            {
                entity.ToTable("Menu", "PER");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(3000);

                entity.Property(e => e.Path).HasMaxLength(3000);
            });

            modelBuilder.Entity<NationDAO>(entity =>
            {
                entity.ToTable("Nation", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Nations)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Nation_Status");
            });

            modelBuilder.Entity<OrganizationDAO>(entity =>
            {
                entity.ToTable("Organization", "MDM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Organization_Organization");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Organizations)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organization_Status");
            });

            modelBuilder.Entity<PageDAO>(entity =>
            {
                entity.ToTable("Page", "PER");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(400);
            });

            modelBuilder.Entity<PermissionDAO>(entity =>
            {
                entity.ToTable("Permission", "PER");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_Menu");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_Role");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_Status");
            });

            modelBuilder.Entity<PermissionActionMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ActionId, e.PermissionId })
                    .HasName("PK_ActionPermissionMapping");

                entity.ToTable("PermissionActionMapping", "PER");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.PermissionActionMappings)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActionPermissionMapping_Action");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.PermissionActionMappings)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActionPermissionMapping_Permission");
            });

            modelBuilder.Entity<PermissionContentDAO>(entity =>
            {
                entity.ToTable("PermissionContent", "PER");

                entity.Property(e => e.Value).HasMaxLength(500);

                entity.HasOne(d => d.Field)
                    .WithMany(p => p.PermissionContents)
                    .HasForeignKey(d => d.FieldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionContent_Field");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.PermissionContents)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionContent_Permission");

                entity.HasOne(d => d.PermissionOperator)
                    .WithMany(p => p.PermissionContents)
                    .HasForeignKey(d => d.PermissionOperatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionContent_PermissionOperator");
            });

            modelBuilder.Entity<PermissionOperatorDAO>(entity =>
            {
                entity.ToTable("PermissionOperator", "PER");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.FieldType)
                    .WithMany(p => p.PermissionOperators)
                    .HasForeignKey(d => d.FieldTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionOperator_FieldType");
            });

            modelBuilder.Entity<ProductDAO>(entity =>
            {
                entity.ToTable("Product", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(3000);

                entity.Property(e => e.ERPCode).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(3000);

                entity.Property(e => e.Note).HasMaxLength(3000);

                entity.Property(e => e.OtherName).HasMaxLength(1000);

                entity.Property(e => e.RetailPrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ScanCode).HasMaxLength(500);

                entity.Property(e => e.SupplierCode).HasMaxLength(500);

                entity.Property(e => e.TechnicalName).HasMaxLength(1000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Product_Brand");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Category");

                entity.HasOne(d => d.CodeGeneratorRule)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CodeGeneratorRuleId)
                    .HasConstraintName("FK_Product_CodeGeneratorRule");

                entity.HasOne(d => d.ProductType)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_ProductType");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Status");

                entity.HasOne(d => d.TaxType)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.TaxTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_TaxType");

                entity.HasOne(d => d.UnitOfMeasureGrouping)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.UnitOfMeasureGroupingId)
                    .HasConstraintName("FK_Product_UnitOfMeasureGrouping");

                entity.HasOne(d => d.UnitOfMeasure)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.UnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_UnitOfMeasure");

                entity.HasOne(d => d.UsedVariation)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.UsedVariationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_UsedVariation");
            });

            modelBuilder.Entity<ProductGroupingDAO>(entity =>
            {
                entity.ToTable("ProductGrouping", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(3000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_GroupItem_GroupItem");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ProductGroupings)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductGrouping_Status");
            });

            modelBuilder.Entity<ProductImageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.ImageId });

                entity.ToTable("ProductImageMapping", "MDM");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.ProductImageMappings)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductImageMapping_Image");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImageMappings)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductImageMapping_Product");
            });

            modelBuilder.Entity<ProductProductGroupingMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.ProductGroupingId })
                    .HasName("PK_ProductProductGrouping");

                entity.ToTable("ProductProductGroupingMapping", "MDM");

                entity.HasOne(d => d.ProductGrouping)
                    .WithMany(p => p.ProductProductGroupingMappings)
                    .HasForeignKey(d => d.ProductGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductProductGroupingMapping_ProductGrouping");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductProductGroupingMappings)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductProductGroupingMapping_Product");
            });

            modelBuilder.Entity<ProductTypeDAO>(entity =>
            {
                entity.ToTable("ProductType", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(3000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ProductTypes)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductType_Status");
            });

            modelBuilder.Entity<ProvinceDAO>(entity =>
            {
                entity.ToTable("Province", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Provinces)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Province_Status");
            });

            modelBuilder.Entity<RequestStateDAO>(entity =>
            {
                entity.ToTable("RequestState", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<RequestWorkflowDefinitionMappingDAO>(entity =>
            {
                entity.HasKey(e => e.RequestId);

                entity.ToTable("RequestWorkflowDefinitionMapping", "WF");

                entity.Property(e => e.RequestId).ValueGeneratedNever();

                entity.HasOne(d => d.RequestState)
                    .WithMany(p => p.RequestWorkflowDefinitionMappings)
                    .HasForeignKey(d => d.RequestStateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestWorkflowDefinitionMapping_RequestState");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.RequestWorkflowDefinitionMappings)
                    .HasForeignKey(d => d.WorkflowDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestWorkflowDefinitionMapping_WorkflowDefinition");
            });

            modelBuilder.Entity<RequestWorkflowHistoryDAO>(entity =>
            {
                entity.HasKey(e => new { e.Counter, e.RequestId, e.WorkflowStepId });

                entity.ToTable("RequestWorkflowHistory", "WF");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.RequestWorkflowHistories)
                    .HasForeignKey(d => d.AppUserId)
                    .HasConstraintName("FK_RequestWorkflowHistory_AppUser");

                entity.HasOne(d => d.WorkflowState)
                    .WithMany(p => p.RequestWorkflowHistories)
                    .HasForeignKey(d => d.WorkflowStateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestWorkflowHistory_WorkflowState");

                entity.HasOne(d => d.WorkflowStep)
                    .WithMany(p => p.RequestWorkflowHistories)
                    .HasForeignKey(d => d.WorkflowStepId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestWorkflowHistory_WorkflowStep");
            });

            modelBuilder.Entity<RequestWorkflowParameterMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.WorkflowParameterId, e.RequestId });

                entity.ToTable("RequestWorkflowParameterMapping", "WF");

                entity.Property(e => e.Value).HasMaxLength(500);

                entity.HasOne(d => d.WorkflowParameter)
                    .WithMany(p => p.RequestWorkflowParameterMappings)
                    .HasForeignKey(d => d.WorkflowParameterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreWorkflowParameterMapping_WorkflowParameter");
            });

            modelBuilder.Entity<RequestWorkflowStepMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.RequestId, e.WorkflowStepId });

                entity.ToTable("RequestWorkflowStepMapping", "WF");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.RequestWorkflowStepMappings)
                    .HasForeignKey(d => d.AppUserId)
                    .HasConstraintName("FK_StoreWorkflow_AppUser");

                entity.HasOne(d => d.WorkflowState)
                    .WithMany(p => p.RequestWorkflowStepMappings)
                    .HasForeignKey(d => d.WorkflowStateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreWorkflow_WorkflowState");

                entity.HasOne(d => d.WorkflowStep)
                    .WithMany(p => p.RequestWorkflowStepMappings)
                    .HasForeignKey(d => d.WorkflowStepId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequestWorkflow_WorkflowStep");
            });

            modelBuilder.Entity<RoleDAO>(entity =>
            {
                entity.ToTable("Role", "PER");

                entity.Property(e => e.Code).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Role_Status");
            });

            modelBuilder.Entity<SchemaDAO>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<ServerDAO>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat)
                    .HasName("IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id).HasMaxLength(100);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<SetDAO>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score })
                    .HasName("IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<SexDAO>(entity =>
            {
                entity.ToTable("Sex", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SiteDAO>(entity =>
            {
                entity.ToTable("Site", "MDM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.HasOne(d => d.Theme)
                    .WithMany(p => p.Sites)
                    .HasForeignKey(d => d.ThemeId)
                    .HasConstraintName("FK_Site_Theme");
            });

            modelBuilder.Entity<StateDAO>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<StatusDAO>(entity =>
            {
                entity.ToTable("Status", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<SupplierDAO>(entity =>
            {
                entity.ToTable("Supplier", "MDM");

                entity.Property(e => e.Address).HasMaxLength(2000);

                entity.Property(e => e.Avatar).HasMaxLength(4000);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.OwnerName).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.TaxCode).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_Supplier_District");

                entity.HasOne(d => d.Nation)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.NationId)
                    .HasConstraintName("FK_Supplier_Nation");

                entity.HasOne(d => d.PersonInCharge)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.PersonInChargeId)
                    .HasConstraintName("FK_Supplier_AppUser");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Supplier_Province");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Supplier_Status");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.WardId)
                    .HasConstraintName("FK_Supplier_Ward");
            });

            modelBuilder.Entity<SupplierBankAccountDAO>(entity =>
            {
                entity.ToTable("SupplierBankAccount", "MDM");

                entity.Property(e => e.BankAccount)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.BankAccountOwnerName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.BankName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.SupplierBankAccounts)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupplierBankAccount_Supplier");
            });

            modelBuilder.Entity<SupplierCategoryMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.SupplierId, e.CategoryId });

                entity.ToTable("SupplierCategoryMapping", "MDM");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.SupplierCategoryMappings)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupplierCategoryMapping_Category");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.SupplierCategoryMappings)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupplierCategoryMapping_Supplier");
            });

            modelBuilder.Entity<SupplierContactorDAO>(entity =>
            {
                entity.ToTable("SupplierContactor", "MDM");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.SupplierContactors)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupplierContactor_Supplier");
            });

            modelBuilder.Entity<TaxTypeDAO>(entity =>
            {
                entity.ToTable("TaxType", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Percentage).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TaxTypes)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TaxType_Status");
            });

            modelBuilder.Entity<ThemeDAO>(entity =>
            {
                entity.ToTable("Theme", "MDM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<UnitOfMeasureDAO>(entity =>
            {
                entity.ToTable("UnitOfMeasure", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(3000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.UnitOfMeasures)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UnitOfMeasure_Status");
            });

            modelBuilder.Entity<UnitOfMeasureGroupingDAO>(entity =>
            {
                entity.ToTable("UnitOfMeasureGrouping", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.UnitOfMeasureGroupings)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UnitOfMeasureGrouping_Status");

                entity.HasOne(d => d.UnitOfMeasure)
                    .WithMany(p => p.UnitOfMeasureGroupings)
                    .HasForeignKey(d => d.UnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UnitOfMeasureGrouping_UnitOfMeasure");
            });

            modelBuilder.Entity<UnitOfMeasureGroupingContentDAO>(entity =>
            {
                entity.ToTable("UnitOfMeasureGroupingContent", "MDM");

                entity.HasOne(d => d.UnitOfMeasureGrouping)
                    .WithMany(p => p.UnitOfMeasureGroupingContents)
                    .HasForeignKey(d => d.UnitOfMeasureGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UnitOfMeasureGroupingContent_UnitOfMeasureGrouping");

                entity.HasOne(d => d.UnitOfMeasure)
                    .WithMany(p => p.UnitOfMeasureGroupingContents)
                    .HasForeignKey(d => d.UnitOfMeasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UnitOfMeasureGroupingContent_UnitOfMeasure");
            });

            modelBuilder.Entity<UsedVariationDAO>(entity =>
            {
                entity.ToTable("UsedVariation", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<VariationDAO>(entity =>
            {
                entity.ToTable("Variation", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.VariationGrouping)
                    .WithMany(p => p.Variations)
                    .HasForeignKey(d => d.VariationGroupingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Variation_VariationGrouping");
            });

            modelBuilder.Entity<VariationGroupingDAO>(entity =>
            {
                entity.ToTable("VariationGrouping", "MDM");

                entity.Property(e => e.Code).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.VariationGroupings)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VariationGrouping_Product");
            });

            modelBuilder.Entity<WardDAO>(entity =>
            {
                entity.ToTable("Ward", "MDM");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Wards)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ward_District");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Wards)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ward_Status");
            });

            modelBuilder.Entity<WarehouseDAO>(entity =>
            {
                entity.ToTable("Warehouse", "MDM");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_Warehouse_District");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Warehouse_Organization");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Warehouse_Province");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Warehouse_Status");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.WardId)
                    .HasConstraintName("FK_Warehouse_Ward");
            });

            modelBuilder.Entity<WorkflowDefinitionDAO>(entity =>
            {
                entity.ToTable("WorkflowDefinition", "WF");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.WorkflowDefinitionCreators)
                    .HasForeignKey(d => d.CreatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDefinition_AppUser");

                entity.HasOne(d => d.Modifier)
                    .WithMany(p => p.WorkflowDefinitionModifiers)
                    .HasForeignKey(d => d.ModifierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDefinition_AppUser1");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.WorkflowDefinitions)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDefinition_Organization");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.WorkflowDefinitions)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDefinition_Status");

                entity.HasOne(d => d.WorkflowType)
                    .WithMany(p => p.WorkflowDefinitions)
                    .HasForeignKey(d => d.WorkflowTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDefinition_WorkflowType");
            });

            modelBuilder.Entity<WorkflowDirectionDAO>(entity =>
            {
                entity.ToTable("WorkflowDirection", "WF");

                entity.Property(e => e.BodyMailForCreator).HasMaxLength(4000);

                entity.Property(e => e.BodyMailForCurrentStep).HasMaxLength(4000);

                entity.Property(e => e.BodyMailForNextStep).HasMaxLength(4000);

                entity.Property(e => e.SubjectMailForCreator).HasMaxLength(500);

                entity.Property(e => e.SubjectMailForCurrentStep).HasMaxLength(500);

                entity.Property(e => e.SubjectMailForNextStep).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.FromStep)
                    .WithMany(p => p.WorkflowDirectionFromSteps)
                    .HasForeignKey(d => d.FromStepId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirection_WorkflowStep");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.WorkflowDirections)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirection_Status");

                entity.HasOne(d => d.ToStep)
                    .WithMany(p => p.WorkflowDirectionToSteps)
                    .HasForeignKey(d => d.ToStepId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirection_WorkflowStep1");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.WorkflowDirections)
                    .HasForeignKey(d => d.WorkflowDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirection_WorkflowDefinition");
            });

            modelBuilder.Entity<WorkflowDirectionConditionDAO>(entity =>
            {
                entity.ToTable("WorkflowDirectionCondition", "WF");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.WorkflowDirection)
                    .WithMany(p => p.WorkflowDirectionConditions)
                    .HasForeignKey(d => d.WorkflowDirectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirectionCondition_WorkflowDirection");

                entity.HasOne(d => d.WorkflowOperator)
                    .WithMany(p => p.WorkflowDirectionConditions)
                    .HasForeignKey(d => d.WorkflowOperatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirectionCondition_WorkflowOperator");

                entity.HasOne(d => d.WorkflowParameter)
                    .WithMany(p => p.WorkflowDirectionConditions)
                    .HasForeignKey(d => d.WorkflowParameterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowDirectionCondition_WorkflowParameter");
            });

            modelBuilder.Entity<WorkflowOperatorDAO>(entity =>
            {
                entity.ToTable("WorkflowOperator", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.WorkflowParameterType)
                    .WithMany(p => p.WorkflowOperators)
                    .HasForeignKey(d => d.WorkflowParameterTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowOperator_WorkflowParameterType");
            });

            modelBuilder.Entity<WorkflowParameterDAO>(entity =>
            {
                entity.ToTable("WorkflowParameter", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.WorkflowParameterType)
                    .WithMany(p => p.WorkflowParameters)
                    .HasForeignKey(d => d.WorkflowParameterTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowParameter_WorkflowParameterType");

                entity.HasOne(d => d.WorkflowType)
                    .WithMany(p => p.WorkflowParameters)
                    .HasForeignKey(d => d.WorkflowTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowParameter_WorkflowType");
            });

            modelBuilder.Entity<WorkflowParameterTypeDAO>(entity =>
            {
                entity.ToTable("WorkflowParameterType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<WorkflowStateDAO>(entity =>
            {
                entity.ToTable("WorkflowState", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<WorkflowStepDAO>(entity =>
            {
                entity.ToTable("WorkflowStep", "WF");

                entity.Property(e => e.BodyMailForReject).HasMaxLength(4000);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.StatusId).HasDefaultValueSql("((1))");

                entity.Property(e => e.SubjectMailForReject).HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.WorkflowSteps)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowStep_Role");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.WorkflowSteps)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowStep_Status");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.WorkflowSteps)
                    .HasForeignKey(d => d.WorkflowDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkflowStep_WorkflowDefinition");
            });

            modelBuilder.Entity<WorkflowTypeDAO>(entity =>
            {
                entity.ToTable("WorkflowType", "ENUM");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
