using System;
using System.Collections.Generic;

using NthDimension.Algebra;

namespace NthDimension.Procedural.Tree
{
    public class Tree
    {
        private readonly Random _random = new Random();

        private Vector3 _position;
        private Branch _root;
        private int _growIter;

        private const int LeafCount = 100;
        private const int TreeLength = 100;
        private const int TreeWidth = 100;
        private const int TreeHeight = 100;
        private const int TrunkHeight = 20;
        private const int MinDistance = 1;
        private const int MaxDistance = 100;
        private const int BranchLength = 1;

        public List<Leaf> Leaves;
        public Dictionary<Vector3, Branch> Branches;
        public bool DoneGrowing;

        private ITreeShape _crown;

        public Tree(Vector3 position)
        {
            _position = position;
            GenerateCrown();
            GenerateTrunk();
            GenerateObstacles();
        }

        private void GenerateObstacles()
        {
            /*var linePt = new Vector3(20, -20, 10);
            var dir = new Vector3(-1, -1.5f, -0.5f) * 3;

            for (var i = 0; i < 20; i++)
            {
                var leaf = new Leaf(linePt + dir * i) { Weight = -Leaf.MaxWeight / 4f };

                Leaves.Add(leaf);
            }*/
        }

        private void GenerateCrown()
        {
            _crown = new Sphere((int)_position.X - TreeWidth / 2, (int)_position.Y - TreeHeight - TrunkHeight, (int)_position.Z - TreeLength / 2, TreeWidth / 2/*, TreeHeight, TreeLength*/);
            Leaves = new List<Leaf>();

            //randomly place leaves within our rectangle
            for (var i = 0; i < LeafCount; i++)
            {
                var location = new Vector3(_crown.X + _random.Next(0, _crown.Width), _crown.Y + _random.Next(0, _crown.Height), _crown.Z + _random.Next(0, _crown.Length));
                if (!_crown.Contains(location))
                    continue;
                var leaf = new Leaf(location) { Weight = (float)((_random.NextDouble() + 0.1) * Leaf.MaxWeight - Leaf.MaxWeight / 2f) };

                Leaves.Add(leaf);
            }
        }

        private void GenerateTrunk()
        {
            Branches = new Dictionary<Vector3, Branch>();

            _root = new Branch(null, _position, new Vector3(0, -1, 0));
            Branches.Add(_root.Position, _root);

            var current = new Branch(_root, new Vector3(_position.X, _position.Y - BranchLength, _position.Z), new Vector3(0, -1, 0));
            Branches.Add(current.Position, current);

            //Keep growing trunk upwards until we reach a leaf       
            while ((_root.Position - current.Position).Length < TrunkHeight)
            {
                var trunk = new Branch(current, new Vector3(current.Position.X, current.Position.Y - BranchLength, current.Position.Z), new Vector3(0, -1, 0));
                Branches.Add(trunk.Position, trunk);
                current = trunk;
            }
        }

        public void Reset()
        {
            _growIter = 0;
            DoneGrowing = false;
            GenerateCrown();
            GenerateTrunk();
            GenerateObstacles();
        }

        public void Grow()
        {
            if (DoneGrowing) return;

            //If no leaves left, we are done
            if (Leaves.Count == 0 || _growIter > 200)
            {
                DoneGrowing = true;
                return;
            }

            //process the leaves
            for (var i = 0; i < Leaves.Count; i++)
            {
                var leafRemoved = false;

                Leaves[i].ClosestBranch = null;

                //Find the nearest branch for this leaf
                foreach (var b in Branches.Values)
                {
                    var direction = Leaves[i].Position - b.Position;
                    var distance = (float)System.Math.Round(direction.Length);            //distance to branch from leaf
                    direction.Normalize();

                    if (distance <= MinDistance)            //Min leaf distance reached, we remove it
                    {
                        Leaves.Remove(Leaves[i]);
                        i--;
                        leafRemoved = true;
                        break;
                    }

                    if (!(distance <= MaxDistance)) continue;

                    if (Leaves[i].ClosestBranch == null)
                        Leaves[i].ClosestBranch = b;
                    else if ((Leaves[i].Position - Leaves[i].ClosestBranch.Position).Length > distance)
                        Leaves[i].ClosestBranch = b;
                }

                //if the leaf was removed, skip
                if (leafRemoved) continue;
                //Set the grow parameters on all the closest branches that are in range
                if (Leaves[i].ClosestBranch == null) continue;

                var dir = Leaves[i].Position - Leaves[i].ClosestBranch.Position;
                dir.Normalize();
                Leaves[i].ClosestBranch.GrowDirection += dir * Leaves[i].Weight;       //add to grow direction of branch
                Leaves[i].ClosestBranch.GrowCount++;
            }

            //Generate the new branches
            var newBranches = new HashSet<Branch>();
            foreach (var b in Branches.Values)
            {
                if (b.GrowCount <= 0) continue;

                var avgDirection = b.GrowDirection / b.GrowCount;
                avgDirection.Normalize();

                var newBranch = new Branch(b, b.Position + avgDirection * BranchLength, avgDirection);

                //AgeParents(newBranch);

                newBranches.Add(newBranch);
                b.Reset();
            }

            //Add the new branches to the tree
            var branchAdded = false;
            foreach (var b in newBranches)
            {
                //Check if branch already exists.  These cases seem to happen when leaf is in specific areas
                if (Branches.TryGetValue(b.Position, out Branch existing)) continue;

                Branches.Add(b.Position, b);
                branchAdded = true;
            }

            //if no branches were added - we are done
            //this handles issues where leaves equal out each other, making branches grow without ever reaching the leaf
            if (!branchAdded)
                DoneGrowing = true;

            _growIter++;
        }

        private static void AgeParents(Branch branch)
        {
            branch.Age += 0.05f;
            if (branch.Parent == null)
                return;

            AgeParents(branch.Parent);
        }
    }
}
