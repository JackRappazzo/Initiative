﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Encounters
{
    public class Creature
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Initiative { get; set; }
        public int InitiativeModifier { get; set; }
        public string Name { get; set; }
        public string SystemName { get; set; }
        public int HitPoints { get; set; }
        public int MaximumHitPoints { get; set; }
        public int ArmorClass { get; set; }
        public bool IsConcentrating { get; set; }
        public bool IsPlayer { get; set; }

        // Speed properties
        public int? WalkSpeed { get; set; }
        public int? FlySpeed { get; set; }
        public int? SwimSpeed { get; set; }
        public int? BurrowSpeed { get; set; }
        public int? ClimbSpeed { get; set; }
        public bool CanHover { get; set; }

        //Actions and spells
        public IEnumerable<CreatureAction> Actions { get; set; } = new List<CreatureAction>();


    }
}
