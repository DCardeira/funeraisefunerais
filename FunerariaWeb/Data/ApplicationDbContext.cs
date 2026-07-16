using FunerariaWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Servico> Servicos { get; set; } = null!;
        public DbSet<Cerimonia> Cerimonias { get; set; } = null!;
        public DbSet<CerimoniaServico> CerimoniaServicos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Chave composta da tabela de junção -> concretiza o muitos-para-muitos.
            builder.Entity<CerimoniaServico>()
                .HasKey(cs => new { cs.CerimoniaId, cs.ServicoId });

            builder.Entity<CerimoniaServico>()
                .HasOne(cs => cs.Cerimonia)
                .WithMany(c => c.CerimoniaServicos)
                .HasForeignKey(cs => cs.CerimoniaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CerimoniaServico>()
                .HasOne(cs => cs.Servico)
                .WithMany(s => s.CerimoniaServicos)
                .HasForeignKey(cs => cs.ServicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Muitos-para-um: uma Cerimonia tem um Cliente; um Cliente tem várias Cerimonias.
            builder.Entity<Cerimonia>()
                .HasOne(c => c.Cliente)
                .WithMany(cl => cl.Cerimonias)
                .HasForeignKey(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Dados iniciais (seed) para não começarem com a app vazia.
            builder.Entity<Servico>().HasData(
                new Servico { Id = 1, Nome = "Velório clássico", Descricao = "Sala de velório equipada, 24h.", PrecoBase = 450.00m, Categoria = CategoriaServico.Cerimonia },
                new Servico { Id = 2, Nome = "Cremação", Descricao = "Serviço de cremação com urna standard.", PrecoBase = 690.00m, Categoria = CategoriaServico.Cerimonia },
                new Servico { Id = 3, Nome = "Transporte funerário", Descricao = "Transporte em carro fúnebre até 50km.", PrecoBase = 120.00m, Categoria = CategoriaServico.Transporte },
                new Servico { Id = 4, Nome = "Coroa de flores", Descricao = "Composição floral personalizada.", PrecoBase = 65.00m, Categoria = CategoriaServico.Floral },
                new Servico { Id = 5, Nome = "Tratamento de documentação", Descricao = "Certidão de óbito e registos legais.", PrecoBase = 40.00m, Categoria = CategoriaServico.Documentacao }
            );
        }
    }
}
