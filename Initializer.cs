using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace Skillz_Code
{
    partial class SSJS12Bot : IPirateBot
    {
        public static PirateGame game;
        public const bool Debug = true;
        public Dictionary<Mothership, int> bunkerCount;
        protected List<Pirate> availablePirates;
        protected List<Capsule> myCapsules;
        protected List<Capsule> enemyCapsules;
        protected List<Pirate> myPirates;

        protected Dictionary<Pirate, Location> pirateDestinations;
        public void DoTurn(PirateGame game)
        {
            Initialize(game);
        }
        public void Initialize(PirateGame pirateGame)
        {
            game = pirateGame;
            availablePirates = pirateGame.GetMyLivingPirates().ToList();
            bunkerCount = new Dictionary<Mothership, int>();
            foreach(var mothership in game.GetEnemyMotherships())
                bunkerCount[mothership]=0;
            myCapsules = game.GetMyCapsules().ToList();
            enemyCapsules = game.GetEnemyCapsules().ToList();
            myPirates = game.GetMyLivingPirates().ToList();
            pirateDestinations = new Dictionary<Pirate, Location>();
        }
    }
}