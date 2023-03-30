using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered.IntegrationTests.OwnedEntities
{
    [Owned]
    public class Ownee
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Owner 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Ownee Ownee { get; set; }
    }


    class ApplicationDbContext : DbContext
    {
        readonly string _databaseName;

        public ApplicationDbContext(string databaseName)
        {
            _databaseName = databaseName;
        }

        public OwneeTrigger OwneeTrigger { get; } = new OwneeTrigger();
        public OwnerTrigger OwnerTrigger { get; } = new OwnerTrigger();

        public DbSet<Owner> B => Set<Owner>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(_databaseName);
            optionsBuilder.UseTriggers(triggerOptions => {
                triggerOptions.AddTrigger(OwneeTrigger);
                triggerOptions.AddTrigger(OwnerTrigger);
            });
        }
    }

    class OwneeTrigger : IBeforeSaveTrigger<Ownee>
    {
        public void BeforeSave(ITriggerContext<Ownee> context)
        {
            Calls++;
        }

        public int Calls { get; set; }
    }

    class OwnerTrigger : IBeforeSaveTrigger<Owner>
    {
        public void BeforeSave(ITriggerContext<Owner> context)
        {
            Calls++;
        }

        public int Calls { get; set; }
    }
}
