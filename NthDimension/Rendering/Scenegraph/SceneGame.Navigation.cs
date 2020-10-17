namespace NthDimension.Rendering.Scenegraph
{
    using NthDimension.Algebra;
    using NthDimension.Physics;
    using NthDimension.Physics.Collision;
    using NthDimension.Physics.Collision.Shapes;
    using NthDimension.Physics.Dynamics;
    using NthDimension.Physics.Dynamics.Constraints;
    using NthDimension.Physics.LinearMath;
    using NthDimension.Rendering.Drawables.Models;
    using System;

    public partial class SceneGame
    {
        PhysicsWorld    worldPhysics;
        CollisionSystem collisionSystem;
        Shape           navigationMeshShape;
        RigidBody       navigationMesh;

        public ContactSettings PhysicsSettings
        {
            get 
            {
                if (null == worldPhysics) return null;                   
                return worldPhysics.ContactSettings; 
            }
        }

        public void CreatePhysics()
        {
            // creating a new collision system and adding it to the new world
            collisionSystem = new CollisionSystemSAP();
            worldPhysics = new PhysicsWorld(collisionSystem);
            worldPhysics.ContactSettings.AllowedPenetration     = .01f;
            worldPhysics.ContactSettings.BiasFactor             = .001f;
            worldPhysics.ContactSettings.BreakThreshold         = .01f;
            worldPhysics.ContactSettings.MaximumBias            = 10.0f;
            worldPhysics.ContactSettings.MinimumVelocity        = .01f;


            // Create the groundShape and the body.
            navigationMeshShape = new BoxShape(new JVector(groundSize, WaterLevel * 2, groundSize));

            // TODO:: NavigationMesh

            navigationMesh = new RigidBody(navigationMeshShape);


            // make the body static, so it can't be moved
            navigationMesh.IsStatic = true;

            // add the ground to the world.
            worldPhysics.AddBody(navigationMesh);
            navigationMesh.Tag = "NavigationMesh";
        }
        public void CreatePhysics(Terrain terrain, ContactSettings contactSettings/*, float scaleX = 1f, float scaleY = 1f*/)
        {
            // creating a new collision system and adding it to the new world
            collisionSystem = new CollisionSystemSAP();
            worldPhysics = new PhysicsWorld(collisionSystem);
            if (null == contactSettings)
            {
                worldPhysics.ContactSettings.AllowedPenetration         = .01f;
                worldPhysics.ContactSettings.BiasFactor                 = .001f;
                worldPhysics.ContactSettings.BreakThreshold             = .01f;
                worldPhysics.ContactSettings.MaximumBias                = 10.0f;
                worldPhysics.ContactSettings.MinimumVelocity            = .01f;
            }
            else
            {
                worldPhysics.ContactSettings.AllowedPenetration         = contactSettings.AllowedPenetration;
                worldPhysics.ContactSettings.BiasFactor                 = contactSettings.BiasFactor;
                worldPhysics.ContactSettings.BreakThreshold             = contactSettings.BreakThreshold;
                worldPhysics.ContactSettings.MaximumBias                = contactSettings.MaximumBias;
                worldPhysics.ContactSettings.MinimumVelocity            = contactSettings.MinimumVelocity;
            }

            System.Collections.Generic.List<JVector> jvectors = new System.Collections.Generic.List<JVector>();

            foreach (var point in terrain.Points)
                jvectors.Add(new JVector(point));

            System.Collections.Generic.List<TriangleVertexIndices> tidx = new System.Collections.Generic.List<TriangleVertexIndices>();

            for (int t = 0; t < terrain.Indices.Length; t += 2)
            {
                if (t + 2 < terrain.Indices.Length)
                    tidx.Add(new TriangleVertexIndices((int)terrain.Indices[t],
                                                       (int)terrain.Indices[t + 1],
                                                       (int)terrain.Indices[t + 2]));

            }

            Octree terrainOctree = new Octree(jvectors, tidx);

            navigationMeshShape = new TriangleMeshShape(terrainOctree);

            navigationMesh = new RigidBody(navigationMeshShape);

            // make the body static, so it can't be moved
            navigationMesh.IsStatic = true;

            // add the ground to the world.
            worldPhysics.AddBody(navigationMesh);
            navigationMesh.Tag = "NavigationMesh";
        }


        /// <summary>
        /// Adds a rigid body into the physics engine world
        /// </summary>
        /// <param name="body"></param>
        public void AddRigidBody(RigidBody body, bool activate = true)
        {            
            this.worldPhysics.AddBody(body);

            if (activate)
                body.IsActive = true;
        }
        /// <summary>
        /// Adds a constraint into the physics engine world
        /// </summary>
        /// <param name="constraint"></param>
        public void AddConstraint(Constraint constraint)
        {
            this.worldPhysics.AddConstraint(constraint);
        }
        /// <summary>
        /// Removes a rigid body from the physics engine world
        /// </summary>
        /// <param name="body"></param>
        public void RemoveRigidBody(RigidBody body)
        {
            this.worldPhysics.RemoveBody(body);
        }
        /// <summary>
        /// Removes a constraint from the physics engine world
        /// </summary>
        /// <param name="constraint"></param>
        public void RemoveConstraint(Constraint constraint)
        {
            this.worldPhysics.RemoveConstraint(constraint);
        }
        
        /// <summary>
        /// Ray cast collision test
        /// </summary>
        /// <param name="position">the position of the ray</param>
        /// <param name="direction">the direction of the ray</param>
        /// <param name="callback">collision callback function</param>
        /// <param name="bodyHit">the body that has collided with the ray</param>
        /// <param name="hitNormal">the normal of the collision</param>
        /// <param name="fraction"></param>
        /// <returns></returns>
        public bool CollisionRaycast(JVector position, JVector direction, RaycastCallback callback, out RigidBody bodyHit, out JVector hitNormal, out float fraction)
        {
            bool result = worldPhysics.CollisionSystem.Raycast(position,
                                                               direction, // Y = -1f jumps ok (low gravity)
                                                               callback,
                                                               out bodyHit,
                                                               out hitNormal,
                                                               out fraction);

            return result;
        }

    }
}
