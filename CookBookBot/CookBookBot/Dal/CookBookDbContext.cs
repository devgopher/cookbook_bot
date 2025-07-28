using CookBookBot.Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace CookBookBot.Dal;

public partial class CookBookDbContext : DbContext
{
    public CookBookDbContext()
    {
    }

    public CookBookDbContext(DbContextOptions<CookBookDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<Ingredient2Recipe> Ingredient2Recipes { get; set; }

    public virtual DbSet<RecipesDataset> RecipesDatasets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("<secret>");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("images_pk");

            entity.ToTable("images");

            entity.HasIndex(e => e.RecipeId, "images_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Image1)
                .HasMaxLength(120000)
                .HasColumnName("image");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Image)
                .HasForeignKey<Image>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("images_recipes_dataset_fk");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ingredients");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LocalizedName)
                .HasMaxLength(50)
                .HasColumnName("localized_name");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Ingredient2Recipe>(entity =>
        {
            entity.HasKey(e => new { e.IngredientId, e.RecipeId }).HasName("ingredient2recipe_pk");

            entity.ToTable("ingredient2recipe");

            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
        });

        modelBuilder.Entity<RecipesDataset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("recipes_dataset_pkey");

            entity.ToTable("recipes_dataset");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Directions)
                .HasMaxLength(1024)
                .HasColumnName("directions");
            entity.Property(e => e.Ingredients)
                .HasMaxLength(512)
                .HasColumnName("ingredients");
            entity.Property(e => e.Link)
                .HasMaxLength(64)
                .HasColumnName("link");
            entity.Property(e => e.Ner)
                .HasMaxLength(256)
                .HasColumnName("NER");
            entity.Property(e => e.Site)
                .HasMaxLength(50)
                .HasColumnName("site");
            entity.Property(e => e.Source)
                .HasMaxLength(50)
                .HasColumnName("source");
            entity.Property(e => e.Title)
                .HasMaxLength(64)
                .HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
