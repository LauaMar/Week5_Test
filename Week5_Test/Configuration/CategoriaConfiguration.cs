using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week5_Test.Model;

namespace Week5_Test.Configuration
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Descrizione)
                .IsRequired()
                .HasMaxLength(100);

            //relazioni
            builder.HasMany(c => c.Spese)
                   .WithOne(s => s.Categoria);
        }
    }
}
