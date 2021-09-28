using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELODIE.Common;
using ELODIE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using ELODIE.Entities;
using ELODIE.Services.MCategory;
using ELODIE.Services.MImage;
using ELODIE.Services.MStatus;

namespace ELODIE.Rpc.category
{
    public partial class CategoryController : RpcController
    {
        private IImageService ImageService;
        private IStatusService StatusService;
        private ICategoryService CategoryService;
        private ICurrentContext CurrentContext;
        public CategoryController(
            IImageService ImageService,
            IStatusService StatusService,
            ICategoryService CategoryService,
            ICurrentContext CurrentContext
        )
        {
            this.ImageService = ImageService;
            this.StatusService = StatusService;
            this.CategoryService = CategoryService;
            this.CurrentContext = CurrentContext;
        }

        [Route(CategoryRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Category_CategoryFilterDTO Category_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CategoryFilter CategoryFilter = ConvertFilterDTOToFilterEntity(Category_CategoryFilterDTO);
            CategoryFilter = await CategoryService.ToFilter(CategoryFilter);
            int count = await CategoryService.Count(CategoryFilter);
            return count;
        }

        [Route(CategoryRoute.List), HttpPost]
        public async Task<ActionResult<List<Category_CategoryDTO>>> List([FromBody] Category_CategoryFilterDTO Category_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CategoryFilter CategoryFilter = ConvertFilterDTOToFilterEntity(Category_CategoryFilterDTO);
            CategoryFilter = await CategoryService.ToFilter(CategoryFilter);
            List<Category> Categories = await CategoryService.List(CategoryFilter);
            List<Category_CategoryDTO> Category_CategoryDTOs = Categories
                .Select(c => new Category_CategoryDTO(c)).ToList();
            return Category_CategoryDTOs;
        }

        [Route(CategoryRoute.Get), HttpPost]
        public async Task<ActionResult<Category_CategoryDTO>> Get([FromBody]Category_CategoryDTO Category_CategoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Category_CategoryDTO.Id))
                return Forbid();

            Category Category = await CategoryService.Get(Category_CategoryDTO.Id);
            return new Category_CategoryDTO(Category);
        }

        [Route(CategoryRoute.Create), HttpPost]
        public async Task<ActionResult<Category_CategoryDTO>> Create([FromBody] Category_CategoryDTO Category_CategoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Category_CategoryDTO.Id))
                return Forbid();

            Category Category = ConvertDTOToEntity(Category_CategoryDTO);
            Category = await CategoryService.Create(Category);
            Category_CategoryDTO = new Category_CategoryDTO(Category);
            if (Category.IsValidated)
                return Category_CategoryDTO;
            else
                return BadRequest(Category_CategoryDTO);
        }

        [Route(CategoryRoute.Update), HttpPost]
        public async Task<ActionResult<Category_CategoryDTO>> Update([FromBody] Category_CategoryDTO Category_CategoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Category_CategoryDTO.Id))
                return Forbid();

            Category Category = ConvertDTOToEntity(Category_CategoryDTO);
            Category = await CategoryService.Update(Category);
            Category_CategoryDTO = new Category_CategoryDTO(Category);
            if (Category.IsValidated)
                return Category_CategoryDTO;
            else
                return BadRequest(Category_CategoryDTO);
        }

        [Route(CategoryRoute.Delete), HttpPost]
        public async Task<ActionResult<Category_CategoryDTO>> Delete([FromBody] Category_CategoryDTO Category_CategoryDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Category_CategoryDTO.Id))
                return Forbid();

            Category Category = ConvertDTOToEntity(Category_CategoryDTO);
            Category = await CategoryService.Delete(Category);
            Category_CategoryDTO = new Category_CategoryDTO(Category);
            if (Category.IsValidated)
                return Category_CategoryDTO;
            else
                return BadRequest(Category_CategoryDTO);
        }
        
        [Route(CategoryRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter = await CategoryService.ToFilter(CategoryFilter);
            CategoryFilter.Id = new IdFilter { In = Ids };
            CategoryFilter.Selects = CategorySelect.Id;
            CategoryFilter.Skip = 0;
            CategoryFilter.Take = int.MaxValue;

            List<Category> Categories = await CategoryService.List(CategoryFilter);
            Categories = await CategoryService.BulkDelete(Categories);
            if (Categories.Any(x => !x.IsValidated))
                return BadRequest(Categories.Where(x => !x.IsValidated));
            return true;
        }

        [Route(CategoryRoute.SaveImage), HttpPost]
        public async Task<ActionResult<Category_ImageDTO>> SaveImage(IFormFile file)
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
            Image = await CategoryService.SaveImage(Image);
            if (Image == null)
                return BadRequest();
            Category_ImageDTO product_ImageDTO = new Category_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
                ThumbnailUrl = Image.ThumbnailUrl,
            };
            return Ok(product_ImageDTO);
        }

        [Route(CategoryRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            ImageFilter ImageFilter = new ImageFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ImageSelect.ALL
            };
            List<Image> Images = await ImageService.List(ImageFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Category> Categories = new List<Category>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Categories);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int ParentIdColumn = 3 + StartColumn;
                int PathColumn = 4 + StartColumn;
                int LevelColumn = 5 + StartColumn;
                int StatusIdColumn = 6 + StartColumn;
                int ImageIdColumn = 7 + StartColumn;
                int RowIdColumn = 11 + StartColumn;
                int UsedColumn = 12 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string ParentIdValue = worksheet.Cells[i + StartRow, ParentIdColumn].Value?.ToString();
                    string PathValue = worksheet.Cells[i + StartRow, PathColumn].Value?.ToString();
                    string LevelValue = worksheet.Cells[i + StartRow, LevelColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string ImageIdValue = worksheet.Cells[i + StartRow, ImageIdColumn].Value?.ToString();
                    string RowIdValue = worksheet.Cells[i + StartRow, RowIdColumn].Value?.ToString();
                    string UsedValue = worksheet.Cells[i + StartRow, UsedColumn].Value?.ToString();
                    
                    Category Category = new Category();
                    Category.Code = CodeValue;
                    Category.Name = NameValue;
                    Category.Path = PathValue;
                    Category.Level = long.TryParse(LevelValue, out long Level) ? Level : 0;
                    Image Image = Images.Where(x => x.Id.ToString() == ImageIdValue).FirstOrDefault();
                    Category.ImageId = Image == null ? 0 : Image.Id;
                    Category.Image = Image;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    Category.StatusId = Status == null ? 0 : Status.Id;
                    Category.Status = Status;
                    
                    Categories.Add(Category);
                }
            }
            Categories = await CategoryService.Import(Categories);
            if (Categories.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Categories.Count; i++)
                {
                    Category Category = Categories[i];
                    if (!Category.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Category.Errors.ContainsKey(nameof(Category.Id)))
                            Error += Category.Errors[nameof(Category.Id)];
                        if (Category.Errors.ContainsKey(nameof(Category.Code)))
                            Error += Category.Errors[nameof(Category.Code)];
                        if (Category.Errors.ContainsKey(nameof(Category.Name)))
                            Error += Category.Errors[nameof(Category.Name)];
                        if (Category.Errors.ContainsKey(nameof(Category.ParentId)))
                            Error += Category.Errors[nameof(Category.ParentId)];
                        if (Category.Errors.ContainsKey(nameof(Category.Path)))
                            Error += Category.Errors[nameof(Category.Path)];
                        if (Category.Errors.ContainsKey(nameof(Category.Level)))
                            Error += Category.Errors[nameof(Category.Level)];
                        if (Category.Errors.ContainsKey(nameof(Category.StatusId)))
                            Error += Category.Errors[nameof(Category.StatusId)];
                        if (Category.Errors.ContainsKey(nameof(Category.ImageId)))
                            Error += Category.Errors[nameof(Category.ImageId)];
                        if (Category.Errors.ContainsKey(nameof(Category.RowId)))
                            Error += Category.Errors[nameof(Category.RowId)];
                        if (Category.Errors.ContainsKey(nameof(Category.Used)))
                            Error += Category.Errors[nameof(Category.Used)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(CategoryRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] Category_CategoryFilterDTO Category_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Category
                var CategoryFilter = ConvertFilterDTOToFilterEntity(Category_CategoryFilterDTO);
                CategoryFilter.Skip = 0;
                CategoryFilter.Take = int.MaxValue;
                CategoryFilter = await CategoryService.ToFilter(CategoryFilter);
                List<Category> Categories = await CategoryService.List(CategoryFilter);

                var CategoryHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                        "ImageId",
                        "RowId",
                        "Used",
                    }
                };
                List<object[]> CategoryData = new List<object[]>();
                for (int i = 0; i < Categories.Count; i++)
                {
                    var Category = Categories[i];
                    CategoryData.Add(new Object[]
                    {
                        Category.Id,
                        Category.Code,
                        Category.Name,
                        Category.ParentId,
                        Category.Path,
                        Category.Level,
                        Category.StatusId,
                        Category.ImageId,
                        Category.RowId,
                        Category.Used,
                    });
                }
                excel.GenerateWorksheet("Category", CategoryHeaders, CategoryData);
                #endregion
                
                #region Image
                var ImageFilter = new ImageFilter();
                ImageFilter.Selects = ImageSelect.ALL;
                ImageFilter.OrderBy = ImageOrder.Id;
                ImageFilter.OrderType = OrderType.ASC;
                ImageFilter.Skip = 0;
                ImageFilter.Take = int.MaxValue;
                List<Image> Images = await ImageService.List(ImageFilter);

                var ImageHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Name",
                        "Url",
                        "ThumbnailUrl",
                        "RowId",
                    }
                };
                List<object[]> ImageData = new List<object[]>();
                for (int i = 0; i < Images.Count; i++)
                {
                    var Image = Images[i];
                    ImageData.Add(new Object[]
                    {
                        Image.Id,
                        Image.Name,
                        Image.Url,
                        Image.ThumbnailUrl,
                    });
                }
                excel.GenerateWorksheet("Image", ImageHeaders, ImageData);
                #endregion
                #region Status
                var StatusFilter = new StatusFilter();
                StatusFilter.Selects = StatusSelect.ALL;
                StatusFilter.OrderBy = StatusOrder.Id;
                StatusFilter.OrderType = OrderType.ASC;
                StatusFilter.Skip = 0;
                StatusFilter.Take = int.MaxValue;
                List<Status> Statuses = await StatusService.List(StatusFilter);

                var StatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StatusData = new List<object[]>();
                for (int i = 0; i < Statuses.Count; i++)
                {
                    var Status = Statuses[i];
                    StatusData.Add(new Object[]
                    {
                        Status.Id,
                        Status.Code,
                        Status.Name,
                    });
                }
                excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Category.xlsx");
        }

        [Route(CategoryRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] Category_CategoryFilterDTO Category_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Category
                var CategoryHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                        "ImageId",
                        "RowId",
                        "Used",
                    }
                };
                List<object[]> CategoryData = new List<object[]>();
                excel.GenerateWorksheet("Category", CategoryHeaders, CategoryData);
                #endregion
                
                #region Image
                var ImageFilter = new ImageFilter();
                ImageFilter.Selects = ImageSelect.ALL;
                ImageFilter.OrderBy = ImageOrder.Id;
                ImageFilter.OrderType = OrderType.ASC;
                ImageFilter.Skip = 0;
                ImageFilter.Take = int.MaxValue;
                List<Image> Images = await ImageService.List(ImageFilter);

                var ImageHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Name",
                        "Url",
                        "ThumbnailUrl",
                        "RowId",
                    }
                };
                List<object[]> ImageData = new List<object[]>();
                for (int i = 0; i < Images.Count; i++)
                {
                    var Image = Images[i];
                    ImageData.Add(new Object[]
                    {
                        Image.Id,
                        Image.Name,
                        Image.Url,
                        Image.ThumbnailUrl,
                    });
                }
                excel.GenerateWorksheet("Image", ImageHeaders, ImageData);
                #endregion
                #region Status
                var StatusFilter = new StatusFilter();
                StatusFilter.Selects = StatusSelect.ALL;
                StatusFilter.OrderBy = StatusOrder.Id;
                StatusFilter.OrderType = OrderType.ASC;
                StatusFilter.Skip = 0;
                StatusFilter.Take = int.MaxValue;
                List<Status> Statuses = await StatusService.List(StatusFilter);

                var StatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StatusData = new List<object[]>();
                for (int i = 0; i < Statuses.Count; i++)
                {
                    var Status = Statuses[i];
                    StatusData.Add(new Object[]
                    {
                        Status.Id,
                        Status.Code,
                        Status.Name,
                    });
                }
                excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Category.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter = await CategoryService.ToFilter(CategoryFilter);
            if (Id == 0)
            {

            }
            else
            {
                CategoryFilter.Id = new IdFilter { Equal = Id };
                int count = await CategoryService.Count(CategoryFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Category ConvertDTOToEntity(Category_CategoryDTO Category_CategoryDTO)
        {
            Category Category = new Category();
            Category.Id = Category_CategoryDTO.Id;
            Category.Code = Category_CategoryDTO.Code;
            Category.Name = Category_CategoryDTO.Name;
            Category.Prefix = Category_CategoryDTO.Prefix;
            Category.Description = Category_CategoryDTO.Description;
            Category.ParentId = Category_CategoryDTO.ParentId;
            Category.Path = Category_CategoryDTO.Path;
            Category.Level = Category_CategoryDTO.Level;
            Category.HasChildren = Category_CategoryDTO.HasChildren;
            Category.StatusId = Category_CategoryDTO.StatusId;
            Category.ImageId = Category_CategoryDTO.ImageId;
            Category.RowId = Category_CategoryDTO.RowId;
            Category.Used = Category_CategoryDTO.Used;
            Category.Image = Category_CategoryDTO.Image == null ? null : new Image
            {
                Id = Category_CategoryDTO.Image.Id,
                Name = Category_CategoryDTO.Image.Name,
                Url = Category_CategoryDTO.Image.Url,
                ThumbnailUrl = Category_CategoryDTO.Image.ThumbnailUrl,
            };
            Category.Parent = Category_CategoryDTO.Parent == null ? null : new Category
            {
                Id = Category_CategoryDTO.Parent.Id,
                Code = Category_CategoryDTO.Parent.Code,
                Name = Category_CategoryDTO.Parent.Name,
                Prefix = Category_CategoryDTO.Parent.Prefix,
                Description = Category_CategoryDTO.Parent.Description,
                ParentId = Category_CategoryDTO.Parent.ParentId,
                Path = Category_CategoryDTO.Parent.Path,
                Level = Category_CategoryDTO.Parent.Level,
                HasChildren = Category_CategoryDTO.Parent.HasChildren,
                StatusId = Category_CategoryDTO.Parent.StatusId,
                ImageId = Category_CategoryDTO.Parent.ImageId,
                RowId = Category_CategoryDTO.Parent.RowId,
                Used = Category_CategoryDTO.Parent.Used,
            };
            Category.Status = Category_CategoryDTO.Status == null ? null : new Status
            {
                Id = Category_CategoryDTO.Status.Id,
                Code = Category_CategoryDTO.Status.Code,
                Name = Category_CategoryDTO.Status.Name,
            };
            Category.BaseLanguage = CurrentContext.Language;
            return Category;
        }

        private CategoryFilter ConvertFilterDTOToFilterEntity(Category_CategoryFilterDTO Category_CategoryFilterDTO)
        {
            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter.Selects = CategorySelect.ALL;
            CategoryFilter.Skip = 0;
            CategoryFilter.Take = 99999;
            CategoryFilter.OrderBy = Category_CategoryFilterDTO.OrderBy;
            CategoryFilter.OrderType = Category_CategoryFilterDTO.OrderType;

            CategoryFilter.Id = Category_CategoryFilterDTO.Id;
            CategoryFilter.Code = Category_CategoryFilterDTO.Code;
            CategoryFilter.Name = Category_CategoryFilterDTO.Name;
            CategoryFilter.Prefix = Category_CategoryFilterDTO.Prefix;
            CategoryFilter.Description = Category_CategoryFilterDTO.Description;
            CategoryFilter.ParentId = Category_CategoryFilterDTO.ParentId;
            CategoryFilter.Path = Category_CategoryFilterDTO.Path;
            CategoryFilter.Level = Category_CategoryFilterDTO.Level;
            CategoryFilter.StatusId = Category_CategoryFilterDTO.StatusId;
            CategoryFilter.ImageId = Category_CategoryFilterDTO.ImageId;
            CategoryFilter.RowId = Category_CategoryFilterDTO.RowId;
            CategoryFilter.CreatedAt = Category_CategoryFilterDTO.CreatedAt;
            CategoryFilter.UpdatedAt = Category_CategoryFilterDTO.UpdatedAt;
            
            CategoryFilter.Search = Category_CategoryFilterDTO.Search;

            return CategoryFilter;
        }
    }
}

