using ELODIE.Common;
using ELODIE.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ELODIE.Repositories
{
    public interface IUOW : IServiceScoped, IDisposable
    {
        Task Begin();
        Task Commit();
        Task Rollback();

        IAppUserRepository AppUserRepository { get; }
        IBrandRepository BrandRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ICodeGeneratorRuleRepository CodeGeneratorRuleRepository { get; }
        IColorRepository ColorRepository { get; }
        IDistrictRepository DistrictRepository { get; }
        IEntityComponentRepository EntityComponentRepository { get; }
        IEntityTypeRepository EntityTypeRepository { get; }
        IFieldRepository FieldRepository { get; }
        IIdGeneratorRepository IdGenerateRepository { get; }
        IImageRepository ImageRepository { get; }
        IItemRepository ItemRepository { get; }
        IItemHistoryRepository ItemHistoryRepository { get; }
        IMenuRepository MenuRepository { get; }
        INationRepository NationRepository { get; }
        IOrganizationRepository OrganizationRepository { get; }
        IPermissionOperatorRepository PermissionOperatorRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductGroupingRepository ProductGroupingRepository { get; }
        IProductTypeRepository ProductTypeRepository { get; }
        IProvinceRepository ProvinceRepository { get; }
        IRequestStateRepository RequestStateRepository { get; }
        IRequestWorkflowDefinitionMappingRepository RequestWorkflowDefinitionMappingRepository { get; }
        IRequestWorkflowHistoryRepository RequestWorkflowHistoryRepository { get; }
        IRequestWorkflowParameterMappingRepository RequestWorkflowParameterMappingRepository { get; }
        IRequestWorkflowStepMappingRepository RequestWorkflowStepMappingRepository { get; }
        IRoleRepository RoleRepository { get; }
        ISexRepository SexRepository { get; }
        ISiteRepository SiteRepository { get; }
        IStatusRepository StatusRepository { get; }
        ISupplierRepository SupplierRepository { get; }
        ITaxTypeRepository TaxTypeRepository { get; }
        IThemeRepository ThemeRepository { get; }
        IUnitOfMeasureRepository UnitOfMeasureRepository { get; }
        IUnitOfMeasureGroupingContentRepository UnitOfMeasureGroupingContentRepository { get; }
        IUnitOfMeasureGroupingRepository UnitOfMeasureGroupingRepository { get; }
        IUsedVariationRepository UsedVariationRepository { get; }
        IVariationRepository VariationRepository { get; }
        IVariationGroupingRepository VariationGroupingRepository { get; }
        IWardRepository WardRepository { get; }
        IWorkflowDefinitionRepository WorkflowDefinitionRepository { get; }
        IWorkflowDirectionRepository WorkflowDirectionRepository { get; }
        IWorkflowOperatorRepository WorkflowOperatorRepository { get; }
        IWorkflowParameterRepository WorkflowParameterRepository { get; }
        IWorkflowParameterTypeRepository WorkflowParameterTypeRepository { get; }
        IWorkflowStateRepository WorkflowStateRepository { get; }
        IWorkflowStepRepository WorkflowStepRepository { get; }
        IWorkflowTypeRepository WorkflowTypeRepository { get; }
        IRequestHistoryRepository RequestHistoryRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        ICustomerGroupingRepository CustomerGroupingRepository { get; }
        ICustomerSourceRepository CustomerSourceRepository { get; }
        IProfessionRepository ProfessionRepository { get; }
    }

    public class UOW : IUOW
    {
        private DataContext DataContext;
        public IAppUserRepository AppUserRepository { get; private set; }
        public IBrandRepository BrandRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }
        public ICodeGeneratorRuleRepository CodeGeneratorRuleRepository { get; private set; }
        public IColorRepository ColorRepository { get; private set; }
        public IDistrictRepository DistrictRepository { get; private set; }
        public IFieldRepository FieldRepository { get; private set; }
        public IEntityComponentRepository EntityComponentRepository { get; private set; } 
        public IEntityTypeRepository EntityTypeRepository { get; private set; }
        public IIdGeneratorRepository IdGenerateRepository { get; private set; }
        public IImageRepository ImageRepository { get; private set; }
        public IItemRepository ItemRepository { get; private set; }
        public IItemHistoryRepository ItemHistoryRepository { get; private set; }
        public IMenuRepository MenuRepository { get; private set; }
        public INationRepository NationRepository { get; private set; }
        public IOrganizationRepository OrganizationRepository { get; private set; }
        public IPermissionOperatorRepository PermissionOperatorRepository { get; private set; }
        public IPermissionRepository PermissionRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IProductGroupingRepository ProductGroupingRepository { get; private set; }
        public IProductTypeRepository ProductTypeRepository { get; private set; }
        public IProvinceRepository ProvinceRepository { get; private set; }
        public IRequestStateRepository RequestStateRepository { get; private set; }
        public IRequestWorkflowDefinitionMappingRepository RequestWorkflowDefinitionMappingRepository { get; private set; }
        public IRequestWorkflowHistoryRepository RequestWorkflowHistoryRepository { get; private set; }
        public IRequestWorkflowParameterMappingRepository RequestWorkflowParameterMappingRepository { get; private set; }
        public IRequestWorkflowStepMappingRepository RequestWorkflowStepMappingRepository { get; private set; }
        public IRoleRepository RoleRepository { get; private set; }
        public ISexRepository SexRepository { get; private set; }
        public ISiteRepository SiteRepository { get; private set; }
        public IStatusRepository StatusRepository { get; private set; }
        public ISupplierRepository SupplierRepository { get; private set; }
        public ITaxTypeRepository TaxTypeRepository { get; private set; }
        public IThemeRepository ThemeRepository { get; private set; }
        public IUnitOfMeasureRepository UnitOfMeasureRepository { get; private set; }
        public IUnitOfMeasureGroupingContentRepository UnitOfMeasureGroupingContentRepository { get; private set; }
        public IUnitOfMeasureGroupingRepository UnitOfMeasureGroupingRepository { get; private set; }
        public IUsedVariationRepository UsedVariationRepository { get; private set; }
        public IVariationRepository VariationRepository { get; private set; }
        public IVariationGroupingRepository VariationGroupingRepository { get; private set; }
        public IWardRepository WardRepository { get; private set; }
        public IWorkflowDefinitionRepository WorkflowDefinitionRepository { get; private set; }
        public IWorkflowDirectionRepository WorkflowDirectionRepository { get; private set; }
        public IWorkflowOperatorRepository WorkflowOperatorRepository { get; private set; }
        public IWorkflowParameterRepository WorkflowParameterRepository { get; private set; }
        public IWorkflowParameterTypeRepository WorkflowParameterTypeRepository { get; private set; }
        public IWorkflowStateRepository WorkflowStateRepository { get; private set; }
        public IWorkflowStepRepository WorkflowStepRepository { get; private set; }
        public IWorkflowTypeRepository WorkflowTypeRepository { get; private set; }
        public IRequestHistoryRepository RequestHistoryRepository { get; private set; }
        public ICustomerRepository CustomerRepository { get; private set; }
        public ICustomerGroupingRepository CustomerGroupingRepository { get; private set; }
        public ICustomerSourceRepository CustomerSourceRepository { get; private set; }
        public IProfessionRepository ProfessionRepository { get; private set; }
        public UOW(DataContext DataContext, IConfiguration Configuration)
        {
            Repositories.RequestHistoryRepository.ConnectionString = Configuration["MongoConnection:ConnectionString"];
            Repositories.RequestHistoryRepository.DatabaseName = Configuration["MongoConnection:Database"];

            this.DataContext = DataContext;

            AppUserRepository = new AppUserRepository(DataContext);
            BrandRepository = new BrandRepository(DataContext);
            CategoryRepository = new CategoryRepository(DataContext);
            CodeGeneratorRuleRepository = new CodeGeneratorRuleRepository(DataContext);
            ColorRepository = new ColorRepository(DataContext);
            DistrictRepository = new DistrictRepository(DataContext);
            FieldRepository = new FieldRepository(DataContext);
            EntityComponentRepository = new EntityComponentRepository(DataContext);
            EntityTypeRepository = new EntityTypeRepository(DataContext);
            IdGenerateRepository = new IdGeneratorRepository(DataContext);
            ImageRepository = new ImageRepository(DataContext);
            ItemRepository = new ItemRepository(DataContext);
            ItemHistoryRepository = new ItemHistoryRepository(DataContext);
            MenuRepository = new MenuRepository(DataContext);
            NationRepository = new NationRepository(DataContext);
            OrganizationRepository = new OrganizationRepository(DataContext);
            PermissionOperatorRepository = new PermissionOperatorRepository(DataContext);
            PermissionRepository = new PermissionRepository(DataContext);
            ProductRepository = new ProductRepository(DataContext);
            ProductGroupingRepository = new ProductGroupingRepository(DataContext);
            ProductTypeRepository = new ProductTypeRepository(DataContext);
            ProvinceRepository = new ProvinceRepository(DataContext);
            RequestStateRepository = new RequestStateRepository(DataContext);
            RequestWorkflowDefinitionMappingRepository = new RequestWorkflowDefinitionMappingRepository(DataContext);
            RequestWorkflowHistoryRepository = new RequestWorkflowHistoryRepository(DataContext);
            RequestWorkflowParameterMappingRepository = new RequestWorkflowParameterMappingRepository(DataContext);
            RequestWorkflowStepMappingRepository = new RequestWorkflowStepMappingRepository(DataContext);
            RoleRepository = new RoleRepository(DataContext);
            SexRepository = new SexRepository(DataContext);
            SiteRepository = new SiteRepository(DataContext);
            StatusRepository = new StatusRepository(DataContext);
            SupplierRepository = new SupplierRepository(DataContext);
            TaxTypeRepository = new TaxTypeRepository(DataContext);
            ThemeRepository = new ThemeRepository(DataContext);
            UnitOfMeasureRepository = new UnitOfMeasureRepository(DataContext);
            UnitOfMeasureGroupingContentRepository = new UnitOfMeasureGroupingContentRepository(DataContext);
            UnitOfMeasureGroupingRepository = new UnitOfMeasureGroupingRepository(DataContext);
            UsedVariationRepository = new UsedVariationRepository(DataContext);
            VariationRepository = new VariationRepository(DataContext);
            VariationGroupingRepository = new VariationGroupingRepository(DataContext);
            WardRepository = new WardRepository(DataContext);
            WorkflowDefinitionRepository = new WorkflowDefinitionRepository(DataContext);
            WorkflowDirectionRepository = new WorkflowDirectionRepository(DataContext);
            WorkflowOperatorRepository = new WorkflowOperatorRepository(DataContext);
            WorkflowParameterRepository = new WorkflowParameterRepository(DataContext);
            WorkflowParameterTypeRepository = new WorkflowParameterTypeRepository(DataContext);
            WorkflowStateRepository = new WorkflowStateRepository(DataContext);
            WorkflowStepRepository = new WorkflowStepRepository(DataContext);
            WorkflowTypeRepository = new WorkflowTypeRepository(DataContext);
            RequestHistoryRepository = new RequestHistoryRepository(DataContext);
            CustomerRepository = new CustomerRepository(DataContext);
            CustomerGroupingRepository = new CustomerGroupingRepository(DataContext);
            CustomerSourceRepository = new CustomerSourceRepository(DataContext);
            ProfessionRepository = new ProfessionRepository(DataContext);
        }
        public async Task Begin()
        {
            return;
        }

        public Task Commit()
        {
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.DataContext == null)
            {
                return;
            }

            this.DataContext.Dispose();
            this.DataContext = null;
        }
    }
}