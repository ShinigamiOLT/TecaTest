using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modelos;

using Modelos.BaseApp;
using Modelos.Identity;
using Persistencia.DataBase.Config;

namespace Persistencia.DataBase
{
    public class ApplicationDbContext :
        IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>,
            ApplicationUserRole, IdentityUserLogin<string>,
            IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        /*****              Tablas de Arranque  ****/

        public DbSet<MyToken> MyToken { get; set; }
     

        /*********************** S Y S T E M A ***********************/
      
        /*********************** C A T A L O G O S ***********************/
      

        /*********************** D A T O S ***********************/
    
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<CuentaAhorro> CuentasAhorros { get; set; }
        public DbSet<OperacionPorCuenta> OperacionesPorCuentas { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("dbo");

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            new ApplicationRoleConfig(builder.Entity<ApplicationRole>());
            new ApplicationUserConfig(builder.Entity<ApplicationUser>());
          

          

           
            builder.Entity<CuentaAhorro>().Property(x => x.SaldoActual).HasColumnType("decimal(18, 4)");
            builder.Entity<OperacionPorCuenta>().Property(x => x.SaldoInicial).HasColumnType("decimal(18, 4)"); 
            builder.Entity<OperacionPorCuenta>().Property(x => x.MontoTransaccion).HasColumnType("decimal(18, 4)");
            builder.Entity<OperacionPorCuenta>().Property(x => x.SaldoFinal).HasColumnType("decimal(18, 4)");

            
            InsertaRoles(builder.Entity<ApplicationRole>());
            SeedUsers(builder);

        }

       

    
    
        private static void InsertaRoles(EntityTypeBuilder<ApplicationRole> entityBuilder)
        {
            var admin = new ApplicationRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = RoleHelper.Admin,
                NormalizedName = RoleHelper.Admin
            };
            var usuario = new ApplicationRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = RoleHelper.Usuario,
                NormalizedName = RoleHelper.Usuario
            };
           
            entityBuilder.HasData(admin);
            entityBuilder.HasData(usuario);
        }

        private static void SeedUsers(ModelBuilder builder)
        {
            var user = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Admin",
                Email = "admin@demo.com",
                LockoutEnabled = false,
                PhoneNumber = "1234567890",
                Nombre = "Admin",
                Apellido = "Systema",
                EsBorrado = Enums.Borrado.Existe,
                Avatar = "http://res.cloudinary.com/ovidiolt/image/upload/v1583956552/o0pioqvizcrdwextc1pn.png"
            };

            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            var hast = passwordHasher.HashPassword(user, "$Demo.2021");

            user.PasswordHash = hast;
            user.NormalizedUserName = user.UserName.ToUpper();
            user.NormalizedEmail = user.Email.ToUpper();
            user.EmailConfirmed = true;
            builder.Entity<ApplicationUser>().HasData(user);
        }
    }

   

    public class ApplicationRoleConfig
    {
        public ApplicationRoleConfig(EntityTypeBuilder<ApplicationRole> entityBuilder)
        {
            entityBuilder.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .IsRequired();
        }
    }

  

   
}