using Domain.Formulas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class FormulaEntityConfiguration : IEntityTypeConfiguration<Formula>
{
    public void Configure(EntityTypeBuilder<Formula> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(255);
        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.Expression).HasColumnType("jsonb");
        builder.Property(x => x.FieldsConfiguration).HasColumnType("jsonb");
    }
}
