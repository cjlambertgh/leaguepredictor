﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LeaguePredictor.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class leaguepredictorEntities : DbContext
    {
        public leaguepredictorEntities()
            : base("name=leaguepredictorEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<team> teams { get; set; }
        public virtual DbSet<league> leagues { get; set; }
        public virtual DbSet<players_selections> players_selections { get; set; }
        public virtual DbSet<team_rankings> team_rankings { get; set; }
        public virtual DbSet<player> players { get; set; }
    }
}
