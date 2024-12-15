using AutoMapper;
using AutoMapper.QueryableExtensions;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Extentions;
using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FoodieHub.API.Repositories.Implementations
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
                var userID = _authService.GetUserID();
                bool isAdmin = await _authService.IsAdmin(userID);
                var uploadImage = await _imageServices.UploadImage(recipeDTO.File,"Recipes");
                if (uploadImage.Success)
                {
                    var newRecipe = new Recipe
                    {
                        Title = recipeDTO.Title,
                        Description = recipeDTO.Description,
                        CookTime = recipeDTO.CookTime,
                        Serves = recipeDTO.Serves,
                        IsAdminUpload = isAdmin,
                        IsActive = recipeDTO.IsActive,
                        ImageURL = uploadImage.FilePath.ToString()??"",
                        RecipeProducts = new List<RecipeProduct>(),
                        RecipeSteps = new List<RecipeStep>(),
                        Ingredients = new List<Ingredient>(),
                        UserID = userID,
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
                            var upload = await _imageServices.UploadImage(item.ImageStep, "RecipeSteps");
                            newStep.ImageURL = upload.FilePath.ToString();
                        }
                        newRecipe.RecipeSteps.Add(newStep);
                    }
                    int countEntity = recipeDTO.RelativeProducts.Count() + recipeDTO.Ingredients.Count()
                        + recipeDTO.RecipeSteps.Count();
                    await _context.Recipes.AddAsync(newRecipe);
                    var result = await _context.SaveChangesAsync();
                    if (result > countEntity)
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
        public async Task<PaginatedModel<GetRecipeDTO>> Get(QueryRecipeModel query)
        {
            var listRecipes = _context.Recipes.AsQueryable();
            if (!string.IsNullOrEmpty(query.SearchIngredient))
            {
                listRecipes = listRecipes.Where(recipe =>
                    recipe.Ingredients.Any(ingredient =>
                        ingredient.Name.ToLower().Contains(query.SearchIngredient.ToLower())));
            }

            if (query.CategoryID.HasValue)
            {
                listRecipes = listRecipes.Where(x => x.CategoryID == query.CategoryID.Value);
            }
            if (query.IsActive.HasValue)
            {
                listRecipes = listRecipes.Where(x => x.IsActive == query.IsActive.Value);
            }
            if (query.IsAdminUpload.HasValue)
            {
                listRecipes = listRecipes.Where(x => x.IsAdminUpload == query.IsAdminUpload.Value);
            }
            if (query.CookOption.HasValue)
            {
                switch (query.CookOption.Value)
                {
                    case 1: // Cook time <= 30 minutes
                        listRecipes = listRecipes.Where(x => x.CookTime <= TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(30)));
                        break;
                    case 2: // Cook time between 30 and 60 minutes
                        listRecipes = listRecipes.Where(x => x.CookTime > TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(30))
                                                          && x.CookTime <= TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(60)));
                        break;
                    case 3: // Cook time > 60 minutes
                        listRecipes = listRecipes.Where(x => x.CookTime > TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(60)));
                        break;
                    default:
                        break;
                }
            }
            return await listRecipes
            .ProjectTo<GetRecipeDTO>(_mapper.ConfigurationProvider).ApplyQuery(query,x=>x.Title);
        }
        public async Task<bool> Rating(CreateRatingDTO ratingDTO)
        {
            string userID = _authService.GetUserID();
            var rating = await _context.Ratings.FirstOrDefaultAsync(x => x.UserID == userID && x.RecipeID == ratingDTO.RecipeID);
            if (rating == null)
            {
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
            
            return await _context.SaveChangesAsync()>0;
        }
        public async Task<IEnumerable<GetRecipeDTO>> GetByUser(string userID)
        {
            return await _context.Recipes.Where(x => x.UserID == userID)
                 .ProjectTo<GetRecipeDTO>(_mapper.ConfigurationProvider)
                        .ToListAsync();
        }
        public async Task<IEnumerable<GetRecipeDTO>> GetOfUser()
        {
            var userID = _authService.GetUserID();
            return await _context.Recipes.Where(x => x.UserID == userID)
                  .ProjectTo<GetRecipeDTO>(_mapper.ConfigurationProvider)
                         .ToListAsync();
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


        public async Task<bool> Delete(int id)
        {
            var userId = _authService.GetUserID();
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) return false;
            if(userId != recipe.UserID)
            {
                bool isAdmin = await _authService.IsAdmin(userId);
                if (!isAdmin) return false;
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {              
                var listComments = await _context.Comments.Where(x => x.RecipeID == id).ToListAsync();
                var listFavorites = await _context.Favorites.Where(x => x.RecipeID == id).ToListAsync();
                var listRatings = await _context.Ratings.Where(x => x.RecipeID == id).ToListAsync();
                var listSteps = await _context.RecipeSteps.Where(x=>x.RecipeID== id).ToListAsync();
                var ingredients = await _context.Ingredients.Where(x => x.RecipeID == id).ToListAsync();
                var recipeProducts = await _context.RecipeProducts.Where(x=>x.RecipeID==id).ToListAsync();
                var imgPath = recipe.ImageURL;
                var listImagePath = new List<string>(); 
                foreach (var comment in listComments)
                {
                    _context.Comments.Remove(comment);

                }
                foreach (var favorite in listFavorites)
                {
                    _context.Favorites.Remove(favorite);
                }
                foreach (var rating in listRatings)
                {
                    _context.Ratings.Remove(rating);
                }
                foreach (var step in listSteps)
                {
                    _context.RecipeSteps.Remove(step);
                    if(step.ImageURL != null) listImagePath.Add(step.ImageURL);
                }
                foreach (var ingredient in ingredients)
                {
                    _context.Ingredients.Remove(ingredient);
                }
                foreach (var product in recipeProducts)
                {
                    _context.RecipeProducts.Remove(product);
                }
                _context.Recipes.Remove(recipe);
                var result = await _context.SaveChangesAsync();
                int entityCount = listComments.Count() + listFavorites.Count() + listRatings.Count() + listSteps.Count()
                    + ingredients.Count()+recipeProducts.Count();
                if (result >entityCount)
                {
                    _imageServices.DeleteImage(imgPath);
                    foreach (var item in listImagePath)
                    {
                        _imageServices.DeleteImage(item);
                    }

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
    
        public async Task<bool> Update(UpdateRecipeDTO recipeDTO)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entityToUpdate = await _context.Recipes.FindAsync(recipeDTO.RecipeID);
                if (entityToUpdate == null) return false;

                // Cap nhat recipe
                entityToUpdate.Title = recipeDTO.Title;
                entityToUpdate.Description=recipeDTO.Description;
                entityToUpdate.CookTime = recipeDTO.CookTime;
                entityToUpdate.Serves = recipeDTO.Serves;
                entityToUpdate.IsActive = recipeDTO.IsActive;
                entityToUpdate.CategoryID = recipeDTO.CategoryID;

                if (recipeDTO.File != null)
                {
                    var uploadResult = await _imageServices.UploadImage(recipeDTO.File, "Recipes");
                    if (uploadResult.Success)
                    {
                        _imageServices.DeleteImage(entityToUpdate.ImageURL);
                        entityToUpdate.ImageURL = uploadResult.FilePath.ToString() ?? "";
                    }
                }
                _context.Recipes.Update(entityToUpdate);

                // STEP
                var listSteps = await _context.RecipeSteps.Where(x => x.RecipeID == entityToUpdate.RecipeID).ToListAsync();

                foreach (var item in recipeDTO.RecipeSteps)
                {
                    var step = listSteps.FirstOrDefault(x => x.Id == item.Id);
                    if(step != null)
                    {
                        // cap nhat
                        step.Step = item.Step;
                        step.Directions = item.Directions;
                        if(item.FileStep != null)
                        {
                            var uploadStep = await _imageServices.UploadImage(item.FileStep, "RecipeSteps");
                            if (uploadStep.Success)
                            {
                                if(item.ImageURL != null) _imageServices.DeleteImage(item.ImageURL);
                                step.ImageURL =uploadStep.FilePath.ToString()??"";
                            }
                        } 
                        _context.RecipeSteps.Update(step);
                    }
                    else
                    {
                        var path = "";
                        if (item.FileStep != null)
                        {
                            var uploadImage = await _imageServices.UploadImage(item.FileStep, "RecipeSteps");
                            if (uploadImage.Success)
                            {
                                path = uploadImage.FilePath.ToString() ?? "";
                            }
                        }
                        // them moi
                        var newStep = new RecipeStep
                        {
                            RecipeID = recipeDTO.RecipeID,
                            Step = item.Step,
                            Directions = item.Directions,
                            ImageURL = path
                        };

                        await _context.RecipeSteps.AddAsync(newStep);
                    }
                }
                var listStepIdCurrent = recipeDTO.RecipeSteps.Select(x => x.Id);
                var listImage = new List<string>();
                foreach (var item in listSteps)
                {
                    if (!listStepIdCurrent.Contains(item.Id))
                    {
                        if (item.ImageURL != null)
                        {
                            listImage.Add(item.ImageURL);
                        }
                        _context.RecipeSteps.Remove(item);
                    }
                }


                // INGREDIENT
                var listIngredients = await _context.Ingredients.Where(x => x.RecipeID == entityToUpdate.RecipeID).ToListAsync();

                foreach (var item in recipeDTO.Ingredients)
                {
                    var ingredient = listIngredients.FirstOrDefault(x => x.Id == item.Id);
                    if (ingredient != null)
                    {
                        // cap nhat
                        ingredient.Name = item.Name;
                        ingredient.Quantity = item.Quantity;
                        ingredient.Unit = item.Unit; 
                        ingredient.ProductID=item.ProductID;
                        _context.Ingredients.Update(ingredient);
                    }
                    else
                    {                        
                        // them moi
                        var newIngredient = new Ingredient
                        {
                            RecipeID = recipeDTO.RecipeID,
                            Name = item.Name,
                            Quantity = item.Quantity,
                            Unit = item.Unit,
                            ProductID = item.ProductID,
                        };

                        await _context.Ingredients.AddAsync(newIngredient);
                    }
                }
                var listIngredientIdCurrent = recipeDTO.Ingredients.Select(x => x.Id);
                foreach (var item in listIngredients)
                {
                    if (!listIngredientIdCurrent.Contains(item.Id))
                    {
                        _context.Ingredients.Remove(item);
                    }
                }

                var result = await _context.SaveChangesAsync();
                int entityChangeCount = recipeDTO.Ingredients.Count() + recipeDTO.RecipeSteps.Count();
                if(result > entityChangeCount)
                {
                    foreach (var item in listImage)
                    {
                        _imageServices.DeleteImage(item);
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                await transaction.RollbackAsync();
                return false;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<DetailRecipeDTO?> GetByID(int id)
        {
            return await _context.Recipes
                .ProjectTo<DetailRecipeDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x=>x.RecipeID==id);
        }
    }
}
