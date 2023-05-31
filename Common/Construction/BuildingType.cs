using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Construction
{
    public interface IBuildingType
    {
        double ChanceofOccuring { get; }
        bool CanBeBuilt { get; }
        bool NeedsHabitabilitytoBuild { get; }
    }
    public class BuildingType : IBuildingType
    {

        public BuildingType()
        {
        }
        public bool CanProduceCompounds { get; set; } //Can form compounds from elements
        public bool CanDoChemistry { get; set; } //if building has this set, than it's parent body can perform Chemistry. 
        public bool NeedsHabitabilitytoBuild { get; set; }
        public bool CanModifyFood { get; set; } // increases or decreases food production
        public bool CanModifyPopulation { get; set; } //increases or decreases birthrate and deathrate
        public bool CanResize { get; set; } // once built, building can be resized up or down. this will decrease or increase the size of it's properties
        public bool HasTechLevel { get; set; } // besides resizing,building can have a technology level. Adds new properties
        public bool CanBeBuilt { get; set; } //false for buildings that cannot be built by direct player actions. Such buildings are distributed at start or created with special events.
        /// <summary>
        /// integer denoting where the structure can be built according to these numbers
        ///  0 = everywhere, 1 is on the CentralHub, 2 is on an orbital body, 3 is in space.
        /// </summary>
        public int WhereCanItBeBuilt { get; set; } // 0 = everywhere, 1 is on the CentralHub, 2 is on an orbital body, 3 is in space. 
        public int FoodStorage { get; set; }
        public int PopulationHousing { get; set; } // how many people can live here max

        public string Name { get; set; }

        public double FoodModifier { get; set; } // if _modifiesfood, how much will it contribute
        public double PopulationModifier { get; set; } // if _modifiespopulation, how much will it contribute

        //Chance of structure that cannot be built occuring at the start on an orbitalbody or natural satellite
        public double ChanceofOccuring { get; set; } // if building is distributed at start, 
    }
}
