using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tamagotchi.Authorization.Models;

namespace Tamagotchi.Authorization.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20181017105634_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Tamagotchi.Authorization.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .HasMaxLength(50);

                    b.Property<string>("Login")
                        .HasMaxLength(24);

                    b.Property<string>("Password")
                        .HasMaxLength(24);

                    b.Property<int>("Pet");

                    b.HasKey("UserId");

                    b.ToTable("TamagotchiUser");
                });
        }
    }
}
