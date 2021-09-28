using ELODIE.Common;
using ELODIE.Entities;
using ELODIE.Enums;
using ELODIE.Helpers;
using ELODIE.Services.MBrand;
using ELODIE.Services.MCategory;
using ELODIE.Services.MItemHistory;
using ELODIE.Services.MProduct;
using ELODIE.Services.MProductGrouping;
using ELODIE.Services.MProductType;
using ELODIE.Services.MStatus;
using ELODIE.Services.MSupplier;
using ELODIE.Services.MTaxType;
using ELODIE.Services.MUnitOfMeasure;
using ELODIE.Services.MUnitOfMeasureGrouping;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELODIE.Rpc.product
{

    public partial class ProductController : RpcController
    {
        private IBrandService BrandService;
        private ICategoryService CategoryService;
        private IProductTypeService ProductTypeService;
        private IStatusService StatusService;
        private ISupplierService SupplierService;
        private ITaxTypeService TaxTypeService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IUnitOfMeasureGroupingService UnitOfMeasureGroupingService;
        private IItemService ItemService;
        private IItemHistoryService ItemHistoryService;
        private IProductGroupingService ProductGroupingService;
        private IProductService ProductService;
        private IUsedVariationService UsedVariationService;
        private ICurrentContext CurrentContext;
        public ProductController(
            IBrandService BrandService,
            ICategoryService CategoryService,
            IProductTypeService ProductTypeService,
            IStatusService StatusService,
            ISupplierService SupplierService,
            ITaxTypeService TaxTypeService,
            IUnitOfMeasureService UnitOfMeasureService,
            IUnitOfMeasureGroupingService UnitOfMeasureGroupingService,
            IItemService ItemService,
            IItemHistoryService ItemHistoryService,
            IProductGroupingService ProductGroupingService,
            IProductService ProductService,
            IUsedVariationService UsedVariationService,
            ICurrentContext CurrentContext
        )
        {
            this.BrandService = BrandService;
            this.CategoryService = CategoryService;
            this.ProductTypeService = ProductTypeService;
            this.StatusService = StatusService;
            this.SupplierService = SupplierService;
            this.TaxTypeService = TaxTypeService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.UnitOfMeasureGroupingService = UnitOfMeasureGroupingService;
            this.ItemService = ItemService;
            this.ItemHistoryService = ItemHistoryService;
            this.ProductGroupingService = ProductGroupingService;
            this.ProductService = ProductService;
            this.UsedVariationService = UsedVariationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ProductRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            ProductFilter = ProductService.ToFilter(ProductFilter);
            int count = await ProductService.Count(ProductFilter);
            return count;
        }

        [Route(ProductRoute.List), HttpPost]
        public async Task<ActionResult<List<Product_ProductDTO>>> List([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            ProductFilter = ProductService.ToFilter(ProductFilter);
            List<Product> Products = await ProductService.List(ProductFilter);
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(c => new Product_ProductDTO(c)).ToList();
            return Product_ProductDTOs;
        }

        [Route(ProductRoute.Get), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Get([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = await ProductService.Get(Product_ProductDTO.Id);
            return new Product_ProductDTO(Product);
        }

        [Route(ProductRoute.GetItem), HttpPost]
        public async Task<ActionResult<Product_ItemDTO>> GetItem([FromBody] Product_ItemDTO Product_ItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Item Item = await ItemService.Get(Product_ItemDTO.Id);
            return new Product_ItemDTO(Item);
        }

        [Route(ProductRoute.Create), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Create([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = ConvertDTOToEntity(Product_ProductDTO);

            Product = await ProductService.Create(Product);
            Product_ProductDTO = new Product_ProductDTO(Product);
            if (Product.IsValidated)
                return Product_ProductDTO;
            else
                return BadRequest(Product_ProductDTO);
        }

        [Route(ProductRoute.Update), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Update([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = ConvertDTOToEntity(Product_ProductDTO);
            Product = await ProductService.Update(Product);
            Product_ProductDTO = new Product_ProductDTO(Product);
            if (Product.IsValidated)
                return Product_ProductDTO;
            else
                return BadRequest(Product_ProductDTO);
        }

        [Route(ProductRoute.Delete), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Delete([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = ConvertDTOToEntity(Product_ProductDTO);
            Product = await ProductService.Delete(Product);
            Product_ProductDTO = new Product_ProductDTO(Product);
            if (Product.IsValidated)
                return Product_ProductDTO;
            else
                return BadRequest(Product_ProductDTO);
        }

        [Route(ProductRoute.CheckCodeGeneratorRule), HttpPost]
        public async Task<ActionResult<bool>> CheckCodeGeneratorRule()
        {
            CodeGeneratorRule CodeGeneratorRule = await ProductService.GetCodeGeneratorRule();
            return CodeGeneratorRule != null;
        }

        [Route(ProductRoute.Import), HttpPost]
        public async Task<ActionResult<List<Product_ProductDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            FileInfo FileInfo = new FileInfo(file.FileName);
            if (!FileInfo.Extension.Equals(".xlsx"))
                return BadRequest("Định dạng file không hợp lệ");

            #region ELODIE
            List<Category> Categorys = await CategoryService.List(new CategoryFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = CategorySelect.ALL
            });
            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(new ProductGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductGroupingSelect.ALL
            });
            List<ProductType> ProductTypes = await ProductTypeService.List(new ProductTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductTypeSelect.ALL
            });
            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureSelect.ALL
            });
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureGroupingSelect.ALL
            });
            List<Supplier> Suppliers = await SupplierService.List(new SupplierFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SupplierSelect.ALL
            });
            List<Brand> Brands = await BrandService.List(new BrandFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = BrandSelect.ALL
            });
            List<TaxType> TaxTypes = await TaxTypeService.List(new TaxTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = TaxTypeSelect.ALL
            });
            List<UsedVariation> UsedVariations = await UsedVariationService.List(new UsedVariationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UsedVariationSelect.ALL
            });
            List<Status> Statuses = await StatusService.List(new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            });

            List<Product> ProductInDBs = await ProductService.List(new ProductFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductSelect.Code
            });
            #endregion
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };
            HashSet<string> ListProductCode = ProductInDBs.Select(x => x.Code).ToHashSet();
            List<Product> Products = new List<Product>();
            StringBuilder errorContent = new StringBuilder();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                #region ProductSheet
                ExcelWorksheet ProductSheet = excelPackage.Workbook.Worksheets["Sản phẩm"];
                if (ProductSheet == null)
                    return BadRequest("File không đúng biểu mẫu import");

                #region Khai báo thứ tự các cột trong Exel file 

                int StartColumn = 1;
                int StartRow = 2;

                int SttColumnn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int CategoryCodeColumn = 3 + StartColumn;
                int ProductGroupCodeColumn = 4 + StartColumn;
                int ProductTypeCodeColumn = 5 + StartColumn;
                int UoMCodeColumn = 6 + StartColumn;
                int UoMGroupCodeColumn = 7 + StartColumn;
                int ERPCodeColumn = 8 + StartColumn;
                int ScanCodeColumn = 9 + StartColumn;
                int BrandCodeColumn = 10 + StartColumn;
                int OtherNameColumn = 11 + StartColumn;
                int TechnicalNameColumn = 12 + StartColumn;
                int TaxTypeCodeColumn = 13 + StartColumn;
                int DescriptionColumn = 14 + StartColumn;
                int SalePriceColumn = 15 + StartColumn;
                int StatusIdColumn = 16 + StartColumn;
                int UsedVariationCodeColumn = 17 + StartColumn;

                int Property1Column = 18 + StartColumn;
                int PropertyValue1Column = 19 + StartColumn;
                int Property2Column = 20 + StartColumn;
                int PropertyValue2Column = 21 + StartColumn;
                int Property3Column = 22 + StartColumn;
                int PropertyValue3Column = 23 + StartColumn;
                int Property4Column = 24 + StartColumn;
                int PropertyValue4Column = 25 + StartColumn;
                #endregion

                var CodeList = new List<string>();

                for (int i = StartRow; i <= ProductSheet.Dimension.End.Row; i++)
                {
                    #region đọc dữ liệu các ô
                    string stt = ProductSheet.Cells[i, SttColumnn].Value?.ToString();
                    if (stt != null && stt.ToLower() == "END".ToLower())
                        break;
                    string CodeValue = ProductSheet.Cells[i, CodeColumn].Value?.ToString();
                    if (CodeList.Contains(CodeValue))
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Mã sản phẩm đã tồn tại trong file");
                    }
                    else if (CodeValue != null)
                        CodeList.Add(CodeValue); // nếu CodeValue ko bị trùng và khác rỗng
                    if (!string.IsNullOrWhiteSpace(CodeValue) && ListProductCode.Contains(CodeValue))
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Mã sản phẩm đã tồn tại");
                    }
                    else if (string.IsNullOrWhiteSpace(CodeValue) && i == ProductSheet.Dimension.End.Row)
                        break;
                    string NameValue = ProductSheet.Cells[i, NameColumn].Value?.ToString();
                    string CategoryCodeValue = ProductSheet.Cells[i, CategoryCodeColumn].Value?.ToString();
                    string ProductGroupCodeValue = ProductSheet.Cells[i, ProductGroupCodeColumn].Value?.ToString();
                    string ProductTypeCodeValue = ProductSheet.Cells[i, ProductTypeCodeColumn].Value?.ToString();
                    string UoMCodeValue = ProductSheet.Cells[i, UoMCodeColumn].Value?.ToString();
                    string UoMGroupCodeValue = ProductSheet.Cells[i, UoMGroupCodeColumn].Value?.ToString();
                    string ERPCodeValue = ProductSheet.Cells[i, ERPCodeColumn].Value?.ToString();
                    string ScanCodeValue = ProductSheet.Cells[i, ScanCodeColumn].Value?.ToString();
                    string BrandCodeValue = ProductSheet.Cells[i, BrandCodeColumn].Value?.ToString();
                    string OtherNameValue = ProductSheet.Cells[i, OtherNameColumn].Value?.ToString();
                    string TechnicalNameValue = ProductSheet.Cells[i, TechnicalNameColumn].Value?.ToString();
                    string TaxTypeCodeValue = ProductSheet.Cells[i, TaxTypeCodeColumn].Value?.ToString();
                    string DescriptionValue = ProductSheet.Cells[i, DescriptionColumn].Value?.ToString();
                    string SalePriceValue = ProductSheet.Cells[i, SalePriceColumn].Value?.ToString();
                    string StatusNameValue = ProductSheet.Cells[i, StatusIdColumn].Value?.ToString();
                    string UsedVariationCodeValue = ProductSheet.Cells[i, UsedVariationCodeColumn].Value?.ToString();
                    //Thuộc tính
                    string Property1Value = ProductSheet.Cells[i, Property1Column].Value?.ToString();
                    string PropertyValue1Value = ProductSheet.Cells[i, PropertyValue1Column].Value?.ToString();
                    string Property2Value = ProductSheet.Cells[i, Property2Column].Value?.ToString();
                    string PropertyValue2Value = ProductSheet.Cells[i, PropertyValue2Column].Value?.ToString();
                    string Property3Value = ProductSheet.Cells[i, Property3Column].Value?.ToString();
                    string PropertyValue3Value = ProductSheet.Cells[i, PropertyValue3Column].Value?.ToString();
                    string Property4Value = ProductSheet.Cells[i, Property4Column].Value?.ToString();
                    string PropertyValue4Value = ProductSheet.Cells[i, PropertyValue4Column].Value?.ToString();
                    #endregion

                    Product Product = new Product();
                    Product.Code = CodeValue;
                    Product.Name = NameValue;
                    Product.ERPCode = ERPCodeValue;
                    Product.ScanCode = ScanCodeValue;
                    Product.OtherName = OtherNameValue;
                    Product.TechnicalName = TechnicalNameValue;
                    Product.Description = DescriptionValue;
                    Product.RowId = Guid.NewGuid();
                    //Product Grouping
                    if (!string.IsNullOrEmpty(ProductGroupCodeValue))
                    {
                        var ProductGroupCodes = ProductGroupCodeValue.Split(';');
                        foreach (var ProductGroupCode in ProductGroupCodes)
                        {
                            ProductGrouping ProductGrouping = ProductGroupings.Where(pg => pg.Code.ToLower() == ProductGroupCode.Trim().ToLower()).FirstOrDefault();
                            if (ProductGrouping != null)
                            {
                                ProductProductGroupingMapping ProductProductGroupingMapping = new ProductProductGroupingMapping();
                                Product.ProductProductGroupingMappings = new List<ProductProductGroupingMapping>();
                                ProductProductGroupingMapping.ProductGroupingId = ProductGrouping.Id;
                                ProductProductGroupingMapping.ProductGrouping = ProductGrouping;
                                Product.ProductProductGroupingMappings.Add(ProductProductGroupingMapping);
                            }
                            else
                            {
                                errorContent.AppendLine($"Lỗi dòng thứ {i}: Nhóm sản phẩm không tồn tại");
                            }
                        }

                    }

                    if (!string.IsNullOrWhiteSpace(CategoryCodeValue))
                    {
                        Product.Category = new Category()
                        {
                            Code = CategoryCodeValue
                        };
                        Product.CategoryId = Categorys.Where(x => x.Code.ToLower() == CategoryCodeValue.Trim().ToLower()).Select(x => x.Id).FirstOrDefault();
                    }
                    else
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập danh mục sản phẩm");
                    }

                    if (!string.IsNullOrWhiteSpace(ProductTypeCodeValue))
                    {
                        Product.ProductType = new ProductType()
                        {
                            Code = ProductTypeCodeValue
                        };
                        Product.ProductTypeId = ProductTypes.Where(x => x.Code.ToLower() == ProductTypeCodeValue.Trim().ToLower()).Select(x => x.Id).FirstOrDefault();
                    }
                    else
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập loại sản phẩm");
                    }

                    if (!string.IsNullOrWhiteSpace(UoMCodeValue))
                    {
                        Product.UnitOfMeasure = new UnitOfMeasure()
                        {
                            Code = UoMCodeValue
                        };
                        Product.UnitOfMeasureId = UnitOfMeasures.Where(x => x.Code.ToLower() == UoMCodeValue.Trim().ToLower()).Select(x => x.Id).FirstOrDefault();
                    }
                    else
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập đơn vị tính");
                    }

                    if (!string.IsNullOrWhiteSpace(BrandCodeValue))
                    {
                        Product.Brand = new Brand()
                        {
                            Code = BrandCodeValue
                        };
                        Product.BrandId = Brands.Where(x => x.Code.ToLower() == BrandCodeValue.Trim().ToLower()).Select(x => x.Id).FirstOrDefault();
                    }
                    if (!string.IsNullOrWhiteSpace(UoMGroupCodeValue))
                    {
                        Product.UnitOfMeasureGrouping = new UnitOfMeasureGrouping()
                        {
                            Code = UoMGroupCodeValue
                        };
                        Product.UnitOfMeasureGroupingId = UnitOfMeasureGroupings.Where(x => x.Code.Trim() == UoMGroupCodeValue.Trim().Trim()).Select(x => x.Id).FirstOrDefault();
                    }
                    Product.TaxType = new TaxType()
                    {
                        Code = TaxTypeCodeValue
                    };
                    Product.TaxTypeId = TaxTypes.Where(x => x.Code.Equals(TaxTypeCodeValue == null ? string.Empty : TaxTypeCodeValue.Trim())).Select(x => x.Id).FirstOrDefault();

                    //giá bán
                    if (long.TryParse(SalePriceValue, out long SalePrice))
                    {
                        Product.SalePrice = SalePrice;
                    }
                    if (string.IsNullOrEmpty(StatusNameValue))
                    {
                        Product.StatusId = -1;
                    }
                    else
                    {
                        Product.StatusId = Statuses.Where(x => x.Name.ToLower().Equals(StatusNameValue == null ? string.Empty : StatusNameValue.Trim().ToLower())).Select(x => x.Id).FirstOrDefault();
                    }
                    if (string.IsNullOrEmpty(UsedVariationCodeValue))
                    {
                        Product.UsedVariationId = -1;
                    }
                    else
                    {
                        Product.UsedVariationId = UsedVariations.Where(x => x.Name.ToLower().Equals(UsedVariationCodeValue == null ? string.Empty : UsedVariationCodeValue.Trim().ToLower())).Select(x => x.Id).FirstOrDefault();
                    }

                    if (Product.UsedVariationId == Enums.UsedVariationEnum.USED.Id)
                    {
                        #region Variation
                        Product.VariationGroupings = new List<VariationGrouping>();
                        if (!string.IsNullOrWhiteSpace(Property1Value))
                        {
                            VariationGrouping VariationGrouping = new VariationGrouping
                            {
                                Name = Property1Value
                            };
                            VariationGrouping.Variations = new List<Variation>();
                            if (!string.IsNullOrWhiteSpace(PropertyValue1Value))
                            {
                                var Values = PropertyValue1Value.Split(';');
                                foreach (var Value in Values)
                                {
                                    var splitValue = Value.Trim().Split('-');
                                    Variation Variation = new Variation
                                    {
                                        Code = splitValue[0].Trim(),
                                        Name = splitValue[1].Trim()
                                    };
                                    VariationGrouping.Variations.Add(Variation);
                                }
                            }
                            Product.VariationGroupings.Add(VariationGrouping);
                        }
                        else
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập thuộc tính 1");
                        }

                        if (!string.IsNullOrWhiteSpace(Property2Value))
                        {
                            VariationGrouping VariationGrouping = new VariationGrouping
                            {
                                Name = Property2Value
                            };
                            VariationGrouping.Variations = new List<Variation>();
                            if (!string.IsNullOrWhiteSpace(PropertyValue2Value))
                            {
                                var Values = PropertyValue2Value.Split(';');
                                foreach (var Value in Values)
                                {
                                    if (!string.IsNullOrEmpty(Value))
                                    {
                                        var splitValue = Value.Trim().Split('-');
                                        Variation Variation = new Variation
                                        {
                                            Code = splitValue[0].Trim(),
                                            Name = splitValue[1].Trim()
                                        };
                                        VariationGrouping.Variations.Add(Variation);
                                    }
                                }
                            }
                            Product.VariationGroupings.Add(VariationGrouping);
                        }
                        else
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập thuộc tính 2");
                        }

                        if (!string.IsNullOrWhiteSpace(Property3Value))
                        {
                            VariationGrouping VariationGrouping = new VariationGrouping
                            {
                                Name = Property3Value
                            };
                            VariationGrouping.Variations = new List<Variation>();
                            if (!string.IsNullOrWhiteSpace(PropertyValue3Value))
                            {
                                var Values = PropertyValue3Value.Split(';');
                                foreach (var Value in Values)
                                {
                                    var splitValue = Value.Trim().Split('-');
                                    Variation Variation = new Variation
                                    {
                                        Code = splitValue[0].Trim(),
                                        Name = splitValue[1].Trim()
                                    };
                                    VariationGrouping.Variations.Add(Variation);
                                }
                            }
                            Product.VariationGroupings.Add(VariationGrouping);
                        }
                        else
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập thuộc tính 3");
                        }

                        if (!string.IsNullOrWhiteSpace(Property4Value))
                        {
                            VariationGrouping VariationGrouping = new VariationGrouping
                            {
                                Name = Property3Value
                            };
                            VariationGrouping.Variations = new List<Variation>();
                            if (!string.IsNullOrWhiteSpace(PropertyValue3Value))
                            {
                                var Values = PropertyValue3Value.Split(';');
                                foreach (var Value in Values)
                                {
                                    var splitValue = Value.Trim().Split('-');
                                    Variation Variation = new Variation
                                    {
                                        Code = splitValue[0].Trim(),
                                        Name = splitValue[1].Trim()
                                    };
                                    VariationGrouping.Variations.Add(Variation);
                                }
                            }
                            Product.VariationGroupings.Add(VariationGrouping);
                        }
                        else
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i}: Chưa nhập thuộc tính 4");
                        }
                        #endregion
                    }
                    Products.Add(Product);
                }

                if (errorContent.Length > 0)
                    return BadRequest(errorContent.ToString());
                #endregion
            }
            #region Item
            foreach (var Product in Products)
            {
                if (Product.UsedVariationId == Enums.UsedVariationEnum.NOTUSED.Id)
                {
                    Product.Items = new List<Item>();
                    Product.Items.Add(new Item
                    {
                        Code = Product.Code,
                        Name = Product.Name,
                        ScanCode = Product.ScanCode,
                        RetailPrice = Product.RetailPrice,
                        SalePrice = Product.SalePrice,
                        ProductId = Product.Id,
                        StatusId = Product.StatusId
                    });
                }
                if (Product.UsedVariationId == Enums.UsedVariationEnum.USED.Id)
                {
                    Product.Items = new List<Item>();
                    var items1 = new List<Item>();
                    var items2 = new List<Item>();
                    var items3 = new List<Item>();
                    var items4 = new List<Item>();
                    //thuộc tính đầu tiên
                    if (Product.VariationGroupings != null && Product.VariationGroupings.Any())
                    {
                        var VariationGrouping1 = Product.VariationGroupings[0];
                        if (VariationGrouping1 != null)
                        {
                            foreach (var Variation in VariationGrouping1.Variations)
                            {
                                Item Item = new Item
                                {
                                    Code = $"{Product.Code}-{Variation.Code}",
                                    Name = $"{Product.Name} - {Variation.Name}",
                                    ScanCode = Product.ScanCode,
                                    RetailPrice = Product.RetailPrice,
                                    SalePrice = Product.SalePrice,
                                    StatusId = Product.StatusId
                                };
                                items1.Add(Item);
                            }
                        }
                        if (Product.VariationGroupings.Count > 1)
                        {
                            var VariationGrouping2 = Product.VariationGroupings[1];
                            if (VariationGrouping2 != null)
                            {
                                foreach (var Variation in VariationGrouping2.Variations)
                                {
                                    foreach (var item in items1)
                                    {
                                        var newObj = Utils.Clone(item);
                                        newObj.Code = newObj.Code + "-" + Variation.Code;
                                        newObj.Name = newObj.Name + " - " + Variation.Name;
                                        items2.Add(newObj);
                                    }
                                }
                            }
                        }

                        if (Product.VariationGroupings.Count > 2)
                        {
                            var VariationGrouping3 = Product.VariationGroupings[2];
                            if (VariationGrouping3 != null)
                            {
                                foreach (var Variation in VariationGrouping3.Variations)
                                {
                                    foreach (var item in items2)
                                    {
                                        var newObj = Utils.Clone(item);
                                        newObj.Code = newObj.Code + "-" + Variation.Code;
                                        newObj.Name = newObj.Name + " - " + Variation.Name;
                                        items3.Add(newObj);
                                    }
                                }
                            }
                        }

                        if (Product.VariationGroupings.Count > 3)
                        {
                            var VariationGrouping4 = Product.VariationGroupings[3];
                            if (VariationGrouping4 != null)
                            {
                                foreach (var Variation in VariationGrouping4.Variations)
                                {
                                    foreach (var item in items3)
                                    {
                                        var newObj = Utils.Clone(item);
                                        newObj.Code = newObj.Code + "-" + Variation.Code;
                                        newObj.Name = newObj.Name + " - " + Variation.Name;
                                        items4.Add(newObj);
                                    }
                                }
                            }
                        }
                    }
                    if (items4.Count > 0)
                        Product.Items = items4;
                    else if (items3.Count > 0)
                        Product.Items = items3;
                    else if (items2.Count > 0)
                        Product.Items = items2;
                    else
                        Product.Items = items1;
                }
            }
            #endregion
            Products.ForEach(x => x.BaseLanguage = CurrentContext.Language);
            Products = await ProductService.Import(Products);
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(c => new Product_ProductDTO(c)).ToList();

            //thông báo lỗi
            for (int i = 0; i < Products.Count; i++)
            {
                if (!Products[i].IsValidated)
                {
                    if (Products[i].Errors != null)
                    {
                        foreach (var Error in Products[i].Errors)
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i + 2}: {Error.Value}");
                        }
                    }
                    if (Products[i].Items != null)
                    {
                        foreach (var item in Products[i].Items)
                        {
                            if (item.Errors != null)
                            {
                                foreach (var Error in item.Errors)
                                {
                                    errorContent.AppendLine($"Lỗi dòng thứ {i + 2}: {Error.Value}");
                                }
                            }
                        }
                    }
                    if (Products[i].VariationGroupings != null)
                    {
                        foreach (var VariationGrouping in Products[i].VariationGroupings)
                        {
                            if (VariationGrouping.Errors != null)
                            {
                                foreach (var Error in VariationGrouping.Errors)
                                {
                                    errorContent.AppendLine($"Lỗi dòng thứ {i + 2}: {Error.Value}");
                                }
                            }
                            if (VariationGrouping.Variations != null)
                            {
                                foreach (var Variation in VariationGrouping.Variations)
                                {
                                    if (Variation.Errors != null)
                                    {
                                        foreach (var Error in Variation.Errors)
                                        {
                                            errorContent.AppendLine($"Lỗi dòng thứ {i + 2}: {Error.Value}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Products.Any(x => !x.IsValidated))
                return BadRequest(errorContent.ToString());
            return Product_ProductDTOs;
        }

        [Route(ProductRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            ProductFilter.Skip = 0;
            ProductFilter.Take = int.MaxValue;
            ProductFilter.Selects = ProductSelect.ALL;
            ProductFilter = ProductService.ToFilter(ProductFilter);

            List<Product> Products = await ProductService.List(ProductFilter);
            #region ELODIE
            List<Category> Categorys = await CategoryService.List(new CategoryFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = CategorySelect.ALL,
            });
            Categorys = Categorys.Where(x => x.HasChildren == false).ToList();
            List<Item> Items = await ItemService.List(new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.ALL,
                ProductId = new IdFilter { In = Products.Select(p => p.Id).ToList() }
            });
            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(new ProductGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductGroupingSelect.ALL,
            });
            List<ProductType> ProductTypes = await ProductTypeService.List(new ProductTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductTypeSelect.ALL,
            });
            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureSelect.ALL,
            });
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureGroupingSelect.ALL
            });
            List<Brand> Brands = await BrandService.List(new BrandFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = BrandSelect.ALL
            });
            List<TaxType> TaxTypes = await TaxTypeService.List(new TaxTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = TaxTypeSelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            #endregion
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-EN");
            MemoryStream MemoryStream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excel = new ExcelPackage(MemoryStream))
            {
                #region sheet product 
                var ProductHeader = new List<string[]>()
                {
                    new string[] { "STT",
                        "Mã sản phẩm",
                        "Tên sản phẩm",
                        "Danh mục sản phẩm",
                        "Nhóm sản phẩm",
                        "Loại sản phẩm",
                        "Đơn vị tính",
                        "Nhóm đơn vị chuyển đổi",
                        "Mã từ ERP",
                        "Mã nhận diện sản phẩm",
                        "Nhãn hiệu",
                        "Tên khác",
                        "Tên kỹ thuật",
                        "% VAT",
                        "Mô tả",
                        "Giá bán",
                        "Trạng thái",
                        "Có tạo phiên bản",
                        "Thuộc tính 1",
                        "Giá trị 1",
                        "Thuộc tính 2",
                        "Giá trị 2",
                        "Thuộc tính 3",
                        "Giá trị 3",
                        "Thuộc tính 4",
                        "Giá trị 4",
                    }
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < Products.Count; i++)
                {
                    Product Product = Products[i];
                    string ProductGroupingName = "";
                    if (Product.ProductProductGroupingMappings != null)
                    {
                        foreach (var ProductProductGroupingMapping in Product.ProductProductGroupingMappings)
                        {
                            ProductGroupingName = string.Join(';', Product.ProductProductGroupingMappings.Select(x => x.ProductGrouping.Name).ToList());
                        }
                    }
                    string VariationGrouping1 = "";
                    string VariationValue1 = "";
                    string VariationGrouping2 = "";
                    string VariationValue2 = "";
                    string VariationGrouping3 = "";
                    string VariationValue3 = "";
                    string VariationGrouping4 = "";
                    string VariationValue4 = "";

                    if (Product.VariationGroupings != null)
                    {
                        if (Product.VariationGroupings.Count > 0)
                        {
                            VariationGrouping1 += Product.VariationGroupings[0].Name;
                            VariationValue1 = string.Join(",", Product.VariationGroupings[0].Variations.Select(v => v.Code + '-' + v.Name).ToArray());
                        }

                        if (Product.VariationGroupings.Count > 1)
                        {
                            VariationGrouping2 += Product.VariationGroupings[1].Name;
                            VariationValue2 = string.Join(",", Product.VariationGroupings[1].Variations.Select(v => v.Code + '-' + v.Name).ToArray());
                        }

                        if (Product.VariationGroupings.Count > 2)
                        {
                            VariationGrouping3 += Product.VariationGroupings[2].Name;
                            VariationValue3 = string.Join(",", Product.VariationGroupings[2].Variations.Select(v => v.Code + '-' + v.Name).ToArray());
                        }

                        if (Product.VariationGroupings.Count > 3)
                        {
                            VariationGrouping4 += Product.VariationGroupings[3].Name;
                            VariationValue4 = string.Join(",", Product.VariationGroupings[3].Variations.Select(v => v.Code + '-' + v.Name).ToArray());
                        }
                    }
                    data.Add(new object[]
                    {
                        i+1,
                        Product.Code,
                        Product.Name,
                        Product.Category?.Name,
                        ProductGroupingName,
                        Product.ProductType?.Name,
                        Product.UnitOfMeasure?.Name,
                        Product.UnitOfMeasureGrouping?.Name,
                        Product.ERPCode,
                        Product.ScanCode,
                        Product.Brand?.Name,
                        Product.OtherName,
                        Product.TechnicalName,
                        Product.TaxType.Code,
                        Product.Description,
                        Product.SalePrice?.ToString("N0", culture),
                        Product.Status?.Name,
                        Product.UsedVariation?.Name,
                        VariationGrouping1,
                        VariationValue1,
                        VariationGrouping2,
                        VariationValue2,
                        VariationGrouping3,
                        VariationValue3,
                        VariationGrouping4,
                        VariationValue4,
                    });
                }
                excel.GenerateWorksheet("Sản phẩm", ProductHeader, data);
                #endregion

                #region sheet item  
                data.Clear();
                var ItemHeader = new List<string[]>()
                {
                    new string[] {
                        "STT",
                        "Mã sản phẩm",
                        "Mã sản phẩm thuộc tính",
                        "Tên sản phẩm thuộc tính",
                        "Mã nhận diện sản phẩm",
                        "Giá bán",
                        "Giá bán lẻ đề xuất",
                        "Trạng thái",
                    }
                };
                for (int i = 0; i < Items.Count; i++)
                {
                    Item Item = Items[i];
                    data.Add(new object[] {
                                i+1,
                                Item.Product?.Code,
                                Item.Code,
                                Item.Name,
                                Item.ScanCode,
                                Item.SalePrice?.ToString("N0", culture),
                                Item.RetailPrice?.ToString("N0", culture),
                                Item.Status?.Name,
                                });
                }
                excel.GenerateWorksheet("Phiên bản sản phẩm", ItemHeader, data);
                #endregion

                #region Sheet Product Type
                data.Clear();
                var CategoryHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã danh mục sản phẩm",
                        "Tên danh mục sản phẩm",
                    }
                };
                foreach (var Category in Categorys)
                {
                    data.Add(new object[]
                    {
                        Category.Code,
                        Category.Name
                    });
                }
                excel.GenerateWorksheet("Danh mục sản phẩm", CategoryHeader, data);
                #endregion

                #region Sheet Product Group
                data.Clear();
                var ProductGroupHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã nhóm sản phẩm",
                        "Tên nhóm sản phẩm",
                    }
                };
                foreach (var ProductGrouping in ProductGroupings)
                {
                    data.Add(new object[]
                    {
                        ProductGrouping.Code,
                        ProductGrouping.Name
                    });
                }
                excel.GenerateWorksheet("Nhóm sản phẩm", ProductGroupHeader, data);
                #endregion

                #region Sheet Product Type
                data.Clear();
                var ProductTypeHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã loại sản phẩm",
                        "Tên loại sản phẩm",
                    }
                };
                foreach (var ProductType in ProductTypes)
                {
                    data.Add(new object[]
                    {
                        ProductType.Code,
                        ProductType.Name
                    });
                }
                excel.GenerateWorksheet("Loại sản phẩm", ProductTypeHeader, data);
                #endregion

                #region Sheet UOM
                data.Clear();
                var UOMHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã đơn vị tính",
                        "Tên đơn vị tính",
                    }
                };
                foreach (var UnitOfMeasure in UnitOfMeasures)
                {
                    data.Add(new object[]
                    {
                        UnitOfMeasure.Code,
                        UnitOfMeasure.Name
                    });
                }
                excel.GenerateWorksheet("Đơn vị tính", UOMHeader, data);
                #endregion

                #region Sheet UOMGrouping
                data.Clear();
                var UOMGroupingHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã nhóm đơn vị chuyển đổi",
                        "Tên nhóm đơn vị chuyển đổi",
                        "Đơn vị cơ bản"
                    }
                };
                foreach (var UnitOfMeasureGrouping in UnitOfMeasureGroupings)
                {
                    data.Add(new object[]
                    {
                        UnitOfMeasureGrouping.Code,
                        UnitOfMeasureGrouping.Name,
                        UnitOfMeasureGrouping.UnitOfMeasure?.Name
                    });
                }
                excel.GenerateWorksheet("Nhóm đơn vị tính", UOMGroupingHeader, data);
                #endregion

                #region Sheet Brand
                data.Clear();
                var BrandHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã nhãn hiệu",
                        "Tên nhãn hiệu",
                    }
                };
                foreach (var Brand in Brands)
                {
                    data.Add(new object[]
                    {
                        Brand.Code,
                        Brand.Name
                    });
                }
                excel.GenerateWorksheet("Nhãn hiệu", BrandHeader, data);
                #endregion

                #region Sheet TaxType
                data.Clear();
                var TaxTypeHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã VAT",
                        "Tên VAT",
                    }
                };
                foreach (var TaxType in TaxTypes)
                {
                    data.Add(new object[]
                    {
                        TaxType.Code,
                        TaxType.Name
                    });
                }
                excel.GenerateWorksheet("Thuế", TaxTypeHeader, data);
                #endregion

                excel.Save();
            }

            return File(MemoryStream.ToArray(), "application/octet-stream", "Product" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
        }

        [Route(ProductRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate()
        {
            #region ELODIE
            List<Category> Categorys = await CategoryService.List(new CategoryFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = CategorySelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            Categorys = Categorys.Where(x => x.HasChildren == false).ToList();
            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(new ProductGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductGroupingSelect.ALL,
            });
            List<ProductType> ProductTypes = await ProductTypeService.List(new ProductTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductTypeSelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureSelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureGroupingSelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            List<Supplier> Suppliers = await SupplierService.List(new SupplierFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SupplierSelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            List<Brand> Brands = await BrandService.List(new BrandFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = BrandSelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            List<TaxType> TaxTypes = await TaxTypeService.List(new TaxTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = TaxTypeSelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            #endregion
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            MemoryStream MemoryStream = new MemoryStream();
            string tempPath = "Templates/Product_Export.xlsx";
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-EN");
            using (var xlPackage = new ExcelPackage(new FileInfo(tempPath)))
            {
                #region sheet ProductGrouping 
                var worksheet_ProductGroup = xlPackage.Workbook.Worksheets["Nhóm sản phẩm"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_ProductGroup = 2;
                int numberCell_ProductGroup = 1;
                for (var i = 0; i < ProductGroupings.Count; i++)
                {
                    ProductGrouping ProductGrouping = ProductGroupings[i];
                    worksheet_ProductGroup.Cells[startRow_ProductGroup + i, numberCell_ProductGroup].Value = ProductGrouping.Code;
                    worksheet_ProductGroup.Cells[startRow_ProductGroup + i, numberCell_ProductGroup + 1].Value = ProductGrouping.Name;
                }
                #endregion

                #region sheet Category
                var worksheet_Category = xlPackage.Workbook.Worksheets["Danh mục sản phẩm"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_Category = 2;
                int numberCell_Category = 1;
                for (var i = 0; i < Categorys.Count; i++)
                {
                    Category Category = Categorys[i];
                    worksheet_Category.Cells[startRow_Category + i, numberCell_Category].Value = Category.Code;
                    worksheet_Category.Cells[startRow_Category + i, numberCell_Category + 1].Value = Category.Name;
                }
                #endregion

                #region sheet ProductType
                var worksheet_ProductType = xlPackage.Workbook.Worksheets["Loại sản phẩm"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_ProductType = 2;
                int numberCell_ProductType = 1;
                for (var i = 0; i < ProductTypes.Count; i++)
                {
                    ProductType ProductType = ProductTypes[i];
                    worksheet_ProductType.Cells[startRow_ProductType + i, numberCell_ProductType].Value = ProductType.Code;
                    worksheet_ProductType.Cells[startRow_ProductType + i, numberCell_ProductType + 1].Value = ProductType.Name;
                }
                #endregion

                #region sheet UoM
                var worksheet_UoM = xlPackage.Workbook.Worksheets["Đơn vị tính"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_UoM = 2;
                int numberCell_UoM = 1;
                for (var i = 0; i < UnitOfMeasures.Count; i++)
                {
                    UnitOfMeasure UnitOfMeasure = UnitOfMeasures[i];
                    worksheet_UoM.Cells[startRow_UoM + i, numberCell_UoM].Value = UnitOfMeasure.Code;
                    worksheet_UoM.Cells[startRow_UoM + i, numberCell_UoM + 1].Value = UnitOfMeasure.Name;
                }
                #endregion

                #region sheet UoMGroup
                var worksheet_UoMGroup = xlPackage.Workbook.Worksheets["Nhóm đơn vị tính"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_UoMGroup = 2;
                int numberCell_UoMGroup = 1;
                for (var i = 0; i < UnitOfMeasureGroupings.Count; i++)
                {
                    UnitOfMeasureGrouping UnitOfMeasureGrouping = UnitOfMeasureGroupings[i];
                    worksheet_UoMGroup.Cells[startRow_UoMGroup + i, numberCell_UoMGroup].Value = UnitOfMeasureGrouping.Code;
                    worksheet_UoMGroup.Cells[startRow_UoMGroup + i, numberCell_UoMGroup + 1].Value = UnitOfMeasureGrouping.Name;
                    worksheet_UoMGroup.Cells[startRow_UoMGroup + i, numberCell_UoMGroup + 2].Value = UnitOfMeasureGrouping.UnitOfMeasure.Name;
                }
                #endregion

                #region sheet Brand
                var worksheet_Brand = xlPackage.Workbook.Worksheets["Nhãn hiệu"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_Brand = 2;
                int numberCell_Brand = 1;
                for (var i = 0; i < Brands.Count; i++)
                {
                    Brand Brand = Brands[i];
                    worksheet_Brand.Cells[startRow_Brand + i, numberCell_Brand].Value = Brand.Code;
                    worksheet_Brand.Cells[startRow_Brand + i, numberCell_Brand + 1].Value = Brand.Name;
                }
                #endregion

                #region sheet TaxType ( VAT )
                var worksheet_TaxType = xlPackage.Workbook.Worksheets["Thuế"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_TaxType = 2;
                int numberCell_TaxType = 1;
                for (var i = 0; i < TaxTypes.Count; i++)
                {
                    TaxType TaxType = TaxTypes[i];
                    worksheet_TaxType.Cells[startRow_TaxType + i, numberCell_TaxType].Value = TaxType.Code;
                    worksheet_TaxType.Cells[startRow_TaxType + i, numberCell_TaxType + 1].Value = TaxType.Name;
                    worksheet_TaxType.Cells[startRow_TaxType + i, numberCell_TaxType + 2].Value = TaxType.Percentage.ToString("N03", culture);
                }
                #endregion

                xlPackage.SaveAs(MemoryStream);
            }

            return File(MemoryStream.ToArray(), "application/octet-stream", "Template_Product.xlsx");
        }

        [Route(ProductRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Id = new IdFilter { In = Ids };
            ProductFilter.Selects = ProductSelect.Id;
            ProductFilter.Skip = 0;
            ProductFilter.Take = int.MaxValue;

            List<Product> Products = await ProductService.List(ProductFilter);
            Products = await ProductService.BulkDelete(Products);
            if (Products.Any(x => !x.IsValidated))
                return BadRequest(Products.Where(x => !x.IsValidated));
            return true;
        }

        [Route(ProductRoute.SaveImage), HttpPost]
        public async Task<ActionResult<Product_ImageDTO>> SaveImage(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            Image Image = new Image
            {
                Name = file.FileName,
                Content = memoryStream.ToArray(),
            };
            Image = await ProductService.SaveImage(Image);
            if (Image == null)
                return BadRequest();
            Product_ImageDTO product_ImageDTO = new Product_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
                ThumbnailUrl = Image.ThumbnailUrl,
            };
            return Ok(product_ImageDTO);
        }

        [Route(ProductRoute.SaveItemImage), HttpPost]
        public async Task<ActionResult<Product_ImageDTO>> SaveItemImage(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            Image Image = new Image
            {
                Name = file.FileName,
                Content = memoryStream.ToArray()
            };
            Image = await ItemService.SaveImage(Image);
            if (Image == null)
                return BadRequest();
            Product_ImageDTO product_ImageDTO = new Product_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
                ThumbnailUrl = Image.ThumbnailUrl,
            };
            return Ok(product_ImageDTO);
        }

        private async Task<bool> HasPermission(long Id)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter = ProductService.ToFilter(ProductFilter);
            if (Id == 0)
            {

            }
            else
            {
                ProductFilter.Id = new IdFilter { Equal = Id };
                int count = await ProductService.Count(ProductFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Product ConvertDTOToEntity(Product_ProductDTO Product_ProductDTO)
        {
            Product Product = new Product();
            Product.Id = Product_ProductDTO.Id;
            Product.Code = Product_ProductDTO.Code;
            Product.Name = Product_ProductDTO.Name;
            Product.ERPCode = Product_ProductDTO.ERPCode;
            Product.Description = Product_ProductDTO.Description;
            Product.ScanCode = Product_ProductDTO.ScanCode;
            Product.CategoryId = Product_ProductDTO.CategoryId;
            Product.ProductTypeId = Product_ProductDTO.ProductTypeId;
            Product.BrandId = Product_ProductDTO.BrandId;
            Product.CodeGeneratorRuleId = Product_ProductDTO.CodeGeneratorRuleId;
            Product.UnitOfMeasureId = Product_ProductDTO.UnitOfMeasureId;
            Product.UnitOfMeasureGroupingId = Product_ProductDTO.UnitOfMeasureGroupingId;
            Product.SalePrice = Product_ProductDTO.SalePrice;
            Product.RetailPrice = Product_ProductDTO.RetailPrice;
            Product.TaxTypeId = Product_ProductDTO.TaxTypeId;
            Product.StatusId = Product_ProductDTO.StatusId;
            Product.OtherName = Product_ProductDTO.OtherName;
            Product.TechnicalName = Product_ProductDTO.TechnicalName;
            Product.Note = Product_ProductDTO.Note;
            Product.IsPurchasable = Product_ProductDTO.IsPurchasable;
            Product.IsSellable = Product_ProductDTO.IsSellable;
            Product.UsedVariationId = Product_ProductDTO.UsedVariationId;
            Product.Brand = Product_ProductDTO.Brand == null ? null : new Brand
            {
                Id = Product_ProductDTO.Brand.Id,
                Code = Product_ProductDTO.Brand.Code,
                Name = Product_ProductDTO.Brand.Name,
                Description = Product_ProductDTO.Brand.Description,
                StatusId = Product_ProductDTO.Brand.StatusId,
            };
            Product.CodeGeneratorRule = Product_ProductDTO.CodeGeneratorRule == null ? null : new CodeGeneratorRule
            {
                Id = Product_ProductDTO.CodeGeneratorRule.Id,
                EntityTypeId = Product_ProductDTO.CodeGeneratorRule.EntityTypeId,
                StatusId = Product_ProductDTO.CodeGeneratorRule.StatusId,
            };
            Product.Category = Product_ProductDTO.Category == null ? null : new Category
            {
                Id = Product_ProductDTO.Category.Id,
                Code = Product_ProductDTO.Category.Code,
                Name = Product_ProductDTO.Category.Name,
                StatusId = Product_ProductDTO.Category.StatusId,
            };
            Product.ProductType = Product_ProductDTO.ProductType == null ? null : new ProductType
            {
                Id = Product_ProductDTO.ProductType.Id,
                Code = Product_ProductDTO.ProductType.Code,
                Name = Product_ProductDTO.ProductType.Name,
                Description = Product_ProductDTO.ProductType.Description,
                StatusId = Product_ProductDTO.ProductType.StatusId,
            };
            Product.Status = Product_ProductDTO.Status == null ? null : new Status
            {
                Id = Product_ProductDTO.Status.Id,
                Code = Product_ProductDTO.Status.Code,
                Name = Product_ProductDTO.Status.Name,
            };
            Product.TaxType = Product_ProductDTO.TaxType == null ? null : new TaxType
            {
                Id = Product_ProductDTO.TaxType.Id,
                Code = Product_ProductDTO.TaxType.Code,
                Name = Product_ProductDTO.TaxType.Name,
                Percentage = Product_ProductDTO.TaxType.Percentage,
                StatusId = Product_ProductDTO.TaxType.StatusId,
            };
            Product.UnitOfMeasure = Product_ProductDTO.UnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = Product_ProductDTO.UnitOfMeasure.Id,
                Code = Product_ProductDTO.UnitOfMeasure.Code,
                Name = Product_ProductDTO.UnitOfMeasure.Name,
                Description = Product_ProductDTO.UnitOfMeasure.Description,
                StatusId = Product_ProductDTO.UnitOfMeasure.StatusId,
            };
            Product.UnitOfMeasureGrouping = Product_ProductDTO.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
            {
                Id = Product_ProductDTO.UnitOfMeasureGrouping.Id,
                Name = Product_ProductDTO.UnitOfMeasureGrouping.Name,
                UnitOfMeasureId = Product_ProductDTO.UnitOfMeasureGrouping.UnitOfMeasureId,
                StatusId = Product_ProductDTO.UnitOfMeasureGrouping.StatusId,
            };
            Product.UsedVariation = Product_ProductDTO.UsedVariation == null ? null : new UsedVariation
            {
                Id = Product_ProductDTO.UsedVariation.Id,
                Code = Product_ProductDTO.UsedVariation.Code,
                Name = Product_ProductDTO.UsedVariation.Name,
            };
            Product.Items = Product_ProductDTO.Items?
                .Select(x => new Item
                {
                    Id = x.Id,
                    Code = x.Code,
                    ERPCode = x.ERPCode,
                    Name = x.Name,
                    ScanCode = x.ScanCode,
                    SalePrice = x.SalePrice,
                    RetailPrice = x.RetailPrice,
                    StatusId = x.StatusId,
                    Used = x.Used,
                    ItemImageMappings = x.ItemImageMappings?.Select(x => new ItemImageMapping
                    {
                        ItemId = x.ItemId,
                        ImageId = x.ImageId,
                    }).ToList()
                }).ToList();
            Product.ProductImageMappings = Product_ProductDTO.ProductImageMappings?
                .Select(x => new ProductImageMapping
                {
                    ImageId = x.ImageId,
                    Image = new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                        ThumbnailUrl = x.Image.ThumbnailUrl,
                    },
                }).ToList();
            Product.ProductProductGroupingMappings = Product_ProductDTO.ProductProductGroupingMappings?
                .Select(x => new ProductProductGroupingMapping
                {
                    ProductGroupingId = x.ProductGroupingId,
                    ProductGrouping = x.ProductGrouping == null ? null : new ProductGrouping
                    {
                        Id = x.ProductGrouping.Id,
                        Code = x.ProductGrouping.Code,
                        Name = x.ProductGrouping.Name,
                        ParentId = x.ProductGrouping.ParentId,
                        Path = x.ProductGrouping.Path,
                        Description = x.ProductGrouping.Description,
                    },
                }).ToList();
            Product.VariationGroupings = Product_ProductDTO.VariationGroupings?
                .Select(x => new VariationGrouping
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Variations = x.Variations.Select(v => new Variation
                    {
                        Id = v.Id,
                        Code = v.Code,
                        Name = v.Name,
                        VariationGroupingId = x.Id
                    }).ToList()
                }).ToList();
            Product.BaseLanguage = CurrentContext.Language;
            return Product;
        }

        private ProductFilter ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Selects = ProductSelect.Code | ProductSelect.Name
                | ProductSelect.Id | ProductSelect.ProductProductGroupingMapping | ProductSelect.ProductType
                | ProductSelect.Status | ProductSelect.UsedVariation | ProductSelect.Category | ProductSelect.Brand;
            ProductFilter.Skip = Product_ProductFilterDTO.Skip;
            ProductFilter.Take = Product_ProductFilterDTO.Take;
            ProductFilter.OrderBy = Product_ProductFilterDTO.OrderBy;
            ProductFilter.OrderType = Product_ProductFilterDTO.OrderType;

            ProductFilter.Id = Product_ProductFilterDTO.Id;
            ProductFilter.Code = Product_ProductFilterDTO.Code;
            ProductFilter.Name = Product_ProductFilterDTO.Name;
            ProductFilter.Description = Product_ProductFilterDTO.Description;
            ProductFilter.ScanCode = Product_ProductFilterDTO.ScanCode;
            ProductFilter.ProductTypeId = Product_ProductFilterDTO.ProductTypeId;
            ProductFilter.BrandId = Product_ProductFilterDTO.BrandId;
            ProductFilter.UnitOfMeasureId = Product_ProductFilterDTO.UnitOfMeasureId;
            ProductFilter.UnitOfMeasureGroupingId = Product_ProductFilterDTO.UnitOfMeasureGroupingId;
            ProductFilter.SalePrice = Product_ProductFilterDTO.SalePrice;
            ProductFilter.RetailPrice = Product_ProductFilterDTO.RetailPrice;
            ProductFilter.TaxTypeId = Product_ProductFilterDTO.TaxTypeId;
            ProductFilter.CategoryId = Product_ProductFilterDTO.CategoryId;
            ProductFilter.StatusId = Product_ProductFilterDTO.StatusId;
            ProductFilter.OtherName = Product_ProductFilterDTO.OtherName;
            ProductFilter.TechnicalName = Product_ProductFilterDTO.TechnicalName;
            ProductFilter.Note = Product_ProductFilterDTO.Note;
            ProductFilter.IsPurchasable = Product_ProductFilterDTO.IsPurchaseable;
            ProductFilter.IsSellable = Product_ProductFilterDTO.IsSellable;
            ProductFilter.Search = Product_ProductFilterDTO.Search;
            ProductFilter.ProductGroupingId = Product_ProductFilterDTO.ProductGroupingId;
            ProductFilter.UsedVariationId = Product_ProductFilterDTO.UsedVariationId;
            return ProductFilter;
        }

        [Route(ProductRoute.CountProductGrouping), HttpPost]
        public async Task<long> CountProductGrouping([FromBody] Product_ProductGroupingFilterDTO Product_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Id = Product_ProductGroupingFilterDTO.Id;
            ProductGroupingFilter.Code = Product_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = Product_ProductGroupingFilterDTO.Name;
            ProductGroupingFilter.ParentId = Product_ProductGroupingFilterDTO.ParentId;
            ProductGroupingFilter.Path = Product_ProductGroupingFilterDTO.Path;
            ProductGroupingFilter.Description = Product_ProductGroupingFilterDTO.Description;
            return await ProductGroupingService.Count(ProductGroupingFilter);
        }

        [Route(ProductRoute.ListProductGrouping), HttpPost]
        public async Task<List<Product_ProductGroupingDTO>> ListProductGrouping([FromBody] Product_ProductGroupingFilterDTO Product_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = Product_ProductGroupingFilterDTO.Skip;
            ProductGroupingFilter.Take = Product_ProductGroupingFilterDTO.Take;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.ALL;
            ProductGroupingFilter.Id = Product_ProductGroupingFilterDTO.Id;
            ProductGroupingFilter.Code = Product_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = Product_ProductGroupingFilterDTO.Name;
            ProductGroupingFilter.ParentId = Product_ProductGroupingFilterDTO.ParentId;
            ProductGroupingFilter.Path = Product_ProductGroupingFilterDTO.Path;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<Product_ProductGroupingDTO> Product_ProductGroupingDTOs = ProductGroupings
                .Select(x => new Product_ProductGroupingDTO(x)).ToList();
            return Product_ProductGroupingDTOs;
        }

        [Route(ProductRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] Product_ItemFilterDTO Product_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = Product_ItemFilterDTO.Id;
            ItemFilter.Code = Product_ItemFilterDTO.Code;
            ItemFilter.Name = Product_ItemFilterDTO.Name;
            ItemFilter.ProductId = Product_ItemFilterDTO.ProductId;
            ItemFilter.SalePrice = Product_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = Product_ItemFilterDTO.ScanCode;
            ItemFilter.RetailPrice = Product_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            return await ItemService.Count(ItemFilter);
        }

        [Route(ProductRoute.ListItem), HttpPost]
        public async Task<List<Product_ItemDTO>> ListItem([FromBody] Product_ItemFilterDTO Product_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = Product_ItemFilterDTO.Skip;
            ItemFilter.Take = Product_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = Product_ItemFilterDTO.Id;
            ItemFilter.Code = Product_ItemFilterDTO.Code;
            ItemFilter.ERPCode = Product_ItemFilterDTO.ERPCode;
            ItemFilter.Name = Product_ItemFilterDTO.Name;
            ItemFilter.ProductId = Product_ItemFilterDTO.ProductId;
            ItemFilter.SalePrice = Product_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = Product_ItemFilterDTO.ScanCode;
            ItemFilter.RetailPrice = Product_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Item> Items = await ItemService.List(ItemFilter);
            List<Product_ItemDTO> Product_ItemDTOs = Items
                .Select(x => new Product_ItemDTO(x)).ToList();
            return Product_ItemDTOs;
        }


        [Route(ProductRoute.CountItemHistory), HttpPost]
        public async Task<long> CountItemHistory([FromBody] Product_ItemHistoryFilterDTO Product_ItemHistoryFilterDTO)
        {
            ItemHistoryFilter ItemHistoryFilter = new ItemHistoryFilter();
            ItemHistoryFilter.ItemId = Product_ItemHistoryFilterDTO.ItemId;
            ItemHistoryFilter.Time = Product_ItemHistoryFilterDTO.Time;
            return await ItemHistoryService.Count(ItemHistoryFilter);
        }
        [Route(ProductRoute.ListItemHistory), HttpPost]
        public async Task<List<Product_ItemHistoryDTO>> ListItemHistory([FromBody] Product_ItemHistoryFilterDTO Product_ItemHistoryFilterDTO)
        {
            ItemHistoryFilter ItemHistoryFilter = new ItemHistoryFilter();
            ItemHistoryFilter.Skip = Product_ItemHistoryFilterDTO.Skip;
            ItemHistoryFilter.Take = Product_ItemHistoryFilterDTO.Take;
            ItemHistoryFilter.OrderBy = ItemHistoryOrder.Time;
            ItemHistoryFilter.OrderType = OrderType.DESC;
            ItemHistoryFilter.Selects = ItemHistorySelect.ALL;
            ItemHistoryFilter.ItemId = Product_ItemHistoryFilterDTO.ItemId;
            ItemHistoryFilter.Time = Product_ItemHistoryFilterDTO.Time;

            List<ItemHistory> ItemHistories = await ItemHistoryService.List(ItemHistoryFilter);
            List<Product_ItemHistoryDTO> Product_ItemHistoryDTOs = ItemHistories
                .Select(x => new Product_ItemHistoryDTO(x)).ToList();
            return Product_ItemHistoryDTOs;
        }

        [Route(ProductRoute.GetProductHistory), HttpPost]
        public async Task<ActionResult<Product_ProductHistoryDTO>> GetProductHistory([FromBody] Product_ProductHistoryDTO Product_ProductHistoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);


            RequestHistory<Product> ProductHistory = await ProductService.GetProductHistoryDetail(Product_ProductHistoryDTO.Id);
            Product_ProductHistoryDTO = new Product_ProductHistoryDTO(ProductHistory);
            if (ProductHistory.IsValidated)
                return Product_ProductHistoryDTO;
            else
                return BadRequest(Product_ProductHistoryDTO);
        }

    }
}

