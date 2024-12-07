using AutoMapper;
using AutoMapper.QueryableExtensions;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Extentions;
using FoodieHub.API.Models.DTOs.Authentication;
using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Services.Implementations
{
    public class RecipeService : IRecipeService
    {
        private readonly AppDbContext _context;
        private readonly ImageExtentions _imageServices;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailService _mailService;
        public RecipeService(AppDbContext context, ImageExtentions imageServices, IAuthService authService, IMapper mapper, UserManager<ApplicationUser> userManager, IMailService mailService)
        {
            _context = context;
            _imageServices = imageServices;
            _authService = authService;
            _mapper = mapper;
            _userManager = userManager;
            _mailService = mailService;
        }



        public async Task<bool> Create(CreateRecipeDTO recipeDTO)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var uploadImg = await _imageServices.UploadImage(recipeDTO.File, "Recipes");

                var user = await _authService.GetCurrentUser();
                bool isAdmin = await _userManager.IsInRoleAsync(user,"Admin");
                if (uploadImg != null && uploadImg.Success)
                {
                    var newRecipe = new Recipe
                    {
                        Title = recipeDTO.Title,
                        Description = recipeDTO.Description,
                        CookTime = recipeDTO.CookTime,
                        Serves = recipeDTO.Serves,
                        IsAdminUpload = isAdmin,
                        IsActive = recipeDTO.IsActive,
                        ImageURL = uploadImg.FilePath.ToString() ?? "",
                        RecipeProducts = new List<RecipeProduct>(), 
                        RecipeSteps = new List<RecipeStep>(),     
                        Ingredients = new List<Ingredient>(),
                        UserID = user.Id,
                        CategoryID = recipeDTO.CategoryID
                    };
                    // Them bang recipe Product
                    foreach (var item in recipeDTO.RelativeProducts)
                    {
                        newRecipe.RecipeProducts.Add(new RecipeProduct
                        {
                            ProductID = item
                        });
                    }

                    // Them Recipe Ingredient
                    foreach (var item in recipeDTO.Ingredients)
                    {
                        newRecipe.Ingredients.Add(new Ingredient
                        {
                            Name = item.Name,
                            Quantity = item.Quantity,
                            ProductID = item.ProductID,
                            Unit = item.Unit
                        });
                    }

                    // Them bang recipe step
                    foreach (var item in recipeDTO.RecipeSteps)
                    {
                        var newStep = new RecipeStep
                        {
                            Step = item.Step,
                            Directions = item.Directions
                        };

                        if (item.ImageStep != null)
                        {
                            var uploadImageStep = await _imageServices.UploadImage(item.ImageStep, "Recipes");
                            if (uploadImageStep.Success)
                            {
                                newStep.ImageURL = uploadImageStep.FilePath.ToString();

                            }                        
                        }
                        newRecipe.RecipeSteps.Add(newStep);
                    }


                    await _context.Recipes.AddAsync(newRecipe);
                    var result = await _context.SaveChangesAsync();
                    if (result >0)
                    {
                        await transaction.CommitAsync();
                        return true;
                    }
                    await transaction.RollbackAsync();
                    return false;
                }
                return false;
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<ServiceResponse> Get(string? search, int? pageSize, int? currentPage)
        {
            var recipes = _context.Recipes
                .Where(x => x.IsActive)
                .ProjectTo<GetRecipes>(_mapper.ConfigurationProvider)
                        .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                recipes = recipes.Where(x => x.Title.ToLower().Contains(search.ToLower()));

            }
            if (pageSize.HasValue && currentPage.HasValue)
            {
                var list = await recipes.ToListAsync();
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Get recipes successfully",
                    Data = list.Paginate(pageSize.Value, currentPage.Value),
                    StatusCode = 200
                };
            }
            return new ServiceResponse
            {
                Success = true,
                Message = "Get recipes successfully",
                Data = await recipes.ToListAsync(),
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> GetDetail(int recipeID)
        {
            var result = await _context.Recipes
                .Where(x => x.IsActive && x.RecipeID == recipeID)
                .ProjectTo<GetRecipeDetail>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync();
            if (result == null)
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Not found this recipe",
                    StatusCode = 404
                };
            return new ServiceResponse
            {
                Success = true,
                Message = "Get detail success",
                Data = result,
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> GetForAdmin(string? search, int? categoryID, bool? isActive, bool? isDeleted, int pageSize, int currentPage)
        {
            var listarticles = _context.Recipes.AsQueryable();
            var listAdminID = await ListAdminIDs();
            listarticles = listarticles.Where(x => listAdminID.Contains(x.UserID));
            if (!string.IsNullOrEmpty(search))
            {
                listarticles = listarticles.Where(x => x.Title.ToLower().Contains(search.ToLower()));
            }
            if (categoryID.HasValue)
            {
                listarticles = listarticles.Where(x => x.CategoryID == categoryID);
            }
            if (isActive.HasValue)
            {
                listarticles = listarticles.Where(x => x.IsActive == isActive.Value);
            }        
            var articles = await listarticles
            .ProjectTo<GetRecipesForAdmin>(_mapper.ConfigurationProvider).ToListAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = "Get recipes successfully",
                Data = articles.Paginate(pageSize, currentPage),
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> Rating(RatingDTO ratingDTO)
        {
            string userID = _authService.GetUserID();
            var rating = await _context.Ratings.FirstOrDefaultAsync(x => x.UserID == userID && x.RecipeID == ratingDTO.RecipeID);
            if (rating == null)
            {
                // Them moi
                await _context.Ratings.AddAsync(new Rating
                {
                    UserID = userID,
                    RecipeID = ratingDTO.RecipeID,
                    RatingValue = ratingDTO.RatingValue
                });
            }
            else
            {
                // Cap nhat
                rating.RatingValue = ratingDTO.RatingValue;
                _context.Ratings.Update(rating);
            }
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Rating successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Rating failed",
                    StatusCode = 400
                };
            }
        }

      
        public async Task<ServiceResponse> Update(int recipeID, UpdateRecipe recipeDTO)
        {
            var recipe = await _context.Recipes.FindAsync(recipeID);
            if (recipe == null) return new ServiceResponse
            {
                Success = false,
                Message = "No recipe found with given ID",
                StatusCode = 404
            };
            _mapper.Map(recipeDTO, recipe);
            var user = await _authService.GetCurrentUser();
            if (user == null) return new ServiceResponse
            {
                Success = false,
                Message = "Unauthorized",
                StatusCode = 401
            };
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                recipe.IsActive = recipeDTO.IsActive ?? false;
            }
            else
            {
                recipe.IsActive = false;
            }

            if (recipeDTO.File != null)
            {
                var uploadImg = await _imageServices.UploadImage(recipeDTO.File, "Recipes");
                if (uploadImg != null && uploadImg.Success)
                {
                    _imageServices.DeleteImage(recipe.ImageURL);
                    recipe.ImageURL = uploadImg.FilePath.ToString();
                }
                else
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Failed to upload image. Please try again",
                        StatusCode = 400
                    };
                }

            }
            _context.Recipes.Update(recipe);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {

                return new ServiceResponse
                {
                    Success = true,
                    Message = "The recipe has been successfully updated",
                    StatusCode = 201
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to update recipe. Please try again",
                    StatusCode = 400
                };
            }
        }

        public async Task<ServiceResponse> GetDetailForAdmin(int receipeID)
        {
            var result = await _context.Recipes
                .Where(x => x.RecipeID == receipeID)
                .Select(x => new
                {
                    x.RecipeID,
                    x.Title,
                    x.Serves,
                    x.CookTime,
                    x.Ingredients,
                    x.ImageURL,
                    x.CategoryID,
                    x.CreatedAt,
                    x.UserID,
                    x.User.Fullname,
                    x.User.Avatar,
                    TotalComment = x.Comments.Where(x=>x.RecipeID!=null).Count(),
                    TotalRating = x.Ratings.Count() > 0 ? x.Ratings.Count() : 0,
                    AverageRating = x.Ratings.Count() > 0 ? x.Ratings.Average(x => x.RatingValue) : 0,
                    TotalFavorite = x.Favorites.Where(x => x.RecipeID != null).Count()
                })
                .FirstOrDefaultAsync();
            if (result == null)
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Not found this recipe",
                    StatusCode = 404
                };
            return new ServiceResponse
            {
                Success = true,
                Message = "Get recipe success",
                Data = result,
                StatusCode = 200
            };
        }
        

        public async Task<ServiceResponse> GetByUser(string userID)
        {
            var recipes = await _context.Recipes.Where(x => x.UserID == userID)
                 .ProjectTo<GetRecipes>(_mapper.ConfigurationProvider)
                        .ToListAsync();
            return new ServiceResponse
            {
                Success = true,
                Data = recipes,
                StatusCode = 200
            };
        }
        public async Task<ServiceResponse> GetOfUser()
        {
            var userID = _authService.GetUserID();
            var recipes = await _context.Recipes.Where(x => x.UserID == userID)
              .Select(x => new
              {
                  x.RecipeID,
                  x.Title,
                  x.ImageURL,
                  x.IsActive,
                  TotalComments = x.Comments != null ? x.Comments.Where(x=>x.RecipeID!=null).Count() : 0,
                  TotalFavorites = x.Favorites != null ? x.Favorites.Where(x=>x.RecipeID!=null).Count() : 0,
                  RatingAverage = x.Ratings != null && x.Ratings.Any() ? x.Ratings.Average(r => r.RatingValue) : 0,
                  x.CreatedAt,
                  x.CategoryID,
                  x.RecipeCategory.CategoryName               
              })
                       .ToListAsync();
            return new ServiceResponse
            {
                Success = true,
                Data = recipes,
                StatusCode = 200
            };
        }

        public async Task<object> GetRecipeOfUserForAdmin(string status)
        {
            var listAdmin = await ListAdminIDs();
            var recipes = _context.Recipes.Where(x => !listAdmin.Contains(x.UserID)).AsQueryable();
            var data = new Object();
         
            return data;
        }


        public async Task<List<string>> ListAdminIDs()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            return admins.Select(admin => admin.Id).ToList();
        }

        public string GeneratePostDeletedEmail(string fullName, string postTitle, DateTime deletedDate)
        {
            return $@"
        Dear {fullName},

        We regret to inform you that your post titled ""{postTitle}"" has been removed from our platform as of {deletedDate}.

        This action was taken because the post violated our community guidelines or terms of service. If you believe this decision was made in error or would like further clarification, please do not hesitate to contact us at nguyenphuc14112003@gmail.com.

        We encourage you to review our community guidelines to ensure future content aligns with our policies.

        Thank you for your understanding and cooperation.

        Best regards,
        The Support Team
    ";
        }


        public async Task<bool> DeleteOfUser(int id)
        {
            var userid = _authService.GetUserID();

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) return false;
            if(userid != recipe.UserID) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {              
                var listComments = await _context.Comments.Where(x => x.RecipeID == id).ToListAsync();
                var listFavorites = await _context.Favorites.Where(x => x.RecipeID == id).ToListAsync();
                var listRatings = await _context.Ratings.Where(x => x.RecipeID == id).ToListAsync();
                if (listComments.Count() > 0)
                {
                    foreach (var comment in listComments)
                    {
                        _context.Comments.Remove(comment);
                    }
                }

                foreach (var favorite in listFavorites)
                {
                    _context.Favorites.Remove(favorite);
                }

                foreach (var rating in listRatings)
                {
                    _context.Ratings.Remove(rating);
                }
                var imgPath = recipe.ImageURL;
                _context.Recipes.Remove(recipe);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<object> GetDetailForEdit(int id)
        {
            var userid = _authService.GetUserID();
            var data = await _context.Recipes.Where(x => x.UserID == userid && x.RecipeID == id)
                .Select(x => new
                {
                    x.RecipeID,
                    x.Title,
                    x.CookTime,
                    x.Ingredients,
                    x.ImageURL,
                    x.Serves,
                    x.CategoryID
                }).FirstOrDefaultAsync();

            return data;
        }

        public async Task<bool> DeleteRecipeOfUser(int id)
        {        
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var recipe = await _context.Recipes.FindAsync(id);
                if (recipe == null) return false;
                var listComments = await _context.Comments.Where(x => x.RecipeID == id).ToListAsync();
                var listFavorites = await _context.Favorites.Where(x => x.RecipeID == id).ToListAsync();
                var listRatings = await _context.Ratings.Where(x => x.RecipeID == id).ToListAsync();
                if (listComments.Count() > 0)
                {
                    foreach (var comment in listComments)
                    {
                        _context.Comments.Remove(comment);
                    }
                }
                if (listFavorites.Count() > 0)
                {
                    foreach (var favorite in listFavorites)
                    {
                        _context.Favorites.Remove(favorite);
                    }
                }
                if (listRatings.Count() > 0)
                {
                    foreach (var rating in listRatings)
                    {
                        _context.Ratings.Remove(rating);
                    }
                }
                var imgPath = recipe.ImageURL;
                var user = await _userManager.FindByIdAsync(recipe.UserID);
                var mailRequest = new MailRequest
                {
                    ToEmail = user.Email,
                    Subject = "Your recipe has been deleted",
                    Body = GeneratePostDeletedEmail(user.Fullname, recipe.Title, DateTime.Now)
                };
                _context.Recipes.Remove(recipe);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await transaction.CommitAsync();
                    _imageServices.DeleteImage(imgPath);

                    await _mailService.SendEmailAsync(mailRequest);

                    return true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
