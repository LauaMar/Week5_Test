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
    public class SpesaConfiguration : IEntityTypeConfiguration<Spesa>
    {
        public void Configure(EntityTypeBuilder<Spesa> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Descrizione)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(s => s.Data)
                .IsRequired();
            builder.Property(s => s.Utente)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(s => s.Importo)
                .IsRequired();
            builder.Property(s => s.Approvato)
                .IsRequired();

            //relazioni
            builder.HasOne(s => s.Categoria)
                   .WithMany(c => c.Spese);

        }
    }
}
