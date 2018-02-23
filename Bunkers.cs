using System.Collections.Generic;
using System.Linq;
using Pirates;

namespace Skillz_Code
{
    partial class SSJS12Bot : IPirateBot
    {
        public void PerformAggressiveBunker()
        {
            foreach (var mothership in game.GetEnemyMotherships())
            {
                foreach (var capsule in enemyCapsules.Where(cap => cap.Holder != null &&
                        GetBestMothershipThroughWormholes(cap.Holder).Equals(mothership)))
                {
                    bunkerCount[mothership]++;
                    var distanceToBorder = capsule.Distance(GetClosestToBorder(capsule.Location));
                    var useablePirates = availablePirates.Where(p => p.Steps(mothership) > p.PushReloadTurns).OrderBy(p => p.Steps(mothership));
                    int count = 0, pushDistanceUsed = 0;
                    foreach (var pirate in useablePirates.OrderByDescending(p => p.PushDistance))
                    {
                        if (pushDistanceUsed < distanceToBorder)
                        {
                            count++;
                            pushDistanceUsed += pirate.PushDistance;
                        }
                    }
                    var requiredPiratesCount = Min((count == 0) ? 1 : count, capsule.Holder.NumPushesForCapsuleLoss);
                    ("Mothership: " + mothership + ", Capsule: " + capsule).Print();
                    ("Required pirates: " + requiredPiratesCount).Print();
                    ("Push pirates: " + count).Print();
                    ("Capsule loss: " + capsule.Holder.NumPushesForCapsuleLoss).Print();
                    var bestWormhole = GetBestWormhole(mothership.Location, capsule.Holder);
                    if (useablePirates.Count() >= requiredPiratesCount)
                    {
                        if (requiredPiratesCount == count)
                            useablePirates = useablePirates.OrderByDescending(p => p.PushDistance);
                        var usedPirates = new List<Pirate>();
                        foreach (var pirate in useablePirates.Take(requiredPiratesCount))
                        {
                            if (bestWormhole != null && bestWormhole.Partner != null &&
                                bestWormhole.Partner.InRange(mothership, mothership.UnloadRange * 3))
                            {
                                // Add push wormhole here
                            }
                            else
                            {
                                if (!TryPushEnemyCapsuleAggressively(pirate, capsule))
                                {
                                    var rangeNeeded = bunkerCount[mothership].Power(2) * game.PushRange;
                                    AssignDestination(pirate, mothership.Location.Towards(capsule, rangeNeeded));
                                }
                            }
                            usedPirates.Add(pirate);
                        }
                        availablePirates = availablePirates.Except(usedPirates).ToList();

                    }
                }
            }
        }

        protected void PerformDefensiveBunker()
        {
            ("Entered bunker").Print();
            foreach (var capsule in game.GetEnemyCapsules().Where(cap => cap.Holder != null))
            {
                var bestMothership = GetBestMothershipThroughWormholes(capsule.Holder);
                if (bestMothership != null)
                {
                    var distanceToBorder = capsule.Distance(GetClosestToBorder(capsule.Location));
                    var useablePirates = availablePirates.Where(p => p.Steps(bestMothership) >= p.PushReloadTurns).OrderBy(p => p.Steps(bestMothership));
                    int count = 0, pushDistanceUsed = 0;
                    foreach (var pirate in useablePirates.OrderByDescending(p => p.PushDistance))
                    {
                        if (pushDistanceUsed < distanceToBorder)
                        {
                            count++;
                            pushDistanceUsed += pirate.PushDistance;
                        }
                    }
                    var requiredPiratesCount = Min((count == 0) ? 1 : count, capsule.Holder.NumPushesForCapsuleLoss);
                    if (useablePirates.Count() >= requiredPiratesCount)
                    {
                        var usedPirates = new List<Pirate>();
                        foreach (var pirate in useablePirates.Take(requiredPiratesCount))
                        {
                            // if(TryPushEnemyCapsuleAggressively(pirate, capsule))
                            AssignDestination(pirate, bestMothership.Location.Towards(capsule, (int) (bestMothership.UnloadRange * 0.5)));
                            // Attempt push
                            usedPirates.Add(pirate);

                        }
                        availablePirates = availablePirates.Except(usedPirates).ToList();
                    }
                }
            }
        }
    }
}