using Microsoft.EntityFrameworkCore;
using TestData;
using TestData.Database;

namespace FlexFetcherTests.Stubs.Database;

public class TestDbContext : DbContext
{
    public DbSet<PeopleEntity> People { get; set; }
    public DbSet<AddressEntity> Addresses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=FlexFetcherUnitTests.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PeopleEntity>().HasKey(p => p.Id);
        modelBuilder.Entity<PeopleEntity>().Property(p => p.Occupation).HasConversion<string>();
        modelBuilder.Entity<AddressEntity>().HasKey(a => a.Id);
        modelBuilder.Entity<UserEntity>().HasKey(a => a.Id);
        modelBuilder.Entity<GroupEntity>().HasKey(g => g.Id);
        modelBuilder.Entity<PeopleGroupEntity>().HasKey(pg => new { pg.PersonId, pg.GroupId });

        modelBuilder.Entity<PeopleEntity>()
                    .HasOne(p => p.Address)
                    .WithOne()
                    .HasForeignKey<AddressEntity>(p => p.PersonId)
                    .IsRequired(false);

        modelBuilder.Entity<PeopleEntity>()
                    .HasOne(p => p.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(p => p.CreatedByUserId)
                    .IsRequired();

        modelBuilder.Entity<PeopleEntity>()
                    .HasOne(p => p.UpdatedByUser)
                    .WithMany()
                    .HasForeignKey(p => p.UpdatedByUserId)
                    .IsRequired(false);

        modelBuilder.Entity<PeopleEntity>()
                    .HasOne(p => p.GenderEntity)
                    .WithMany()
                    .HasForeignKey(p => p.Gender)
                    .IsRequired();

        var people = InMemoryDataHelper.GetPeople();
        var addresses = people.Select(p => p.Address).Where(a => a != null).ToList();
        people.ForEach(p => p.Address = null);
        var users = people.Select(p => p.CreatedByUser).ToList();
        users.AddRange(people.Select(p => p.UpdatedByUser).Where(u => u != null));
        users = users.Distinct().ToList();
        people.ForEach(p => p.CreatedByUser = null);
        people.ForEach(p => p.UpdatedByUser = null);
        var groups = people.SelectMany(p => p.PeopleGroups).Select(pg => pg.Group).Distinct().ToList();
        var peopleGroups = people.SelectMany(p => p.PeopleGroups).ToList();
        peopleGroups.ForEach(pg => pg.Person = null);
        peopleGroups.ForEach(pg => pg.Group = null);
        people.ForEach(p => p.PeopleGroups = new List<PeopleGroupEntity>());

        var genders = InMemoryDataHelper.GetGenders();

        modelBuilder.Entity<GroupEntity>().HasData(groups!);
        modelBuilder.Entity<PeopleEntity>().HasData(people);
        modelBuilder.Entity<PeopleGroupEntity>().HasData(peopleGroups);
        modelBuilder.Entity<AddressEntity>().HasData(addresses!);
        modelBuilder.Entity<UserEntity>().HasData(users!);
        modelBuilder.Entity<GenderEntity>().HasData(genders);
    }
}